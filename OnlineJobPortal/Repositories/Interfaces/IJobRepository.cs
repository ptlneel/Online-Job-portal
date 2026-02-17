using OnlineJobPortal.Models;

namespace OnlineJobPortal.Repositories.Interfaces;

public interface IJobRepository : IGenericRepository<Job>
{
    Task<(IEnumerable<Job> Jobs, int TotalCount)> SearchJobsAsync(string? query, int? categoryId, string? location, decimal? minSalary, int page, int pageSize, string sort);
    Task<IEnumerable<Job>> GetEmployerJobsAsync(string employerId);
    Task<Job?> GetJobDetailsAsync(int id);
}
