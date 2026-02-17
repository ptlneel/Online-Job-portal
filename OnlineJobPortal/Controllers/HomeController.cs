using Microsoft.AspNetCore.Mvc;
using OnlineJobPortal.Repositories.Interfaces;
using OnlineJobPortal.ViewModels;

namespace OnlineJobPortal.Controllers;

public class HomeController : Controller
{
    private readonly IJobRepository _jobRepository;

    public HomeController(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<IActionResult> Index([FromQuery] JobSearchViewModel vm)
    {
        var (jobs, totalCount) = await _jobRepository.SearchJobsAsync(vm.Query, vm.CategoryId, vm.Location, vm.MinSalary, vm.Page, vm.PageSize, vm.Sort);
        ViewBag.TotalCount = totalCount;
        ViewBag.Page = vm.Page;
        ViewBag.PageSize = vm.PageSize;
        return View((vm, jobs));
    }

    public IActionResult Privacy() => View();
    public IActionResult Error() => View();
}
