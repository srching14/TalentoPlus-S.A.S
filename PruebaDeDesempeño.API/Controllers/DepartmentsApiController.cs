using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaDeDesempeño.Web.Data;

namespace PruebaDeDesempeño.API.Controllers;

/// <summary>
/// API Controller público para departamentos
/// </summary>
[ApiController]
[Route("api/departments")]
public class DepartmentsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Listar todos los departamentos (público - sin JWT)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
    {
        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            })
            .ToListAsync();

        return Ok(departments);
    }
}

public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
