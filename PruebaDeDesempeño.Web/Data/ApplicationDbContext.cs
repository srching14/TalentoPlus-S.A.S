using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PruebaDeDesempeño.Web.Models;

namespace PruebaDeDesempeño.Web.Data;

/// <summary>
/// Contexto de base de datos principal de la aplicación.
/// Hereda de IdentityDbContext para integrar ASP.NET Core Identity.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets para TalentoPlus
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.DocumentNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.FullName);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Salary).HasPrecision(18, 2);

            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ApplicationUser)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de Department
        modelBuilder.Entity<Department>(entity => { entity.HasIndex(e => e.Name); });

        // Seed de roles
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "1",
                Name = "Administrador",
                NormalizedName = "ADMINISTRADOR"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "Empleado",
                NormalizedName = "EMPLEADO"
            }
        );

        // Seed de departamentos
        modelBuilder.Entity<Department>().HasData(
            new Department
            {
                Id = 1, Name = "Recursos Humanos", Description = "Gestión del talento humano",
                CreatedAt = DateTime.UtcNow
            },
            new Department
            {
                Id = 2, Name = "Tecnología", Description = "Desarrollo y soporte tecnológico",
                CreatedAt = DateTime.UtcNow
            },
            new Department
                { Id = 3, Name = "Ventas", Description = "Gestión comercial y ventas", CreatedAt = DateTime.UtcNow },
            new Department
                { Id = 4, Name = "Marketing", Description = "Marketing y comunicaciones", CreatedAt = DateTime.UtcNow },
            new Department
            {
                Id = 5, Name = "Finanzas", Description = "Gestión financiera y contable", CreatedAt = DateTime.UtcNow
            },
            new Department
                { Id = 6, Name = "Operaciones", Description = "Operaciones y logística", CreatedAt = DateTime.UtcNow },
            new Department
                { Id = 7, Name = "Logística", Description = "Logística y distribución", CreatedAt = DateTime.UtcNow },
            new Department
                { Id = 8, Name = "Contabilidad", Description = "Contabilidad y auditoría", CreatedAt = DateTime.UtcNow }
        );

        // Seed de usuario administrador
        var hasher = new PasswordHasher<ApplicationUser>();
        var adminUser = new ApplicationUser
        {
            Id = "admin-user-id",
            UserName = "admin@talentoplusadmin.com",
            NormalizedUserName = "ADMIN@TALENTOPLUSADMIN.COM",
            Email = "admin@talentoplusadmin.com",
            NormalizedEmail = "ADMIN@TALENTOPLUSADMIN.COM",
            EmailConfirmed = true,
            FullName = "Administrador TalentoPlus",
            CreatedAt = DateTime.UtcNow,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

        modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

        // Asignar rol de administrador
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "1",
                UserId = "admin-user-id"
            }
        );
    }
}
