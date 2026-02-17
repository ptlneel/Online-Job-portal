namespace OnlineJobPortal.Services;

public interface IDashboardService
{
    Task<object> GetAdminStatsAsync();
    Task<object> GetEmployerStatsAsync(string employerId);
    Task<object> GetJobSeekerStatsAsync(string userId);
}
