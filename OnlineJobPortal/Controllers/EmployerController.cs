using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineJobPortal.Models;
using OnlineJobPortal.Repositories.Interfaces;
using OnlineJobPortal.Services;
using OnlineJobPortal.ViewModels;

namespace OnlineJobPortal.Controllers;

[Authorize(Roles = "Employer")]
public class EmployerController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJobRepository _jobRepository;
    private readonly IGenericRepository<JobCategory> _categoryRepository;
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IDashboardService _dashboardService;
    private readonly IWebHostEnvironment _env;

    public EmployerController(UserManager<ApplicationUser> userManager, IJobRepository jobRepository,
        IGenericRepository<JobCategory> categoryRepository, IJobApplicationRepository applicationRepository,
        IDashboardService dashboardService, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _jobRepository = jobRepository;
        _categoryRepository = categoryRepository;
        _applicationRepository = applicationRepository;
        _dashboardService = dashboardService;
        _env = env;
    }

    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(await _dashboardService.GetEmployerStatsAsync(user!.Id));
    }

    [HttpGet]
    public async Task<IActionResult> CompanyProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompanyProfile(ApplicationUser model, IFormFile? logo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        user.CompanyName = model.CompanyName;
        if (logo != null && logo.Length > 0)
        {
            var allowed = new[] { ".png", ".jpg", ".jpeg", ".webp" };
            var ext = Path.GetExtension(logo.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext) || logo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError(string.Empty, "Logo must be image file under 2MB.");
                return View(user);
            }

            var folder = Path.Combine(_env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(folder, fileName);
            await using var stream = new FileStream(path, FileMode.Create);
            await logo.CopyToAsync(stream);
            user.CompanyLogo = $"/uploads/logos/{fileName}";
        }

        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Company profile updated";
        return RedirectToAction(nameof(CompanyProfile));
    }

    public async Task<IActionResult> ManageJobs() => View(await _jobRepository.GetEmployerJobsAsync(_userManager.GetUserId(User)!));

    [HttpGet]
    public async Task<IActionResult> CreateJob()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return View(new JobCreateEditViewModel
        {
            Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString()))
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJob(JobCreateEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var entity = new Job
        {
            Title = vm.Title,
            Description = vm.Description,
            Salary = vm.Salary,
            Location = vm.Location,
            CategoryId = vm.CategoryId,
            EmployerId = _userManager.GetUserId(User)!,
            CreatedDate = DateTime.UtcNow
        };
        await _jobRepository.AddAsync(entity);
        await _jobRepository.SaveChangesAsync();
        return RedirectToAction(nameof(ManageJobs));
    }

    public async Task<IActionResult> Applicants() => View(await _applicationRepository.GetApplicationsByEmployerAsync(_userManager.GetUserId(User)!));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, string status)
    {
        var app = await _applicationRepository.GetByIdAsync(id);
        if (app == null) return NotFound();
        app.Status = status;
        _applicationRepository.Update(app);
        await _applicationRepository.SaveChangesAsync();
        return RedirectToAction(nameof(Applicants));
    }
}
