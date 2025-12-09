using System.ComponentModel.DataAnnotations;
using PruebaDeDesempeño.Web.Models;

namespace PruebaDeDesempeño.Web.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de documento es requerido")]
        [Display(Name = "Número de Documento")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [Display(Name = "Tipo de Documento")]
        public string DocumentType { get; set; } = "CC";

        [Required(ErrorMessage = "El nombre completo es requerido")]
        [Display(Name = "Nombre Completo")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Teléfono inválido")]
        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Género")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "El cargo es requerido")]
        [Display(Name = "Cargo")]
        public string Position { get; set; } = string.Empty;

        [Required(ErrorMessage = "El salario es requerido")]
        [Display(Name = "Salario")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Ingreso")]
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Estado")]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Activo;

        [Required]
        [Display(Name = "Nivel Educativo")]
        public EducationLevel EducationLevel { get; set; } = EducationLevel.Bachillerato;

        [Display(Name = "Perfil Profesional")]
        [DataType(DataType.MultilineText)]
        public string? ProfessionalProfile { get; set; }

        [Required(ErrorMessage = "El departamento es requerido")]
        [Display(Name = "Departamento")]
        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
    }

    public class EmployeeListViewModel
    {
        public List<EmployeeViewModel> Employees { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int? DepartmentFilter { get; set; }
        public EmployeeStatus? StatusFilter { get; set; }
        public List<Department> Departments { get; set; } = new();
    }

    public class ImportEmployeesViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un archivo Excel")]
        [Display(Name = "Archivo Excel")]
        public IFormFile ExcelFile { get; set; } = null!;
    }
}
