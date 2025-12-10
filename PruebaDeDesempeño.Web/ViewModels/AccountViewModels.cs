using System.ComponentModel.DataAnnotations;

namespace PruebaDeDesempeño.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")] public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(300, ErrorMessage = "El nombre no puede exceder 300 caracteres")]
    [Display(Name = "Nombre Completo")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;
}
