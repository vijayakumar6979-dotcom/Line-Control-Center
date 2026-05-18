namespace LineControlCenter.UI.Services;

/// <summary>
/// Scoped service — captures the Windows NT login from HttpContext during
/// the initial SSR pass (App.razor) before the SignalR WebSocket upgrade,
/// then makes it available throughout the Blazor circuit lifetime.
/// </summary>
public class UserSessionService
{
    /// <summary>NT login in "DOMAIN\username" or "username" form.</summary>
    public string NtLogin { get; private set; } = string.Empty;

    public void SetNtLogin(string? ntLogin)
    {
        if (!string.IsNullOrWhiteSpace(ntLogin))
            NtLogin = ntLogin;
    }
}
