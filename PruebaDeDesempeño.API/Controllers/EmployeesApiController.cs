using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;

namespace PruebaDeDesempeño.API.Controllers;

/// <summary>
/// API Controller para gestión de empleados
/// </summary>
[ApiController]
[Route("api/admin/employees")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class EmployeesApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeesApiController> _logger;

    public EmployeesApiController(ApplicationDbContext context, ILogger<EmployeesApiController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los empleados activos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                DocumentNumber = e.DocumentNumber,
                DocumentType = e.DocumentType,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                Position = e.Position,
                Salary = e.Salary,
                Status = e.Status.ToString(),
                DepartmentName = e.Department != null ? e.Department.Name : null,
                HireDate = e.HireDate
            })
            .ToListAsync();

        return Ok(employees);
    }

    /// <summary>
    /// Obtiene un empleado por su ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.Id == id && e.IsActive)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                DocumentNumber = e.DocumentNumber,
                DocumentType = e.DocumentType,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                Address = e.Address,
                BirthDate = e.BirthDate,
                Gender = e.Gender,
                Position = e.Position,
                Salary = e.Salary,
                Status = e.Status.ToString(),
                EducationLevel = e.EducationLevel.ToString(),
                ProfessionalProfile = e.ProfessionalProfile,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department != null ? e.Department.Name : null,
                HireDate = e.HireDate
            })
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            return NotFound(new { message = "Empleado no encontrado" });
        }

        return Ok(employee);
    }

    /// <summary>
    /// Crea un nuevo empleado
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verificar si el documento ya existe
        if (await _context.Employees.AnyAsync(e => e.DocumentNumber == dto.DocumentNumber))
        {
            return Conflict(new { message = "Ya existe un empleado con ese número de documento" });
        }

        // Verificar si el email ya existe
        if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
        {
            return Conflict(new { message = "Ya existe un empleado con ese email" });
        }

        // Verificar que el departamento exista
        var department = await _context.Departments.FindAsync(dto.DepartmentId);
        if (department == null)
        {
            return BadRequest(new { message = "Departamento no encontrado" });
        }

        var employee = new Employee
        {
            DocumentNumber = dto.DocumentNumber,
            DocumentType = dto.DocumentType ?? "CC",
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            Position = dto.Position,
            Salary = dto.Salary,
            HireDate = dto.HireDate ?? DateTime.UtcNow,
            Status = EmployeeStatus.Activo,
            EducationLevel = dto.EducationLevel ?? EducationLevel.Bachillerato,
            ProfessionalProfile = dto.ProfessionalProfile,
            DepartmentId = dto.DepartmentId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Empleado creado: {FullName} (ID: {Id})", employee.FullName, employee.Id);

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, new EmployeeDto
        {
            Id = employee.Id,
            DocumentNumber = employee.DocumentNumber,
            FullName = employee.FullName,
            Email = employee.Email,
            Position = employee.Position,
            DepartmentName = department.Name
        });
    }

    /// <summary>
    /// Actualiza un empleado existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null || !employee.IsActive)
        {
            return NotFound(new { message = "Empleado no encontrado" });
        }

        // Verificar email único (excluyendo el empleado actual)
        if (!string.IsNullOrEmpty(dto.Email) &&
            await _context.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id))
        {
            return Conflict(new { message = "Ya existe un empleado con ese email" });
        }

        // Actualizar campos
        if (!string.IsNullOrEmpty(dto.FullName)) employee.FullName = dto.FullName;
        if (!string.IsNullOrEmpty(dto.Email)) employee.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Phone)) employee.Phone = dto.Phone;
        if (!string.IsNullOrEmpty(dto.Address)) employee.Address = dto.Address;
        if (!string.IsNullOrEmpty(dto.Position)) employee.Position = dto.Position;
        if (dto.Salary.HasValue) employee.Salary = dto.Salary.Value;
        if (dto.DepartmentId.HasValue)
        {
            var dept = await _context.Departments.FindAsync(dto.DepartmentId.Value);
            if (dept == null) return BadRequest(new { message = "Departamento no encontrado" });
            employee.DepartmentId = dto.DepartmentId.Value;
        }

        if (dto.Status.HasValue) employee.Status = dto.Status.Value;

        employee.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Elimina un empleado (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null || !employee.IsActive)
        {
            return NotFound(new { message = "Empleado no encontrado" });
        }

        employee.IsActive = false;
        employee.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Empleado eliminado: {FullName} (ID: {Id})", employee.FullName, employee.Id);

        return NoContent();
    }
}

// DTOs
public class EmployeeDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string? Status { get; set; }
    public string? EducationLevel { get; set; }
    public string? ProfessionalProfile { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime HireDate { get; set; }
}

public class CreateEmployeeDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime? HireDate { get; set; }
    public EducationLevel? EducationLevel { get; set; }
    public string? ProfessionalProfile { get; set; }
    public int DepartmentId { get; set; }
}

public class UpdateEmployeeDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Position { get; set; }
    public decimal? Salary { get; set; }
    public int? DepartmentId { get; set; }
    public EmployeeStatus? Status { get; set; }
}
