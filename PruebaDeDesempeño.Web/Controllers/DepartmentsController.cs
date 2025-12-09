using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.ViewModels;

namespace PruebaDeDesempeño.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class DepartmentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Departments
    public async Task<IActionResult> Index()
    {
        var departments = await _context.Departments
            .Select(d => new DepartmentViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                EmployeeCount = d.Employees.Count(e => e.IsActive)
            })
            .ToListAsync();
        return View(departments);
    }

    // GET: Departments/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Departments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            var department = new Department
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Departments/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        var model = new DepartmentViewModel
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description
        };
        return View(model);
    }

    // POST: Departments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DepartmentViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            department.Name = model.Name;
            department.Description = model.Description;
            department.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // POST: Departments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department != null)
        {
            // Check if department has employees
            var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id && e.IsActive);
            if (hasEmployees)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el departamento porque tiene empleados asignados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
