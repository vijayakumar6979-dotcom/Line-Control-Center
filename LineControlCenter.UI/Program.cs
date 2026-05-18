using LineControlCenter.Application.Interfaces;
using LineControlCenter.Application.Services;
using LineControlCenter.Application.Settings;
using LineControlCenter.Infrastructure.Data;
using LineControlCenter.UI.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddRadzenComponents();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(LineControlCenter.Application.Queries.TestData.GetBkTestDataQuery).Assembly);
    cfg.AddOpenBehavior(typeof(LineControlCenter.Application.Behaviours.ExceptionHandlingBehavior<,>));
    cfg.AddOpenBehavior(typeof(LineControlCenter.Application.Behaviours.ValidationBehavior<,>));
});

builder.Services.AddDbContext<ManufacturingDbContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IManufacturingDbContext>(sp => sp.GetRequiredService<ManufacturingDbContext>());
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ManufacturingDbContext>());
builder.Services.AddScoped<IBkTestTarRawDataService, BkTestTarRawDataService>();
builder.Services.AddScoped<IBkFctUphService, BkFctUphService>();

builder.Services.AddDbContextFactory<PostgresqlDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"),
        npgsql => npgsql.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null)));
// Transient: each MediatR handler resolves a fresh DbContext instance from the factory.
// This prevents "A second operation was started on this context instance..." when
// concurrent components (e.g. dashboard timer + NcrCarSummaryPanel) issue queries
// on the same Blazor circuit.
builder.Services.AddTransient<IPostgresqlDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<PostgresqlDbContext>>().CreateDbContext());
builder.Services.AddSingleton<IPostgresqlDbContextFactory, PostgresqlDbContextFactory>();
builder.Services.AddScoped<ILccSafetyTblService, LccSafetyTblService>();
builder.Services.AddScoped<ILccNcrCarService, LccNcrCarService>();
builder.Services.AddScoped<IJcasCarService, JcasCarService>();
builder.Services.AddScoped<RefreshStateService>();

// Reverse proxy for SOMS bay camera.
// UseDefaultCredentials passes the app-pool / service-account Windows identity so
// SOMS doesn't redirect to login. The proxy also strips the CSP frame-ancestors
// header that prevents the iframe from loading inside our dashboard.
builder.Services.AddHttpClient("SomsProxy", client =>
{
    client.BaseAddress = new Uri("http://awase1pgaict01:7777");
    client.Timeout     = TimeSpan.FromSeconds(30);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AllowAutoRedirect      = false,
    UseCookies             = false,
    UseDefaultCredentials  = true,          // Windows auth passthrough
    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                           | System.Net.DecompressionMethods.Deflate
                           | System.Net.DecompressionMethods.Brotli,
});

builder.Services.Configure<DpmSettings>(
    builder.Configuration.GetSection(DpmSettings.SectionName));

// Factory + transient: each handler gets a fresh context; prevents concurrent-context errors.
builder.Services.AddDbContextFactory<JbkTeDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("JbkTeConnection"),
        npgsql => npgsql.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null)));
builder.Services.AddTransient<IJbkTeDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<JbkTeDbContext>>().CreateDbContext());

builder.Services.AddScoped<LineControlCenter.Application.Interfaces.IFeedbackService,
                           LineControlCenter.UI.Services.FeedbackService>();
builder.Services.AddScoped<LineControlCenter.Application.Interfaces.IEmailReportService,
                           LineControlCenter.UI.Services.EmailReportService>();
builder.Services.AddSingleton<LineControlCenter.Application.Interfaces.IAdUserService,
                              LineControlCenter.UI.Services.AdUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LineControlCenter.UI.Services.UserSessionService>();

