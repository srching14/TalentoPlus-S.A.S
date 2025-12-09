using OfficeOpenXml;
using OfficeOpenXml.Style;
using PruebaDeDesempeño.Web.Data;
using PruebaDeDesempeño.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace PruebaDeDesempeño.Web.Services;

public interface IExcelExportService
{
    Task<byte[]> ExportEmployeesAsync();
}

public class ExcelExportService : IExcelExportService
{
    private readonly ApplicationDbContext _context;

    public ExcelExportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportEmployeesAsync()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .OrderBy(e => e.FullName)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Empleados");

        // Headers
        var headers = new[]
        {
            "Documento", "Tipo Doc.", "Nombre Completo", "Email", "Teléfono",
            "Dirección", "Departamento", "Cargo", "Salario", "Fecha Ingreso",
            "Estado", "Nivel Educativo"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        // Estilo de encabezados
        using (var range = worksheet.Cells[1, 1, 1, headers.Length])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
            range.Style.Font.Color.SetColor(Color.White);
        }

        // Datos
        int row = 2;
        foreach (var emp in employees)
        {
            worksheet.Cells[row, 1].Value = emp.DocumentNumber;
            worksheet.Cells[row, 2].Value = emp.DocumentType;
            worksheet.Cells[row, 3].Value = emp.FullName;
            worksheet.Cells[row, 4].Value = emp.Email;
            worksheet.Cells[row, 5].Value = emp.Phone;
            worksheet.Cells[row, 6].Value = emp.Address;
            worksheet.Cells[row, 7].Value = emp.Department?.Name;
            worksheet.Cells[row, 8].Value = emp.Position;
            worksheet.Cells[row, 9].Value = emp.Salary;
            worksheet.Cells[row, 10].Value = emp.HireDate.ToString("yyyy-MM-dd");
            worksheet.Cells[row, 11].Value = emp.Status.ToString();
            worksheet.Cells[row, 12].Value = emp.EducationLevel.ToString();
            row++;
        }

        // Formato de moneda
        worksheet.Column(9).Style.Numberformat.Format = "$#,##0.00";

        // Auto-ajustar columnas
        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }
}
