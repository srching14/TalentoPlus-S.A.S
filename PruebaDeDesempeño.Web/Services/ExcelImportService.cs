using OfficeOpenXml;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace PruebaDeDesempeño.Web.Services;

public interface IExcelImportService
{
    Task<ImportResult> ImportEmployeesFromExcelAsync(Stream fileStream, string fileName);
}

public class ImportResult
{
    public bool Success { get; set; }
    public int EmployeesImported { get; set; }
    public int EmployeesUpdated { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class ExcelImportService : IExcelImportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExcelImportService> _logger;

    public ExcelImportService(ApplicationDbContext context, ILogger<ExcelImportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImportResult> ImportEmployeesFromExcelAsync(Stream fileStream, string fileName)
    {
        var result = new ImportResult();

        try
        {
            using var package = new ExcelPackage(fileStream);

            // Tomar la primera hoja o buscar una hoja llamada "Empleados"
            var worksheet = package.Workbook.Worksheets.FirstOrDefault(w =>
                                w.Name.ToLower().Contains("empleado") ||
                                w.Name.ToLower().Contains("employee"))
                            ?? package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                result.Errors.Add("No se encontró ninguna hoja en el archivo Excel.");
                result.Success = false;
                return result;
            }

            await ImportEmployeesAsync(worksheet, result);

            if (result.Errors.Count == 0)
            {
                await _context.SaveChangesAsync();
                result.Success = true;
            }
            else
            {
                result.Success = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar archivo Excel: {FileName}", fileName);
            result.Errors.Add($"Error general: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    private async Task ImportEmployeesAsync(ExcelWorksheet worksheet, ImportResult result)
    {
        var rowCount = worksheet.Dimension?.Rows ?? 0;
        if (rowCount < 2)
        {
            result.Errors.Add("El archivo Excel está vacío o no tiene datos.");
            return;
        }

        // Detectar columnas por encabezados
        var headers = GetHeaders(worksheet);

        // Validar que existan columnas mínimas requeridas
        if (!ValidateRequiredHeaders(headers, result))
        {
            return;
        }

        // Cargar departamentos en memoria para búsqueda rápida
        var departments = await _context.Departments
            .Where(d => d.IsActive)
            .ToDictionaryAsync(d => d.Name.ToLower(), d => d);

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                // Campos requeridos
                var documentNumber = GetCellValue(worksheet, row, headers, "documento", "numerodocumento",
                    "documentnumber", "cedula");
                if (string.IsNullOrWhiteSpace(documentNumber))
                {
                    result.Warnings.Add($"Fila {row}: Número de documento vacío, omitiendo.");
                    continue;
                }

                // Nombre completo: combinar Nombres + Apellidos si están separados, o usar NombreCompleto
                var fullName = GetCellValue(worksheet, row, headers, "nombre", "nombrecompleto", "fullname", "name");
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    // Intentar combinar Nombres + Apellidos
                    var nombres = GetCellValue(worksheet, row, headers, "nombres", "primernombre", "firstname");
                    var apellidos = GetCellValue(worksheet, row, headers, "apellidos", "apellido", "lastname",
                        "surname");

                    if (!string.IsNullOrWhiteSpace(nombres))
                    {
                        fullName = string.IsNullOrWhiteSpace(apellidos)
                            ? nombres
                            : $"{nombres} {apellidos}";
                    }
                }

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    result.Errors.Add($"Fila {row}: Nombre completo requerido para documento '{documentNumber}'.");
                    continue;
                }

                // Email: opcional, generar si no existe
                var email = GetCellValue(worksheet, row, headers, "email", "correo", "correoelectronico");
                if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                {
                    // Generar email a partir del nombre
                    var cleanName = fullName.ToLower()
                        .Replace(" ", ".")
                        .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                        .Replace("ñ", "n");
                    email = $"{cleanName}@talentoplusadmin.com";
                }

                var position = GetCellValue(worksheet, row, headers, "cargo", "position", "puesto");
                if (string.IsNullOrWhiteSpace(position))
                {
                    result.Errors.Add($"Fila {row}: Cargo requerido para empleado '{fullName}'.");
                    continue;
                }

                var departmentName = GetCellValue(worksheet, row, headers, "departamento", "department", "area");
                if (string.IsNullOrWhiteSpace(departmentName))
                {
                    result.Errors.Add($"Fila {row}: Departamento requerido para empleado '{fullName}'.");
                    continue;
                }

                // Buscar departamento
                if (!departments.TryGetValue(departmentName.ToLower(), out var department))
                {
                    result.Errors.Add($"Fila {row}: Departamento '{departmentName}' no encontrado.");
                    continue;
                }

                // Campos opcionales
                var documentType = GetCellValue(worksheet, row, headers, "tipodocumento", "documenttype", "tipo") ??
                                   "CC";
                var phone = GetCellValue(worksheet, row, headers, "telefono", "phone", "celular", "tel");
                var address = GetCellValue(worksheet, row, headers, "direccion", "address");
                var gender = GetCellValue(worksheet, row, headers, "genero", "gender", "sexo");
                var professionalProfile = GetCellValue(worksheet, row, headers, "perfil", "perfilprofesional",
                    "profile", "professionalprofile");

                // Fecha de nacimiento
                DateTime? birthDate = null;
                var birthDateStr = GetCellValue(worksheet, row, headers, "fechanacimiento", "birthdate", "nacimiento");
                if (!string.IsNullOrWhiteSpace(birthDateStr) &&
                    DateTime.TryParse(birthDateStr, out var parsedBirthDate))
                {
                    birthDate = parsedBirthDate;
                }

                // Salario
                decimal salary = 0;
                var salaryStr = GetCellValue(worksheet, row, headers, "salario", "salary", "sueldo");
                if (!string.IsNullOrWhiteSpace(salaryStr))
                {
                    decimal.TryParse(salaryStr.Replace("$", "").Replace(",", ""), out salary);
                }

                // Fecha de ingreso
                DateTime hireDate = DateTime.UtcNow;
                var hireDateStr = GetCellValue(worksheet, row, headers, "fechaingreso", "hiredate", "ingreso",
                    "fechacontratacion");
                if (!string.IsNullOrWhiteSpace(hireDateStr) && DateTime.TryParse(hireDateStr, out var parsedHireDate))
                {
                    hireDate = parsedHireDate;
                }

                // Estado
                var statusStr = GetCellValue(worksheet, row, headers, "estado", "status");
                var status = ParseEmployeeStatus(statusStr);

                // Nivel educativo
                var educationStr = GetCellValue(worksheet, row, headers, "niveleducativo", "educationlevel",
                    "educacion", "education");
                var educationLevel = ParseEducationLevel(educationStr);

                // Verificar si el empleado ya existe (por documento o email)
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e =>
                        e.DocumentNumber == documentNumber || e.Email.ToLower() == email.ToLower());

                if (existingEmployee != null)
                {
                    // Actualizar empleado existente
                    existingEmployee.FullName = fullName;
                    existingEmployee.DocumentType = documentType;
                    existingEmployee.Email = email;
                    existingEmployee.Phone = phone;
                    existingEmployee.Address = address;
                    existingEmployee.BirthDate = birthDate;
                    existingEmployee.Gender = gender;
                    existingEmployee.Position = position;
                    existingEmployee.Salary = salary;
                    existingEmployee.HireDate = hireDate;
                    existingEmployee.Status = status;
                    existingEmployee.EducationLevel = educationLevel;
                    existingEmployee.ProfessionalProfile = professionalProfile;
                    existingEmployee.DepartmentId = department.Id;
                    existingEmployee.UpdatedAt = DateTime.UtcNow;
                    existingEmployee.IsActive = true;

                    result.EmployeesUpdated++;
                    result.Warnings.Add($"Fila {row}: Empleado '{fullName}' actualizado.");
                }
                else
                {
                    // Crear nuevo empleado
                    var employee = new Employee
                    {
                        DocumentNumber = documentNumber,
                        DocumentType = documentType,
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        Address = address,
                        BirthDate = birthDate,
                        Gender = gender,
                        Position = position,
                        Salary = salary,
                        HireDate = hireDate,
                        Status = status,
                        EducationLevel = educationLevel,
                        ProfessionalProfile = professionalProfile,
                        DepartmentId = department.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Employees.Add(employee);
                    result.EmployeesImported++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Fila {row}: Error al procesar empleado - {ex.Message}");
            }
        }
    }

    private bool ValidateRequiredHeaders(Dictionary<string, int> headers, ImportResult result)
    {
        var requiredHeaders = new[]
        {
            new[] { "documento", "numerodocumento", "documentnumber", "cedula" },
            // Nombre: puede ser combinado o separado (Nombres + Apellidos)
            new[] { "nombre", "nombrecompleto", "fullname", "name", "nombres", "primernombre", "firstname" },
            new[] { "cargo", "position", "puesto" },
            new[] { "departamento", "department", "area" }
        };

        foreach (var headerGroup in requiredHeaders)
        {
            if (!headerGroup.Any(h => headers.ContainsKey(h.ToLower())))
            {
                result.Errors.Add($"Falta columna requerida. Se esperaba alguna de: {string.Join(", ", headerGroup)}");
                return false;
            }
        }

        return true;
    }

    private EmployeeStatus ParseEmployeeStatus(string? statusStr)
    {
        if (string.IsNullOrWhiteSpace(statusStr))
            return EmployeeStatus.Activo;

        statusStr = statusStr.ToLower().Trim();

        return statusStr switch
        {
            "activo" or "active" => EmployeeStatus.Activo,
            "inactivo" or "inactive" => EmployeeStatus.Inactivo,
            "vacaciones" or "vacation" or "vacations" => EmployeeStatus.Vacaciones,
            "licencia" or "leave" => EmployeeStatus.Licencia,
            _ => EmployeeStatus.Activo
        };
    }

    private EducationLevel ParseEducationLevel(string? educationStr)
    {
        if (string.IsNullOrWhiteSpace(educationStr))
            return EducationLevel.Bachillerato;

        educationStr = educationStr.ToLower().Trim();

        return educationStr switch
        {
            "bachillerato" or "highschool" or "secundaria" => EducationLevel.Bachillerato,
            "tecnico" or "technical" => EducationLevel.Tecnico,
            "tecnologo" or "technologist" => EducationLevel.Tecnologo,
            "universitario" or "university" or "undergraduate" => EducationLevel.Universitario,
            "posgrado" or "postgraduate" => EducationLevel.Posgrado,
            "maestria" or "master" or "masters" => EducationLevel.Maestria,
            "doctorado" or "doctorate" or "phd" => EducationLevel.Doctorado,
            _ => EducationLevel.Bachillerato
        };
    }

    private Dictionary<string, int> GetHeaders(ExcelWorksheet worksheet)
    {
        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var colCount = worksheet.Dimension?.Columns ?? 0;

        for (int col = 1; col <= colCount; col++)
        {
            var headerValue = worksheet.Cells[1, col].Text?.Trim().ToLower().Replace(" ", "");
            if (!string.IsNullOrEmpty(headerValue) && !headers.ContainsKey(headerValue))
            {
                headers[headerValue] = col;
            }
        }

        return headers;
    }

    private string? GetCellValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers,
        params string[] possibleHeaders)
    {
        foreach (var header in possibleHeaders)
        {
            if (headers.TryGetValue(header.ToLower().Replace(" ", ""), out int col))
            {
                var value = worksheet.Cells[row, col].Text?.Trim();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
        }

        return null;
    }
}
