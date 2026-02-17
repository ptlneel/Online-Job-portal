using Microsoft.AspNetCore.Identity;
using OnlineJobPortal.Models;
using OnlineJobPortal.ViewModels;

namespace OnlineJobPortal.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model)
    {
        var user = new ApplicationUser
        {
            FullName = model.FullName,
            UserName = model.Email,
            Email = model.Email,
            Role = model.Role,
            IsApproved = model.Role == "Employer" ? false : true,
            CompanyName = model.CompanyName,
            CreatedDate = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, model.Role);
        return (true, Array.Empty<string>());
    }

    public async Task<(bool Success, string RedirectAction, string RedirectController, string? Error)> LoginAsync(LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || user.IsBlocked)
            return (false, "Login", "Account", "Invalid login attempt or user blocked.");

        if (user.Role == "Employer" && !user.IsApproved)
            return (false, "Login", "Account", "Employer account is pending admin approval.");

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (!result.Succeeded)
            return (false, "Login", "Account", "Invalid login attempt.");

        if (await _userManager.IsInRoleAsync(user, "Admin")) return (true, "Dashboard", "Admin", null);
        if (await _userManager.IsInRoleAsync(user, "Employer")) return (true, "Dashboard", "Employer", null);
        return (true, "Dashboard", "JobSeeker", null);
    }
}
