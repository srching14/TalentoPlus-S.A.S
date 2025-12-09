using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.Services;
using PruebaDeDesempeño.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace PruebaDeDesempeño.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IExcelImportService _excelImportService;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfService _pdfService;
    private readonly ApplicationDbContext _context;

    public EmployeesController(
        IEmployeeService employeeService,
        IExcelImportService excelImportService,
        IExcelExportService excelExportService,
        IPdfService pdfService,
        ApplicationDbContext context)
    {
        _employeeService = employeeService;
        _excelImportService = excelImportService;
        _excelExportService = excelExportService;
        _pdfService = pdfService;
        _context = context;
    }

    // GET: Employees
    public async Task<IActionResult> Index(string? searchTerm, int? departmentId, EmployeeStatus? status)
    {
        ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name", departmentId);
        ViewBag.SearchTerm = searchTerm;
        ViewBag.Status = status;

        var employees = await _employeeService.SearchEmployeesAsync(searchTerm, departmentId, status);
        return View(employees);
    }

    // GET: Employees/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // GET: Employees/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
        return View();
    }

    // POST: Employees/Createq
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _employeeService.CreateEmployeeAsync(model))
            {
                TempData["SuccessMessage"] = "Empleado creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Ocurrió un error al crear el empleado.");
        }

        ViewBag.Departments =
            new SelectList(await _context.Departments.ToListAsync(), "Id", "Name", model.DepartmentId);
        return View(model);
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        ViewBag.Departments =
            new SelectList(await _context.Departments.ToListAsync(), "Id", "Name", employee.DepartmentId);
        return View(employee);
    }

    // POST: Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (await _employeeService.UpdateEmployeeAsync(model))
            {
                TempData["SuccessMessage"] = "Empleado actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Ocurrió un error al actualizar el empleado.");
        }

        ViewBag.Departments =
            new SelectList(await _context.Departments.ToListAsync(), "Id", "Name", model.DepartmentId);
        return View(model);
    }

    // POST: Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (await _employeeService.DeleteEmployeeAsync(id))
        {
            TempData["SuccessMessage"] = "Empleado eliminado exitosamente.";
        }
        else
        {
            TempData["ErrorMessage"] = "Error al eliminar el empleado.";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Employees/ImportExcel
    public IActionResult ImportExcel()
    {
        return View();
    }

    // POST: Employees/ImportExcel
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Por favor seleccione un archivo.");
            return View();
        }

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("", "El archivo debe ser un Excel (.xlsx).");
            return View();
        }

        using var stream = file.OpenReadStream();
        var result = await _excelImportService.ImportEmployeesFromExcelAsync(stream, file.FileName);

        // Renderizar vista de resultados (reusando la vista Import/Result si es posible, o crear una nueva)
        // Como eliminé ImportController, usaré una vista en Employees/ImportResult
        return View("ImportResult", result);
    }

    // GET: Employees/ExportExcel
    public async Task<IActionResult> ExportExcel()
    {
        var content = await _excelExportService.ExportEmployeesAsync();
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Empleados_TalentoPlus.xlsx");
    }

    // GET: Employees/DownloadCv/5
    public async Task<IActionResult> DownloadCv(int id)
    {
        var employee = await _employeeService.GetEmployeeEntityByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var pdfBytes = _pdfService.GenerateEmployeeCV(employee);
        return File(pdfBytes, "application/pdf", $"CV_{employee.DocumentNumber}.pdf");
    }
}
