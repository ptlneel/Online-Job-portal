using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineJobPortal.Models;

public class Job
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 99999999)]
    public decimal Salary { get; set; }

    [Required, StringLength(150)]
    public string Location { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }
    public JobCategory? Category { get; set; }

    [Required]
    public string EmployerId { get; set; } = string.Empty;
    public ApplicationUser? Employer { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
