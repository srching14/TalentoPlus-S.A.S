using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PruebaDeDesempeño.API.Controllers;

/// <summary>
/// API Controller para autoservicio de empleados
/// </summary>
[ApiController]
[Route("api/employees")]
public class EmployeeSelfServiceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<EmployeeSelfServiceController> _logger;

    public EmployeeSelfServiceController(
        ApplicationDbContext context,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<EmployeeSelfServiceController> logger)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }

    // ================== ENDPOINTS PÚBLICOS (SIN JWT) ==================

    /// <summary>
    /// Autoregistro de empleado (público) - Genera contraseña automática y la envía por email
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> SelfRegister([FromBody] EmployeeSelfRegisterDto dto)
    {
        try
        {
            // Validar que el documento no exista
            if (await _context.Employees.AnyAsync(e => e.DocumentNumber == dto.DocumentNumber))
            {
                return Conflict(new { success = false, message = "Ya existe un empleado con ese número de documento" });
            }

            // Validar que el email no exista
            if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
            {
                return Conflict(new { success = false, message = "Ya existe un empleado con ese email" });
            }

            // Validar que el departamento exista
            var department = await _context.Departments.FindAsync(dto.DepartmentId);
            if (department == null)
            {
                return BadRequest(new { success = false, message = "Departamento no encontrado" });
            }

            // Generar contraseña automática
            var generatedPassword = GenerateRandomPassword();
            var passwordHash = HashPassword(generatedPassword);

            // Crear el empleado
            var employee = new Employee
            {
                DocumentNumber = dto.DocumentNumber,
                DocumentType = dto.DocumentType ?? "CC",
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                Position = dto.Position ?? "Pendiente de asignación",
                Salary = 0, // Pendiente de asignación por admin
                HireDate = DateTime.UtcNow,
                Status = EmployeeStatus.Activo, // Activo para que pueda hacer login inmediatamente
                EducationLevel = dto.EducationLevel ?? EducationLevel.Bachillerato,
                ProfessionalProfile = dto.ProfessionalProfile,
                DepartmentId = dto.DepartmentId,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nuevo empleado autoregistrado: {FullName} ({Email})", employee.FullName, employee.Email);

            // Enviar email de bienvenida REAL con la contraseña
            try
            {
                var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%); padding: 30px; text-align: center; color: white; }}
        .content {{ padding: 30px; }}
        .credentials {{ background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #6366f1; }}
        .password {{ font-size: 24px; font-weight: bold; color: #6366f1; letter-spacing: 2px; }}
        .footer {{ background: #1e293b; color: white; padding: 20px; text-align: center; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Bienvenido(a) a TalentoPlus S.A.S.!</h1>
        </div>
        <div class='content'>
            <p>Estimado(a) <strong>{employee.FullName}</strong>,</p>
            <p>Tu registro en nuestra plataforma ha sido completado exitosamente.</p>
            
            <div class='credentials'>
                <h3>🔐 Tus credenciales de acceso:</h3>
                <p><strong>Documento:</strong> {employee.DocumentNumber}</p>
                <p><strong>Contraseña:</strong> <span class='password'>{generatedPassword}</span></p>
            </div>
            
            <p><strong>⚠️ Importante:</strong> Guarda esta contraseña en un lugar seguro. La necesitarás para autenticarte en la API.</p>
            
            <p><strong>Datos de tu registro:</strong></p>
            <ul>
                <li>Tipo de Documento: {employee.DocumentType}</li>
                <li>Email: {employee.Email}</li>
                <li>Departamento: {department.Name}</li>
            </ul>
            
            <p>Ya puedes autenticarte en la plataforma usando tu <strong>número de documento</strong> y la <strong>contraseña</strong> proporcionada.</p>
            
            <p>Gracias por unirte a nuestro equipo.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.Now.Year} TalentoPlus S.A.S. | Todos los derechos reservados</p>
        </div>
    </div>
</body>
</html>";
                    
                await _emailService.SendEmailAsync(
                    employee.Email,
                    "🎉 Bienvenido a TalentoPlus - Tus credenciales de acceso",
                    htmlBody);

                _logger.LogInformation("Email con contraseña enviado a: {Email}", employee.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email de bienvenida a {Email}", employee.Email);
                // Retornar la contraseña en la respuesta si el email falla
                return Ok(new
                {
                    success = true,
                    message = "Registro exitoso. NOTA: No se pudo enviar el email, guarda esta contraseña.",
                    employeeId = employee.Id,
                    temporaryPassword = generatedPassword // Solo si el email falla
                });
            }

            return Ok(new
            {
                success = true,
                message = "Registro exitoso. Se ha enviado un email con tus credenciales de acceso.",
                employeeId = employee.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en autoregistro de empleado");
            return StatusCode(500, new { success = false, message = "Error al procesar el registro" });
        }
    }

    /// <summary>
    /// Login de empleado (documento + contraseña) → JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> EmployeeLogin([FromBody] EmployeeLoginDto dto)
    {
        try
        {
            // Buscar empleado por documento
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => 
                    e.DocumentNumber == dto.DocumentNumber && 
                    e.IsActive);

            if (employee == null)
            {
                return Unauthorized(new { success = false, message = "Credenciales inválidas" });
            }

            // Verificar contraseña
            if (string.IsNullOrEmpty(employee.PasswordHash) || !VerifyPassword(dto.Password, employee.PasswordHash))
            {
                return Unauthorized(new { success = false, message = "Credenciales inválidas" });
            }

            // Verificar que esté activo
            if (employee.Status != EmployeeStatus.Activo)
            {
                return Unauthorized(new { success = false, message = "Tu cuenta no está activa. Contacta al administrador." });
            }

            // Generar JWT token
            var token = GenerateEmployeeJwtToken(employee);

            _logger.LogInformation("Empleado autenticado: {FullName} ({Email})", employee.FullName, employee.Email);

            return Ok(new
            {
                success = true,
                message = "Login exitoso",
                token,
                employee = new
                {
                    id = employee.Id,
                    fullName = employee.FullName,
                    email = employee.Email,
                    department = employee.Department?.Name
                },
                expiration = DateTime.UtcNow.AddHours(24)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login de empleado");
            return StatusCode(500, new { success = false, message = "Error al iniciar sesión" });
        }
    }

    // ================== ENDPOINTS PROTEGIDOS (REQUIEREN JWT) ==================

    /// <summary>
    /// Consultar información propia del empleado (desde JWT)
    /// </summary>
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetMyInfo()
    {
        try
        {
            var employeeId = GetEmployeeIdFromToken();
            if (employeeId == null)
            {
                return Unauthorized(new { success = false, message = "Token inválido" });
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeId && e.IsActive);

            if (employee == null)
            {
                return NotFound(new { success = false, message = "Empleado no encontrado" });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = employee.Id,
                    documentType = employee.DocumentType,
                    documentNumber = employee.DocumentNumber,
                    fullName = employee.FullName,
                    email = employee.Email,
                    phone = employee.Phone,
                    address = employee.Address,
                    birthDate = employee.BirthDate,
                    gender = employee.Gender,
                    position = employee.Position,
                    salary = employee.Salary,
                    status = employee.Status.ToString(),
                    educationLevel = employee.EducationLevel.ToString(),
                    professionalProfile = employee.ProfessionalProfile,
                    department = new
                    {
                        id = employee.Department?.Id,
                        name = employee.Department?.Name
                    },
                    hireDate = employee.HireDate
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo información del empleado");
            return StatusCode(500, new { success = false, message = "Error al obtener información" });
        }
    }

    /// <summary>
    /// Descargar hoja de vida en PDF (desde JWT)
    /// </summary>
    [HttpGet("me/cv")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> DownloadMyCv()
    {
        try
        {
            var employeeId = GetEmployeeIdFromToken();
            if (employeeId == null)
            {
                return Unauthorized(new { success = false, message = "Token inválido" });
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeId && e.IsActive);

            if (employee == null)
            {
                return NotFound(new { success = false, message = "Empleado no encontrado" });
            }

            // Generar PDF
            var pdfBytes = GenerateEmployeeCvPdf(employee);

            return File(pdfBytes, "application/pdf", $"CV_{employee.FullName.Replace(" ", "_")}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error descargando CV del empleado");
            return StatusCode(500, new { success = false, message = "Error al generar el PDF" });
        }
    }

    // ================== MÉTODOS PRIVADOS ==================

    private static string GenerateRandomPassword(int length = 10)
    {
        const string upperCase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lowerCase = "abcdefghjkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%";
        
        var random = new Random();
        var password = new StringBuilder();
        
        // Asegurar al menos uno de cada tipo
        password.Append(upperCase[random.Next(upperCase.Length)]);
        password.Append(lowerCase[random.Next(lowerCase.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(special[random.Next(special.Length)]);
        
        // Completar el resto
        const string allChars = upperCase + lowerCase + digits;
        for (int i = 4; i < length; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }
        
        // Mezclar
        return new string(password.ToString().ToCharArray().OrderBy(_ => random.Next()).ToArray());
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }

    private string GenerateEmployeeJwtToken(Employee employee)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "SuperSecretKeyForJWTAuthenticationPruebaDeDesempeno2024!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.Name, employee.FullName),
            new Claim("employeeId", employee.Id.ToString()),
            new Claim("documentNumber", employee.DocumentNumber),
            new Claim(ClaimTypes.Role, "Empleado")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "PruebaDeDesempenoAPI",
            audience: _configuration["Jwt:Audience"] ?? "PruebaDeDesempenoClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int? GetEmployeeIdFromToken()
    {
        var employeeIdClaim = User.FindFirst("employeeId")?.Value;
        if (string.IsNullOrEmpty(employeeIdClaim))
        {
            employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        if (int.TryParse(employeeIdClaim, out int employeeId))
        {
            return employeeId;
        }

        return null;
    }

    private byte[] GenerateEmployeeCvPdf(Employee employee)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("HOJA DE VIDA").Bold().FontSize(24).FontColor(Colors.Blue.Darken2);
                        col.Item().Text("TalentoPlus S.A.S.").FontSize(12).FontColor(Colors.Grey.Darken1);
                    });
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    // Información Personal
                    col.Item().Background(Colors.Blue.Darken2).Padding(5).Text("INFORMACIÓN PERSONAL").Bold().FontColor(Colors.White);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                        });

                        AddTableRow(table, "Nombre Completo:", employee.FullName);
                        AddTableRow(table, "Documento:", $"{employee.DocumentType} {employee.DocumentNumber}");
                        AddTableRow(table, "Email:", employee.Email);
                        AddTableRow(table, "Teléfono:", employee.Phone ?? "No especificado");
                        AddTableRow(table, "Dirección:", employee.Address ?? "No especificada");
                        AddTableRow(table, "Fecha de Nacimiento:", employee.BirthDate?.ToString("dd/MM/yyyy") ?? "No especificada");
                        AddTableRow(table, "Género:", employee.Gender ?? "No especificado");
                    });

                    col.Item().PaddingTop(15).Background(Colors.Blue.Darken2).Padding(5).Text("INFORMACIÓN LABORAL").Bold().FontColor(Colors.White);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                        });

                        AddTableRow(table, "Cargo:", employee.Position);
                        AddTableRow(table, "Departamento:", employee.Department?.Name ?? "No asignado");
                        AddTableRow(table, "Fecha de Ingreso:", employee.HireDate.ToString("dd/MM/yyyy"));
                        AddTableRow(table, "Estado:", employee.Status.ToString());
                        AddTableRow(table, "Nivel Educativo:", employee.EducationLevel.ToString());
                    });

                    if (!string.IsNullOrEmpty(employee.ProfessionalProfile))
                    {
                        col.Item().PaddingTop(15).Background(Colors.Blue.Darken2).Padding(5).Text("PERFIL PROFESIONAL").Bold().FontColor(Colors.White);
                        col.Item().Padding(5).Text(employee.ProfessionalProfile).FontSize(10);
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generado el ").FontSize(9);
                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).Bold();
                    text.Span(" - TalentoPlus S.A.S.").FontSize(9);
                });
            });
        });

        return document.GeneratePdf();
    }

    private void AddTableRow(TableDescriptor table, string label, string value)
    {
        table.Cell().Padding(3).Text(label).Bold();
        table.Cell().Padding(3).Text(value);
    }
}

// DTOs
public class EmployeeSelfRegisterDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Position { get; set; }
    public EducationLevel? EducationLevel { get; set; }
    public string? ProfessionalProfile { get; set; }
    public int DepartmentId { get; set; }
}

public class EmployeeLoginDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
