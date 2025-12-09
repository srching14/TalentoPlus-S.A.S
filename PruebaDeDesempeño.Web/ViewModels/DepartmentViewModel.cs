using System.ComponentModel.DataAnnotations;

namespace PruebaDeDesempeño.Web.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Número de Empleados")]
        public int EmployeeCount { get; set; }
    }
}
