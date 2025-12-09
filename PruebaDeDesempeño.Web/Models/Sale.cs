using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaDeDesempeño.Web.Models;

/// <summary>
/// Representa una venta en el sistema.
/// </summary>
public class Sale
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El número de venta es requerido")]
    [StringLength(50, ErrorMessage = "El número de venta no puede exceder 50 caracteres")]
    [Display(Name = "Número de Venta")]
    public string SaleNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cliente es requerido")]
    [Display(Name = "Cliente")]
    public int ClientId { get; set; }

    [Display(Name = "Fecha de Venta")]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Subtotal")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "IVA")]
    public decimal IVA { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Total")]
    public decimal Total { get; set; }

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    [StringLength(500)]
    [Display(Name = "Ruta del Recibo PDF")]
    public string? PdfReceiptPath { get; set; }

    [Display(Name = "Estado")]
    public SaleStatus Status { get; set; } = SaleStatus.Completed;

    [Display(Name = "Fecha de Creación")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}

public enum SaleStatus
{
    [Display(Name = "Pendiente")]
    Pending = 0,

    [Display(Name = "Completada")]
    Completed = 1,

    [Display(Name = "Cancelada")]
    Cancelled = 2
}
