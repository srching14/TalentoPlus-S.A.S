using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaDeDesempeño.Web.Models;

/// <summary>
/// Representa un detalle/línea de una venta.
/// </summary>
public class SaleDetail
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "La venta es requerida")]
    [Display(Name = "Venta")]
    public int SaleId { get; set; }

    [Required(ErrorMessage = "El producto es requerido")]
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio Unitario")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Subtotal")]
    public decimal Subtotal { get; set; }

    // Navegación
    [ForeignKey("SaleId")]
    public virtual Sale? Sale { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
