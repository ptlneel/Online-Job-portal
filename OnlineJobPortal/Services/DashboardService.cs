using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineJobPortal.Data;
using OnlineJobPortal.Models;

namespace OnlineJobPortal.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<object> GetAdminStatsAsync()
    {
        var users = _userManager.Users.AsQueryable();
        return new
        {
            TotalUsers = await users.CountAsync(),
            TotalEmployers = await users.CountAsync(u => u.Role == "Employer"),
            TotalJobSeekers = await users.CountAsync(u => u.Role == "JobSeeker"),
            TotalJobs = await _context.Jobs.CountAsync(),
            TotalApplications = await _context.JobApplications.CountAsync()
        };
    }

    public async Task<object> GetEmployerStatsAsync(string employerId) => new
    {
        TotalJobsPosted = await _context.Jobs.CountAsync(j => j.EmployerId == employerId),
        TotalApplicationsReceived = await _context.JobApplications.CountAsync(a => a.Job!.EmployerId == employerId)
    };

    public async Task<object> GetJobSeekerStatsAsync(string userId) => new
    {
        TotalAppliedJobs = await _context.JobApplications.CountAsync(a => a.UserId == userId)
    };
}
