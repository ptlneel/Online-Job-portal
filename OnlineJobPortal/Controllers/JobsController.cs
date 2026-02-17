using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineJobPortal.Models;
using OnlineJobPortal.Repositories.Interfaces;
using OnlineJobPortal.ViewModels;

namespace OnlineJobPortal.Controllers;

public class JobsController : Controller
{
    private readonly IJobRepository _jobRepository;
    private readonly IGenericRepository<JobCategory> _categoryRepository;
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public JobsController(IJobRepository jobRepository, IGenericRepository<JobCategory> categoryRepository, IJobApplicationRepository applicationRepository, UserManager<ApplicationUser> userManager)
    {
        _jobRepository = jobRepository;
        _categoryRepository = categoryRepository;
        _applicationRepository = applicationRepository;
        _userManager = userManager;
    }

    public async Task<IActionResult> Details(int id)
    {
        var job = await _jobRepository.GetJobDetailsAsync(id);
        if (job == null) return NotFound();
        return View(job);
    }

    [Authorize(Roles = "Employer")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) return NotFound();

        var categories = await _categoryRepository.GetAllAsync();
        return View(new JobCreateEditViewModel
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Salary = job.Salary,
            Location = job.Location,
            CategoryId = job.CategoryId,
            Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString()))
        });
    }

    [Authorize(Roles = "Employer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(JobCreateEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var job = await _jobRepository.GetByIdAsync(vm.Id);
        if (job == null) return NotFound();

        job.Title = vm.Title;
        job.Description = vm.Description;
        job.Salary = vm.Salary;
        job.Location = vm.Location;
        job.CategoryId = vm.CategoryId;
        _jobRepository.Update(job);
        await _jobRepository.SaveChangesAsync();
        return RedirectToAction("ManageJobs", "Employer");
    }

    [Authorize(Roles = "Employer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job != null)
        {
            _jobRepository.Delete(job);
            await _jobRepository.SaveChangesAsync();
        }
        return RedirectToAction("ManageJobs", "Employer");
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(int jobId)
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _userManager.GetUserAsync(User);
        if (user?.ResumePath == null)
            return Json(new { success = false, message = "Please upload your resume before applying." });

        if (await _applicationRepository.ExistsAsync(jobId, userId))
            return Json(new { success = false, message = "You have already applied for this job." });

        await _applicationRepository.AddAsync(new JobApplication
        {
            JobId = jobId,
            UserId = userId,
            ApplyDate = DateTime.UtcNow,
            Status = "Submitted"
        });
        await _applicationRepository.SaveChangesAsync();
        return Json(new { success = true, message = "Application submitted successfully." });
    }
}
