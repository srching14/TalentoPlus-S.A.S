using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaDeDesempeño.Web.Models
{
    /// <summary>
    /// Representa un empleado de TalentoPlus S.A.S.
    /// </summary>
    public class Employee
    {
        [Key] public int Id { get; set; }

        // Información Personal
        [Required(ErrorMessage = "El número de documento es requerido")]
        [StringLength(20, ErrorMessage = "El número de documento no puede exceder 20 caracteres")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [StringLength(10)]
        public string DocumentType { get; set; } = "CC"; // CC, CE, Pasaporte

        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(200, ErrorMessage = "El nombre completo no puede exceder 200 caracteres")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El teléfono no es válido")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(300)] public string? Address { get; set; }

        [DataType(DataType.Date)] public DateTime? BirthDate { get; set; }

        [StringLength(20)] public string? Gender { get; set; } // Masculino, Femenino, Otro

        // Información Laboral
        [Required(ErrorMessage = "El cargo es requerido")]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")] public decimal Salary { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es requerida")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        [Required] public EmployeeStatus Status { get; set; } = EmployeeStatus.Activo;

        // Información Académica y Profesional
        [Required] public EducationLevel EducationLevel { get; set; } = EducationLevel.Bachillerato;

        [StringLength(1000)] public string? ProfessionalProfile { get; set; }

        // Relación con Departamento
        [Required(ErrorMessage = "El departamento es requerido")]
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))] public virtual Department? Department { get; set; }

        // Contraseña para API (hash)
        public string? PasswordHash { get; set; }

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Relación con Usuario (para auto-registro)
        public string? ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
