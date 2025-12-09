using Xunit;
using Microsoft.EntityFrameworkCore;
using PruebaDeDesempeño.Web.Services;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;

namespace PruebaDeDesempeño.Tests.Services;

/// <summary>
/// Pruebas unitarias para EmployeeService
/// </summary>
public class EmployeeServiceTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    /// <summary>
    /// Prueba unitaria 1: Verificar que GetAllEmployeesAsync retorna solo empleados activos
    /// </summary>
    [Fact]
    public async Task GetAllEmployeesAsync_ReturnsOnlyActiveEmployees()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Crear departamento
        var department = new Department
        {
            Id = 1,
            Name = "Tecnología",
            Description = "Desarrollo",
            CreatedAt = DateTime.UtcNow
        };
        context.Departments.Add(department);

        // Crear empleados
        context.Employees.AddRange(
            new Employee
            {
                FullName = "Juan Activo",
                Email = "juan@test.com",
                DocumentNumber = "123456",
                DocumentType = "CC",
                Position = "Developer",
                DepartmentId = 1,
                IsActive = true,
                Status = EmployeeStatus.Activo,
                HireDate = DateTime.UtcNow,
                Salary = 5000000,
                CreatedAt = DateTime.UtcNow
            },
            new Employee
            {
                FullName = "Pedro Inactivo",
                Email = "pedro@test.com",
                DocumentNumber = "654321",
                DocumentType = "CC",
                Position = "Tester",
                DepartmentId = 1,
                IsActive = false, // Empleado inactivo
                Status = EmployeeStatus.Inactivo,
                HireDate = DateTime.UtcNow,
                Salary = 4000000,
                CreatedAt = DateTime.UtcNow
            }
        );
        await context.SaveChangesAsync();

        var employeeService = new EmployeeService(context);

        // Act
        var result = await employeeService.GetAllEmployeesAsync();

        // Assert
        Assert.Single(result); // Solo 1 empleado activo
        Assert.Equal("Juan Activo", result.First().FullName);
    }

    /// <summary>
    /// Prueba unitaria 2: Verificar que GetEmployeeByIdAsync retorna el empleado correcto
    /// </summary>
    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsCorrectEmployee()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        var department = new Department
        {
            Id = 1,
            Name = "Ventas",
            Description = "Comercial",
            CreatedAt = DateTime.UtcNow
        };
        context.Departments.Add(department);

        var employee = new Employee
        {
            Id = 1,
            FullName = "María García",
            Email = "maria@test.com",
            DocumentNumber = "999888",
            DocumentType = "CC",
            Position = "Sales Manager",
            DepartmentId = 1,
            IsActive = true,
            Status = EmployeeStatus.Activo,
            HireDate = DateTime.UtcNow,
            Salary = 8000000,
            CreatedAt = DateTime.UtcNow
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var employeeService = new EmployeeService(context);

        // Act
        var result = await employeeService.GetEmployeeByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("María García", result!.FullName);
        Assert.Equal("maria@test.com", result.Email);
        Assert.Equal("Ventas", result.DepartmentName);
    }

    /// <summary>
    /// Prueba unitaria 3: Verificar que GetEmployeeByIdAsync retorna null para empleado inexistente
    /// </summary>
    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsNullForNonExistentEmployee()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var employeeService = new EmployeeService(context);

        // Act
        var result = await employeeService.GetEmployeeByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Prueba unitaria 4: Verificar que SearchEmployeesAsync filtra por nombre correctamente
    /// </summary>
    [Fact]
    public async Task SearchEmployeesAsync_FiltersByNameCorrectly()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        var department = new Department
        {
            Id = 1,
            Name = "RRHH",
            Description = "Recursos Humanos",
            CreatedAt = DateTime.UtcNow
        };
        context.Departments.Add(department);

        context.Employees.AddRange(
            new Employee
            {
                FullName = "Carlos López",
                Email = "carlos@test.com",
                DocumentNumber = "111",
                DocumentType = "CC",
                Position = "Analista",
                DepartmentId = 1,
                IsActive = true,
                Status = EmployeeStatus.Activo,
                HireDate = DateTime.UtcNow,
                Salary = 3000000,
                CreatedAt = DateTime.UtcNow
            },
            new Employee
            {
                FullName = "Ana Martínez",
                Email = "ana@test.com",
                DocumentNumber = "222",
                DocumentType = "CC",
                Position = "Coordinador",
                DepartmentId = 1,
                IsActive = true,
                Status = EmployeeStatus.Activo,
                HireDate = DateTime.UtcNow,
                Salary = 4500000,
                CreatedAt = DateTime.UtcNow
            }
        );
        await context.SaveChangesAsync();

        var employeeService = new EmployeeService(context);

        // Act
        var result = await employeeService.SearchEmployeesAsync("Carlos", null, null);

        // Assert
        Assert.Single(result);
        Assert.Equal("Carlos López", result.First().FullName);
    }
}
