using System.ComponentModel.DataAnnotations;

namespace PruebaDeDesempeño.Web.ViewModels;

public class SaleViewModel
{
    public int Id { get; set; }

    [Display(Name = "Número de Venta")]
    public string? SaleNumber { get; set; }

    [Required(ErrorMessage = "El cliente es requerido")]
    [Display(Name = "Cliente")]
    public int ClientId { get; set; }

    [Display(Name = "Nombre del Cliente")]
    public string? ClientName { get; set; }

    [Display(Name = "Fecha de Venta")]
    public DateTime SaleDate { get; set; } = DateTime.Now;

    [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    [Display(Name = "Subtotal")]
    public decimal Subtotal { get; set; }

    [Display(Name = "IVA (19%)")]
    public decimal IVA { get; set; }

    [Display(Name = "Total")]
    public decimal Total { get; set; }

    [Display(Name = "Ruta del Recibo PDF")]
    public string? PdfReceiptPath { get; set; }

    public List<SaleDetailViewModel> Details { get; set; } = new();
}

public class SaleDetailViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El producto es requerido")]
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Display(Name = "Nombre del Producto")]
    public string? ProductName { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; } = 1;

    [Display(Name = "Precio Unitario")]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Subtotal")]
    public decimal Subtotal { get; set; }
}

public class CreateSaleViewModel
{
    [Required(ErrorMessage = "El cliente es requerido")]
    [Display(Name = "Cliente")]
    public int ClientId { get; set; }

    [StringLength(500)]
    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    public List<CreateSaleDetailViewModel> Details { get; set; } = new();
}

public class CreateSaleDetailViewModel
{
    [Required(ErrorMessage = "El producto es requerido")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Quantity { get; set; } = 1;
}
