using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineJobPortal.ViewModels;

public class JobCreateEditViewModel
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 99999999)]
    public decimal Salary { get; set; }

    [Required, StringLength(150)]
    public string Location { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = [];
}

public class JobSearchViewModel
{
    public string? Query { get; set; }
    public int? CategoryId { get; set; }
    public string? Location { get; set; }
    public decimal? MinSalary { get; set; }
    public string Sort { get; set; } = "latest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 6;
}
