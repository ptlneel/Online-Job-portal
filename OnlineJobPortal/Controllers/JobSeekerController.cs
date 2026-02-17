using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineJobPortal.Models;
using OnlineJobPortal.Repositories.Interfaces;
using OnlineJobPortal.Services;

namespace OnlineJobPortal.Controllers;

[Authorize(Roles = "JobSeeker")]
public class JobSeekerController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IDashboardService _dashboardService;
    private readonly IWebHostEnvironment _env;

    public JobSeekerController(UserManager<ApplicationUser> userManager, IJobApplicationRepository applicationRepository, IDashboardService dashboardService, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _applicationRepository = applicationRepository;
        _dashboardService = dashboardService;
        _env = env;
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User)!;
        return View(await _dashboardService.GetJobSeekerStatsAsync(userId));
    }

    [HttpGet]
    public async Task<IActionResult> Profile() => View(await _userManager.GetUserAsync(User));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ApplicationUser model, IFormFile? resume)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        user.FullName = model.FullName;

        if (resume != null && resume.Length > 0)
        {
            var ext = Path.GetExtension(resume.FileName).ToLowerInvariant();
            if (ext != ".pdf" || resume.Length > 3 * 1024 * 1024)
            {
                ModelState.AddModelError(string.Empty, "Resume must be PDF under 3MB.");
                return View(user);
            }

            var folder = Path.Combine(_env.WebRootPath, "uploads", "resumes");
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid()}.pdf";
            var path = Path.Combine(folder, fileName);
            await using var stream = new FileStream(path, FileMode.Create);
            await resume.CopyToAsync(stream);
            user.ResumePath = $"/uploads/resumes/{fileName}";
        }

        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Profile updated";
        return RedirectToAction(nameof(Profile));
    }

    public async Task<IActionResult> AppliedJobs()
        => View(await _applicationRepository.GetApplicationsByUserAsync(_userManager.GetUserId(User)!));
}
