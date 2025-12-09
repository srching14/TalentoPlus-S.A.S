using System.ComponentModel.DataAnnotations;

namespace PruebaDeDesempeño.Web.ViewModels;

/// <summary>
/// ViewModel para edición de perfil del cliente.
/// </summary>
public class ClientProfileViewModel
{
    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(300, ErrorMessage = "El nombre no puede exceder 300 caracteres")]
    [Display(Name = "Nombre Completo")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Contraseña Actual")]
    public string? CurrentPassword { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Nueva Contraseña")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string? ConfirmNewPassword { get; set; }

    // Read-only properties for display
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
