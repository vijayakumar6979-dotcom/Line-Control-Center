using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using LineControlCenter.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LineControlCenter.UI.Services;

public class AdUserService : IAdUserService
{
    private readonly ILogger<AdUserService> _logger;

    public AdUserService(ILogger<AdUserService> logger)
    {
        _logger = logger;
    }

    public AdUserInfo? GetUserInfo(string ntLogin)
    {
        if (string.IsNullOrWhiteSpace(ntLogin))
            return null;

        // Strip domain prefix (e.g. "JABIL\vijay" → "vijay")
        var samAccount = ntLogin.Contains('\\')
            ? ntLogin[(ntLogin.IndexOf('\\') + 1)..]
            : ntLogin;

        if (!OperatingSystem.IsWindows())
        {
            _logger.LogWarning("AD lookup is only supported on Windows. Returning NT login as fallback.");
            return new AdUserInfo(samAccount, samAccount, string.Empty);
        }

        return LookupOnWindows(samAccount);
    }

    [SupportedOSPlatform("windows")]
    private AdUserInfo LookupOnWindows(string samAccount)
    {
        try
        {
            using var ctx  = new PrincipalContext(ContextType.Domain);
            using var user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, samAccount);

            if (user is null)
                return new AdUserInfo(samAccount, samAccount, string.Empty);

            var displayName = user.DisplayName ?? user.Name ?? samAccount;
            var email       = user.EmailAddress ?? string.Empty;

            return new AdUserInfo(samAccount, displayName, email);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AD lookup failed for user '{User}'. Falling back to NT login.", samAccount);
            return new AdUserInfo(samAccount, samAccount, string.Empty);
        }
    }
}
