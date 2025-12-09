using Microsoft.EntityFrameworkCore;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.ViewModels;

namespace PruebaDeDesempeño.Web.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeViewModel>> GetAllEmployeesAsync();
        Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id);
        Task<Employee?> GetEmployeeEntityByIdAsync(int id);
        Task<Employee?> GetEmployeeByDocumentAsync(string documentNumber);
        Task<Employee?> GetEmployeeByEmailAsync(string email);
        Task<bool> CreateEmployeeAsync(EmployeeViewModel model);
        Task<bool> UpdateEmployeeAsync(EmployeeViewModel model);
        Task<bool> DeleteEmployeeAsync(int id);

        Task<List<EmployeeViewModel>> SearchEmployeesAsync(string? searchTerm, int? departmentId,
            EmployeeStatus? status);

        Task<DashboardViewModel> GetDashboardDataAsync();
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeViewModel>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.IsActive)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new EmployeeViewModel
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
                    HireDate = e.HireDate,
                    Status = e.Status,
                    EducationLevel = e.EducationLevel,
                    ProfessionalProfile = e.ProfessionalProfile,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department!.Name
                })
                .ToListAsync();
        }

        public async Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.Id == id && e.IsActive)
                .Select(e => new EmployeeViewModel
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
                    HireDate = e.HireDate,
                    Status = e.Status,
                    EducationLevel = e.EducationLevel,
                    ProfessionalProfile = e.ProfessionalProfile,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department!.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Employee?> GetEmployeeEntityByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        public async Task<Employee?> GetEmployeeByDocumentAsync(string documentNumber)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.DocumentNumber == documentNumber && e.IsActive);
        }

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Email == email && e.IsActive);
        }

        public async Task<bool> CreateEmployeeAsync(EmployeeViewModel model)
        {
            try
            {
                var employee = new Employee
                {
                    DocumentNumber = model.DocumentNumber,
                    DocumentType = model.DocumentType,
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    BirthDate = model.BirthDate,
                    Gender = model.Gender,
                    Position = model.Position,
                    Salary = model.Salary,
                    HireDate = model.HireDate,
                    Status = model.Status,
                    EducationLevel = model.EducationLevel,
                    ProfessionalProfile = model.ProfessionalProfile,
                    DepartmentId = model.DepartmentId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeViewModel model)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(model.Id);
                if (employee == null || !employee.IsActive)
                    return false;

                employee.DocumentNumber = model.DocumentNumber;
                employee.DocumentType = model.DocumentType;
                employee.FullName = model.FullName;
                employee.Email = model.Email;
                employee.Phone = model.Phone;
                employee.Address = model.Address;
                employee.BirthDate = model.BirthDate;
                employee.Gender = model.Gender;
                employee.Position = model.Position;
                employee.Salary = model.Salary;
                employee.HireDate = model.HireDate;
                employee.Status = model.Status;
                employee.EducationLevel = model.EducationLevel;
                employee.ProfessionalProfile = model.ProfessionalProfile;
                employee.DepartmentId = model.DepartmentId;
                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                    return false;

                // Soft delete
                employee.IsActive = false;
                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<EmployeeViewModel>> SearchEmployeesAsync(string? searchTerm, int? departmentId,
            EmployeeStatus? status)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Where(e => e.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.FullName.Contains(searchTerm) ||
                    e.DocumentNumber.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm) ||
                    e.Position.Contains(searchTerm));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            return await query
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new EmployeeViewModel()
                {
                    Id = e.Id,
                    DocumentNumber = e.DocumentNumber,
                    DocumentType = e.DocumentType,
                    FullName = e.FullName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Position = e.Position,
                    Salary = e.Salary,
                    HireDate = e.HireDate,
                    Status = e.Status,
                    EducationLevel = e.EducationLevel,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department!.Name
                })
                .ToListAsync();
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.IsActive)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

            var viewModel = new DashboardViewModel
            {
                TotalEmployees = employees.Count,
                EmployeesOnVacation = employees.Count(e => e.Status == EmployeeStatus.Vacaciones),
                ActiveEmployees = employees.Count(e => e.Status == EmployeeStatus.Activo),
                InactiveEmployees = employees.Count(e => e.Status == EmployeeStatus.Inactivo),
                AverageSalary = employees.Any() ? employees.Average(e => e.Salary) : 0,
                NewEmployeesThisMonth = employees.Count(e => e.HireDate >= firstDayOfMonth),
                EmployeesByDepartment = employees
                    .GroupBy(e => e.Department!.Name)
                    .ToDictionary(g => g.Key, g => g.Count()),
                RecentEmployees = employees
                    .OrderByDescending(e => e.HireDate)
                    .Take(5)
                    .Select(e => new RecentEmployeeViewModel
                    {
                        Id = e.Id,
                        FullName = e.FullName,
                        Position = e.Position,
                        DepartmentName = e.Department!.Name,
                        HireDate = e.HireDate,
                        Status = e.Status.ToString()
                    })
                    .ToList()
            };

            return viewModel;
        }
    }
}
