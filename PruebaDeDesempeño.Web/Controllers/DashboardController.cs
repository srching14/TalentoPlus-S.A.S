using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaDeDesempeño.Web.Services;

namespace PruebaDeDesempeño.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class DashboardController : Controller
{
    private readonly IEmployeeService _employeeService;

    public DashboardController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _employeeService.GetDashboardDataAsync();
        return View(model);
    }
}
