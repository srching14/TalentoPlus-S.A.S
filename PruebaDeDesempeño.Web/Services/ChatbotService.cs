using PruebaDeDesempeño.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using PruebaDeDesempeño.Web.Models;

namespace PruebaDeDesempeño.Web.Services;

public interface IChatbotService
{
    Task<string> ProcessMessageAsync(string message);
}

public class ChatbotService : IChatbotService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatbotService> _logger;
    private readonly GeminiService? _geminiService;

    public ChatbotService(
        ApplicationDbContext context,
        ILogger<ChatbotService> logger,
        GeminiService? geminiService = null)
    {
        _context = context;
        _logger = logger;
        _geminiService = geminiService;
    }

    public async Task<string> ProcessMessageAsync(string message)
    {
        try
        {
            _logger.LogInformation("Procesando pregunta: {Message}", message);
            var lowerMessage = message.ToLower().Trim();

            // Primero intentar con patrones locales (siempre funciona)
            var response = await ProcessWithPatternMatchingAsync(lowerMessage);

            if (response != null)
            {
                return response;
            }

            // Si no hay match local y Gemini está disponible, intentar con IA
            if (_geminiService != null)
            {
                try
                {
                    var intent = await _geminiService.InterpretQueryAsync(message);
                    if (intent.Intent != "unknown")
                    {
                        return await ProcessIntentAsync(intent);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Gemini no disponible, usando fallback");
                }
            }

            return GetHelpMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando mensaje: {Message}", message);
            return "❌ Ocurrió un error al procesar tu consulta. Por favor intenta nuevamente.";
        }
    }

    private async Task<string?> ProcessWithPatternMatchingAsync(string message)
    {
        // ============ CONSULTAS POR CARGO ============
        if (message.Contains("auxiliar"))
        {
            var count = await _context.Employees
                .Where(e => e.IsActive && e.Position.ToLower().Contains("auxiliar"))
                .CountAsync();
            return $"📋 Hay **{count} auxiliares** en la plataforma.";
        }

        if (message.Contains("desarrollador") || message.Contains("developer"))
        {
            var count = await _context.Employees
                .Where(e => e.IsActive && (e.Position.ToLower().Contains("desarrollador") ||
                                           e.Position.ToLower().Contains("developer")))
                .CountAsync();
            return $"💻 Hay **{count} desarrolladores** en la plataforma.";
        }

        if (message.Contains("gerente") || message.Contains("manager"))
        {
            var count = await _context.Employees
                .Where(e => e.IsActive &&
                            (e.Position.ToLower().Contains("gerente") || e.Position.ToLower().Contains("manager")))
                .CountAsync();
            return $"👔 Hay **{count} gerentes** en la plataforma.";
        }

        if (message.Contains("analista"))
        {
            var count = await _context.Employees
                .Where(e => e.IsActive && e.Position.ToLower().Contains("analista"))
                .CountAsync();
            return $"📊 Hay **{count} analistas** en la plataforma.";
        }

        // ============ CONSULTAS POR DEPARTAMENTO ============
        if (message.Contains("tecnología") || message.Contains("tecnologia") || message.Contains("ti") ||
            message.Contains("sistemas"))
        {
            return await GetEmployeesByDepartmentAsync("Tecnología");
        }

        if (message.Contains("recursos humanos") || message.Contains("rrhh"))
        {
            return await GetEmployeesByDepartmentAsync("Recursos Humanos");
        }

        if (message.Contains("ventas") || message.Contains("comercial"))
        {
            return await GetEmployeesByDepartmentAsync("Ventas");
        }

        if (message.Contains("marketing"))
        {
            return await GetEmployeesByDepartmentAsync("Marketing");
        }

        if (message.Contains("finanzas") || message.Contains("contabilidad"))
        {
            return await GetEmployeesByDepartmentAsync("Finanzas");
        }

        if (message.Contains("operaciones") || message.Contains("logística") || message.Contains("logistica"))
        {
            return await GetEmployeesByDepartmentAsync("Operaciones");
        }

        // ============ CONSULTAS POR ESTADO ============
        if (message.Contains("inactivo"))
        {
            var count = await _context.Employees
                .Where(e => e.IsActive && e.Status == EmployeeStatus.Inactivo)
                .CountAsync();
            return $"⏸️ Hay **{count} empleados** en estado **inactivo**.";
        }

        if (message.Contains("vacaciones"))
        {
            var count = await _context.Employees
                .Where(e => e.IsActive && e.Status == EmployeeStatus.Vacaciones)
                .CountAsync();
            return $"🏖️ Hay **{count} empleados** de vacaciones.";
        }

        if (message.Contains("licencia"))
        {
            var count = await _context.Employees
                .Where(e => e.IsActive && e.Status == EmployeeStatus.Licencia)
                .CountAsync();
            return $"📋 Hay **{count} empleados** en licencia.";
        }

        if (message.Contains("activo") && !message.Contains("inactivo"))
        {
            var count = await _context.Employees
                .Where(e => e.IsActive && e.Status == EmployeeStatus.Activo)
                .CountAsync();
            return $"✅ Hay **{count} empleados** en estado activo.";
        }

        // ============ CONSULTAS SOBRE SALARIOS ============
        if (message.Contains("salario") || message.Contains("sueldo"))
        {
            var employees = await _context.Employees.Where(e => e.IsActive).ToListAsync();
            if (!employees.Any()) return "📊 No hay empleados registrados.";

            var avg = employees.Average(e => e.Salary);
            var max = employees.Max(e => e.Salary);
            var min = employees.Min(e => e.Salary);

            return $"💰 **Estadísticas Salariales:**\n\n" +
                   $"- Promedio: **${avg:N0} COP**\n" +
                   $"- Máximo: **${max:N0} COP**\n" +
                   $"- Mínimo: **${min:N0} COP**";
        }

        // ============ ESTADÍSTICAS GENERALES ============
        if (message.Contains("cuántos empleados") || message.Contains("cuantos empleados") ||
            message.Contains("total de empleados") || message.Contains("estadísticas"))
        {
            return await GetGeneralStatsAsync();
        }

        // ============ EMPLEADOS POR DEPARTAMENTO ============
        if (message.Contains("departamento") || message.Contains("por área"))
        {
            return await GetAllDepartmentsStatsAsync();
        }

        // ============ CONTRATACIONES RECIENTES ============
        if (message.Contains("nuevos") || message.Contains("reciente") || message.Contains("contrataron"))
        {
            return await GetRecentHiresAsync();
        }

        return null; // No match found
    }

    private async Task<string> GetEmployeesByDepartmentAsync(string departmentName)
    {
        var count = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive && e.Department!.Name.ToLower().Contains(departmentName.ToLower()))
            .CountAsync();

        if (count == 0)
        {
            return $"🔍 No se encontraron empleados en el departamento de {departmentName}.";
        }

        return $"🏢 Hay **{count} empleados** en el departamento de **{departmentName}**.";
    }

    private async Task<string> GetGeneralStatsAsync()
    {
        var total = await _context.Employees.Where(e => e.IsActive).CountAsync();
        var active = await _context.Employees.Where(e => e.IsActive && e.Status == EmployeeStatus.Activo).CountAsync();
        var vacation = await _context.Employees.Where(e => e.IsActive && e.Status == EmployeeStatus.Vacaciones)
            .CountAsync();
        var inactive = await _context.Employees.Where(e => e.IsActive && e.Status == EmployeeStatus.Inactivo)
            .CountAsync();

        return $"📊 **Estadísticas Generales:**\n\n" +
               $"- Total de empleados: **{total}**\n" +
               $"- Empleados activos: **{active}**\n" +
               $"- De vacaciones: **{vacation}**\n" +
               $"- Inactivos: **{inactive}**";
    }

    private async Task<string> GetAllDepartmentsStatsAsync()
    {
        var departments = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .GroupBy(e => e.Department!.Name)
            .Select(g => new { Department = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        var sb = new StringBuilder("🏢 **Empleados por departamento:**\n\n");
        foreach (var dept in departments)
        {
            sb.AppendLine($"- **{dept.Department}**: {dept.Count}");
        }

        return sb.ToString();
    }

    private async Task<string> GetRecentHiresAsync()
    {
        var recentEmployees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.HireDate)
            .Take(5)
            .ToListAsync();

        if (!recentEmployees.Any())
        {
            return "🆕 No hay empleados registrados.";
        }

        var sb = new StringBuilder($"🆕 **Últimos 5 empleados contratados:**\n\n");
        foreach (var emp in recentEmployees)
        {
            sb.AppendLine($"- **{emp.FullName}** - {emp.Position}");
            sb.AppendLine($"  {emp.Department?.Name} | {emp.HireDate:dd/MM/yyyy}\n");
        }

        return sb.ToString();
    }

    private async Task<string> ProcessIntentAsync(QueryIntent intent)
    {
        return intent.Intent switch
        {
            "count_by_position" => await CountByPositionAsync(intent.Parameters.Position),
            "count_by_department" => await GetEmployeesByDepartmentAsync(intent.Parameters.Department ?? ""),
            "count_by_status" => await CountByStatusAsync(intent.Parameters.Status),
            "salary_stats" => await GetSalaryStatsAsync(),
            "recent_hires" => await GetRecentHiresAsync(),
            "general_stats" => await GetGeneralStatsAsync(),
            _ => GetHelpMessage()
        };
    }

    private async Task<string> CountByPositionAsync(string? position)
    {
        if (string.IsNullOrWhiteSpace(position)) return await GetGeneralStatsAsync();

        var count = await _context.Employees
            .Where(e => e.IsActive && e.Position.ToLower().Contains(position.ToLower()))
            .CountAsync();

        return $"📋 Hay **{count}** empleados con cargo relacionado a '{position}'.";
    }

    private async Task<string> CountByStatusAsync(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return await GetGeneralStatsAsync();

        var employeeStatus = status.ToLower() switch
        {
            "activo" => EmployeeStatus.Activo,
            "inactivo" => EmployeeStatus.Inactivo,
            "vacaciones" => EmployeeStatus.Vacaciones,
            "licencia" => EmployeeStatus.Licencia,
            _ => (EmployeeStatus?)null
        };

        if (employeeStatus == null) return $"🔍 Estado '{status}' no reconocido.";

        var count = await _context.Employees
            .Where(e => e.IsActive && e.Status == employeeStatus)
            .CountAsync();

        return $"📊 Hay **{count} empleados** en estado **{employeeStatus}**.";
    }

    private async Task<string> GetSalaryStatsAsync()
    {
        var employees = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        if (!employees.Any()) return "📊 No hay empleados registrados.";

        return $"💰 **Estadísticas Salariales:**\n\n" +
               $"- Promedio: **${employees.Average(e => e.Salary):N0} COP**\n" +
               $"- Máximo: **${employees.Max(e => e.Salary):N0} COP**\n" +
               $"- Mínimo: **${employees.Min(e => e.Salary):N0} COP**";
    }

    private string GetHelpMessage()
    {
        return "🤔 No entendí tu consulta. Intenta preguntar:\n\n" +
               "**Por cargo:**\n" +
               "- ¿Cuántos auxiliares hay?\n" +
               "- ¿Cuántos desarrolladores hay?\n\n" +
               "**Por departamento:**\n" +
               "- ¿Cuántos empleados hay en Tecnología?\n" +
               "- ¿Cuántos empleados hay en Ventas?\n\n" +
               "**Por estado:**\n" +
               "- ¿Cuántos empleados están inactivos?\n" +
               "- ¿Cuántos están de vacaciones?\n\n" +
               "**Otros:**\n" +
               "- ¿Cuál es el salario promedio?\n" +
               "- ¿Cuántos empleados hay?";
    }
}
