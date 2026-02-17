using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineJobPortal.Models;
using OnlineJobPortal.Repositories.Interfaces;
using OnlineJobPortal.Services;

namespace OnlineJobPortal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepository<JobCategory> _categoryRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IJobApplicationRepository _applicationRepository;

    public AdminController(IDashboardService dashboardService, UserManager<ApplicationUser> userManager,
        IGenericRepository<JobCategory> categoryRepository, IJobRepository jobRepository, IJobApplicationRepository applicationRepository)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
        _categoryRepository = categoryRepository;
        _jobRepository = jobRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<IActionResult> Dashboard() => View(await _dashboardService.GetAdminStatsAsync());

    public IActionResult PendingEmployers() => View(_userManager.Users.Where(x => x.Role == "Employer" && !x.IsApproved).ToList());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveEmployer(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) { user.IsApproved = true; await _userManager.UpdateAsync(user); }
        return RedirectToAction(nameof(PendingEmployers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) { user.IsBlocked = true; await _userManager.UpdateAsync(user); }
        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Categories() => View(await _categoryRepository.GetAllAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(JobCategory category)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Categories));
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var item = await _categoryRepository.GetByIdAsync(id);
        if (item != null)
        {
            _categoryRepository.Delete(item);
            await _categoryRepository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Categories));
    }

    public async Task<IActionResult> Jobs() => View(await _jobRepository.GetAllAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job != null)
        {
            _jobRepository.Delete(job);
            await _jobRepository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Jobs));
    }

    public async Task<IActionResult> Applications() => View(await _applicationRepository.GetAllAsync());
}
