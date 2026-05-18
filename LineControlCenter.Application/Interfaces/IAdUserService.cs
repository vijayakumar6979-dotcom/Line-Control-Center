namespace LineControlCenter.Application.Interfaces;

public record AdUserInfo(string NtId, string DisplayName, string Email);

public interface IAdUserService
{
    /// <summary>
    /// Looks up AD display name and email for the given NT login (e.g. "DOMAIN\username" or just "username").
    /// Returns null if the user cannot be found.
    /// </summary>
    AdUserInfo? GetUserInfo(string ntLogin);
}
