using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Role { get; set; }

    [MaxLength(255)]
    public string? ResumePath { get; set; }

    [MaxLength(150)]
    public string? CompanyName { get; set; }

    [MaxLength(255)]
    public string? CompanyLogo { get; set; }

    public bool IsApproved { get; set; }

    public bool IsBlocked { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<Job> PostedJobs { get; set; } = new List<Job>();
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
