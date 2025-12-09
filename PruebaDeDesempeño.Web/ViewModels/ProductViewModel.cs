using System.ComponentModel.DataAnnotations;

namespace PruebaDeDesempeño.Web.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El precio unitario es requerido")]
    [Range(0.01, 999999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    [Display(Name = "Precio Unitario")]
    [DataType(DataType.Currency)]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage = "El stock es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es requerida")]
    [StringLength(100, ErrorMessage = "La categoría no puede exceder 100 caracteres")]
    [Display(Name = "Categoría")]
    public string Category { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La unidad no puede exceder 50 caracteres")]
    [Display(Name = "Unidad de Medida")]
    public string? UnitOfMeasure { get; set; }

    [Display(Name = "Activo")]
    public bool IsActive { get; set; } = true;
}