// Windows Authentication so HttpContext.User.Identity.Name resolves to DOMAIN\username.
// - When hosted in IIS / IIS Express → IIS performs the auth, app uses IISDefaults scheme.
// - When self-hosted (Kestrel / dotnet run) → use Negotiate (Kerberos / NTLM).
if (builder.Environment.IsDevelopment() == false ||
    string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_IIS_HTTPAUTH"), "negotiate;ntlm;", StringComparison.OrdinalIgnoreCase) ||
    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_IIS_HTTPAUTH")))
{
    builder.Services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);
}
else
{
    builder.Services
        .AddAuthentication(Microsoft.AspNetCore.Authentication.Negotiate.NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate();
}
builder.Services.AddAuthorization();

var app = builder.Build();

// The SOMS bay camera iframe is embedded via the /soms-proxy reverse proxy (same-origin,
// HTTPS) so there are no mixed-content issues. Direct links to the upstream HTTP server
// are intentionally routed through the proxy as well.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// ── SOMS reverse proxy ──────────────────────────────────────────────────────
// Strips CSP frame-ancestors and X-Frame-Options so the iframe can embed,
// and authenticates via Windows credentials so SOMS returns bay data.
app.Map("/soms-proxy/{**path}", async (
    string? path,
    HttpContext httpContext,
    IHttpClientFactory httpClientFactory) =>
{
    const string upstreamBase = "http://awase1pgaict01:7777";
    const string proxyBase    = "/soms-proxy";

    // WebSocket upgrade requests (Upgrade: websocket / 101 Switching Protocols) cannot
    // be tunnelled through HttpClient. Return 501 immediately so the caller fails fast
    // rather than committing a 101 response and then crashing in the catch block when
    // the remote host closes the connection.
    if (httpContext.WebSockets.IsWebSocketRequest ||
        string.Equals(httpContext.Request.Headers.Upgrade, "websocket", StringComparison.OrdinalIgnoreCase))
    {
        httpContext.Response.StatusCode = 501; // Not Implemented
        await httpContext.Response.WriteAsync("WebSocket connections are not supported through the SOMS proxy.");
        return;
    }

    var client   = httpClientFactory.CreateClient("SomsProxy");
    var query    = httpContext.Request.QueryString.Value ?? string.Empty;
    var upstream = $"/{path ?? string.Empty}{query}";

    try
    {
        using var req = new HttpRequestMessage(new HttpMethod(httpContext.Request.Method), upstream);

        foreach (var h in httpContext.Request.Headers)
        {
            if (h.Key.Equals("Host",              StringComparison.OrdinalIgnoreCase)) continue;
            if (h.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase)) continue;
            try { req.Headers.TryAddWithoutValidation(h.Key, h.Value.ToArray()); } catch { }
        }

        if (httpContext.Request.ContentLength > 0 || httpContext.Request.Headers.ContainsKey("Transfer-Encoding"))
        {
            req.Content = new StreamContent(httpContext.Request.Body);
            if (httpContext.Request.ContentType is { } ct)
                req.Content.Headers.TryAddWithoutValidation("Content-Type", ct);
        }

        using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

        httpContext.Response.StatusCode = (int)resp.StatusCode;

        foreach (var h in resp.Headers.Concat(resp.Content.Headers))
        {
            // Strip headers that block iframe embedding
            if (h.Key.Equals("X-Frame-Options",         StringComparison.OrdinalIgnoreCase)) continue;
            if (h.Key.Equals("Content-Security-Policy", StringComparison.OrdinalIgnoreCase)) continue;
            if (h.Key.Equals("Transfer-Encoding",       StringComparison.OrdinalIgnoreCase)) continue;
            if (h.Key.Equals("Content-Length",          StringComparison.OrdinalIgnoreCase)) continue;
            if (h.Key.Equals("Location", StringComparison.OrdinalIgnoreCase))
            {
                var loc = (h.Value.FirstOrDefault() ?? string.Empty)
                    .Replace(upstreamBase, proxyBase, StringComparison.OrdinalIgnoreCase);
                httpContext.Response.Headers["Location"] = loc;
                continue;
            }
            try { httpContext.Response.Headers.TryAdd(h.Key, h.Value.ToArray()); } catch { }
        }

        var contentType = resp.Content.Headers.ContentType?.MediaType ?? string.Empty;
        bool isText = contentType.Contains("html",       StringComparison.OrdinalIgnoreCase)
                   || contentType.Contains("javascript", StringComparison.OrdinalIgnoreCase)
                   || contentType.Contains("css",        StringComparison.OrdinalIgnoreCase)
                   || contentType.Contains("json",       StringComparison.OrdinalIgnoreCase)
                   || contentType.Contains("text/",      StringComparison.OrdinalIgnoreCase);

        if (isText)
        {
            var enc  = resp.Content.Headers.ContentType?.CharSet is { } cs
                     ? System.Text.Encoding.GetEncoding(cs)
                     : System.Text.Encoding.UTF8;
            var body = await resp.Content.ReadAsStringAsync();

            await httpContext.Response.Body.WriteAsync(enc.GetBytes(body));
        }
        else
        {
            await resp.Content.CopyToAsync(httpContext.Response.Body);
        }
    }
    catch (Exception ex)
    {
        // If headers have already been flushed (e.g. the upstream started streaming
        // and then dropped the connection) we cannot change the status code; just
        // abort the response so the client sees an incomplete response rather than
        // an unhandled server exception.
        if (httpContext.Response.HasStarted)
        {
            httpContext.Abort();
            return;
        }

        httpContext.Response.StatusCode  = 502;
        httpContext.Response.ContentType = "text/plain";
        await httpContext.Response.WriteAsync($"SOMS proxy error: {ex.Message}");
    }
}).AllowAnonymous().DisableAntiforgery();

app.MapRazorComponents<LineControlCenter.UI.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();