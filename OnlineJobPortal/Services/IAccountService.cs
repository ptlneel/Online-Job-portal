using OnlineJobPortal.ViewModels;

namespace OnlineJobPortal.Services;

public interface IAccountService
{
    Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model);
    Task<(bool Success, string RedirectAction, string RedirectController, string? Error)> LoginAsync(LoginViewModel model);
}
