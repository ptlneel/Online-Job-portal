using OnlineJobPortal.Models;

namespace OnlineJobPortal.Repositories.Interfaces;

public interface IJobApplicationRepository : IGenericRepository<JobApplication>
{
    Task<bool> ExistsAsync(int jobId, string userId);
    Task<IEnumerable<JobApplication>> GetApplicationsByEmployerAsync(string employerId);
    Task<IEnumerable<JobApplication>> GetApplicationsByUserAsync(string userId);
}
