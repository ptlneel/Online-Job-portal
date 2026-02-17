using Microsoft.EntityFrameworkCore;
using OnlineJobPortal.Data;
using OnlineJobPortal.Models;
using OnlineJobPortal.Repositories.Interfaces;

namespace OnlineJobPortal.Repositories.Implementations;

public class JobApplicationRepository : GenericRepository<JobApplication>, IJobApplicationRepository
{
    public JobApplicationRepository(ApplicationDbContext context) : base(context) { }

    public Task<bool> ExistsAsync(int jobId, string userId)
        => _context.JobApplications.AnyAsync(x => x.JobId == jobId && x.UserId == userId);

    public async Task<IEnumerable<JobApplication>> GetApplicationsByEmployerAsync(string employerId)
        => await _context.JobApplications
            .Include(a => a.User)
            .Include(a => a.Job)
            .Where(a => a.Job!.EmployerId == employerId)
            .OrderByDescending(a => a.ApplyDate)
            .ToListAsync();

    public async Task<IEnumerable<JobApplication>> GetApplicationsByUserAsync(string userId)
        => await _context.JobApplications
            .Include(a => a.Job)!.ThenInclude(j => j!.Employer)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.ApplyDate)
            .ToListAsync();
}
