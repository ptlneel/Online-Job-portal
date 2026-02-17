using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models;

public class JobApplication
{
    public int Id { get; set; }

    public int JobId { get; set; }
    public Job? Job { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public DateTime ApplyDate { get; set; } = DateTime.UtcNow;

    [Required, StringLength(30)]
    public string Status { get; set; } = "Submitted";
}
