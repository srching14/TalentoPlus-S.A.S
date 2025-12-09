using System.ComponentModel.DataAnnotations;

namespace PruebaDeDesempeño.Web.Models
{
    /// <summary>
    /// Representa un departamento de la empresa
    /// </summary>
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Indica si el registro está activo (soft delete)
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Relaciones
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
