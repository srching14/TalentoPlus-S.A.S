using Microsoft.AspNetCore.Identity;

namespace PruebaDeDesempeño.Web.Models;

/// <summary>
/// Usuario de la aplicación que extiende IdentityUser.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Relación con empleado (para auto-registro de empleados)
    public int? EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }
}
