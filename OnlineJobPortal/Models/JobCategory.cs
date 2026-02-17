using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.Models;

public class JobCategory
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
