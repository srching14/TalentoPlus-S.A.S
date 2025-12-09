using System.ComponentModel.DataAnnotations;

namespace PruebaDeDesempeño.Web.ViewModels;

public class ClientViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(300, ErrorMessage = "El nombre no puede exceder 300 caracteres")]
    [Display(Name = "Nombre Completo")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de documento es requerido")]
    [StringLength(20, ErrorMessage = "El tipo de documento no puede exceder 20 caracteres")]
    [Display(Name = "Tipo de Documento")]
    public string DocumentType { get; set; } = "CC";

    [Required(ErrorMessage = "El número de documento es requerido")]
    [StringLength(50, ErrorMessage = "El número de documento no puede exceder 50 caracteres")]
    [Display(Name = "Número de Documento")]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(256, ErrorMessage = "El correo no puede exceder 256 caracteres")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "El número de teléfono no es válido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    [Display(Name = "Teléfono")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La dirección no puede exceder 500 caracteres")]
    [Display(Name = "Dirección")]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres")]
    [Display(Name = "Ciudad")]
    public string? City { get; set; }

    [Display(Name = "Activo")]
    public bool IsActive { get; set; } = true;
}
