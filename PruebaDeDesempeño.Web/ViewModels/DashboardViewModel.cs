namespace PruebaDeDesempeño.Web.ViewModels;

public class DashboardViewModel
{
    // KPIs principales
    public int TotalEmployees { get; set; }
    public int EmployeesOnVacation { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }

    // Estadísticas por departamento
    public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();

    // Empleados recientes
    public List<RecentEmployeeViewModel> RecentEmployees { get; set; } = new();

    // Estadísticas adicionales
    public decimal AverageSalary { get; set; }
    public int NewEmployeesThisMonth { get; set; }
}

public class RecentEmployeeViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

