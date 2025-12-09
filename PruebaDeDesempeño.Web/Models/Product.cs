using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaDeDesempeño.Web.Models;

/// <summary>
/// Representa un producto/insumo de construcción en el sistema.
/// </summary>
public class Product
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El precio unitario es requerido")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    [Display(Name = "Precio Unitario")]
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

    [Display(Name = "Fecha de Creación")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Fecha de Actualización")]
    public DateTime? UpdatedAt { get; set; }

    // Navegación
    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
