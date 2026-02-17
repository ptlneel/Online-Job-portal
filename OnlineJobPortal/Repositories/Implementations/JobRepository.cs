using Microsoft.EntityFrameworkCore;
using OnlineJobPortal.Data;
using OnlineJobPortal.Models;
using OnlineJobPortal.Repositories.Interfaces;

namespace OnlineJobPortal.Repositories.Implementations;

public class JobRepository : GenericRepository<Job>, IJobRepository
{
    public JobRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(IEnumerable<Job> Jobs, int TotalCount)> SearchJobsAsync(string? query, int? categoryId, string? location, decimal? minSalary, int page, int pageSize, string sort)
    {
        var jobsQuery = _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.Employer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
            jobsQuery = jobsQuery.Where(j => j.Title.Contains(query) || j.Description.Contains(query));

        if (categoryId.HasValue)
            jobsQuery = jobsQuery.Where(j => j.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(location))
            jobsQuery = jobsQuery.Where(j => j.Location.Contains(location));

        if (minSalary.HasValue)
            jobsQuery = jobsQuery.Where(j => j.Salary >= minSalary.Value);

        jobsQuery = sort switch
        {
            "salary_desc" => jobsQuery.OrderByDescending(j => j.Salary),
            "salary_asc" => jobsQuery.OrderBy(j => j.Salary),
            "oldest" => jobsQuery.OrderBy(j => j.CreatedDate),
            _ => jobsQuery.OrderByDescending(j => j.CreatedDate)
        };

        var totalCount = await jobsQuery.CountAsync();
        var jobs = await jobsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (jobs, totalCount);
    }

    public async Task<IEnumerable<Job>> GetEmployerJobsAsync(string employerId)
        => await _context.Jobs.Include(j => j.Category).Where(j => j.EmployerId == employerId).OrderByDescending(j => j.CreatedDate).ToListAsync();

    public async Task<Job?> GetJobDetailsAsync(int id)
        => await _context.Jobs.Include(j => j.Category).Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == id);
}
