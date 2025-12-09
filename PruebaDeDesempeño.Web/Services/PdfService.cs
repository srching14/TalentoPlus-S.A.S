using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PruebaDeDesempeño.Web.Models;

namespace PruebaDeDesempeño.Web.Services;

public interface IPdfService
{
    byte[] GenerateEmployeeCV(Employee employee);
    string SaveCVToFile(Employee employee);
}

public class PdfService : IPdfService
{
    private readonly IWebHostEnvironment _environment;

    public PdfService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public byte[] GenerateEmployeeCV(Employee employee)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeHeader(c, employee));
                page.Content().Element(c => ComposeContent(c, employee));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public string SaveCVToFile(Employee employee)
    {
        var pdfBytes = GenerateEmployeeCV(employee);

        var cvsPath = Path.Combine(_environment.WebRootPath, "cvs");
        if (!Directory.Exists(cvsPath))
        {
            Directory.CreateDirectory(cvsPath);
        }

        var fileName = $"cv_{employee.DocumentNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var filePath = Path.Combine(cvsPath, fileName);

        File.WriteAllBytes(filePath, pdfBytes);

        return $"/cvs/{fileName}";
    }

    private void ComposeHeader(IContainer container, Employee employee)
    {
        container.Column(column =>
        {
            // Header principal
            column.Item().Background(Colors.Blue.Darken3).Padding(15).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("HOJA DE VIDA")
                        .FontSize(24)
                        .Bold()
                        .FontColor(Colors.White);

                    col.Item().PaddingTop(5).Text(employee.FullName)
                        .FontSize(18)
                        .SemiBold()
                        .FontColor(Colors.White);

                    col.Item().Text(employee.Position)
                        .FontSize(12)
                        .FontColor(Colors.Grey.Lighten2);
                });
            });

            // Línea separadora
            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Blue.Darken3);
        });
    }

    private void ComposeContent(IContainer container, Employee employee)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Información Personal
            column.Item().Element(c => ComposeSection(c, "INFORMACIÓN PERSONAL", () =>
            {
                return new List<(string Label, string Value)>
                {
                    ("Documento", $"{employee.DocumentType} {employee.DocumentNumber}"),
                    ("Fecha de Nacimiento", employee.BirthDate?.ToString("dd/MM/yyyy") ?? "No especificada"),
                    ("Género", employee.Gender ?? "No especificado"),
                    ("Email", employee.Email),
                    ("Teléfono", employee.Phone ?? "No especificado"),
                    ("Dirección", employee.Address ?? "No especificada")
                };
            }));

            column.Item().PaddingTop(15);

            // Información Laboral
            column.Item().Element(c => ComposeSection(c, "INFORMACIÓN LABORAL", () =>
            {
                return new List<(string Label, string Value)>
                {
                    ("Cargo", employee.Position),
                    ("Departamento", employee.Department?.Name ?? "No asignado"),
                    ("Fecha de Ingreso", employee.HireDate.ToString("dd/MM/yyyy")),
                    ("Estado", GetStatusText(employee.Status)),
                    ("Salario", $"${employee.Salary:N2} COP")
                };
            }));

            column.Item().PaddingTop(15);

            // Formación Académica
            column.Item().Element(c => ComposeSection(c, "FORMACIÓN ACADÉMICA", () =>
            {
                return new List<(string Label, string Value)>
                {
                    ("Nivel Educativo", GetEducationLevelText(employee.EducationLevel))
                };
            }));

            column.Item().PaddingTop(15);

            // Perfil Profesional
            if (!string.IsNullOrWhiteSpace(employee.ProfessionalProfile))
            {
                column.Item().Column(profileCol =>
                {
                    profileCol.Item().Text("PERFIL PROFESIONAL")
                        .FontSize(14)
                        .SemiBold()
                        .FontColor(Colors.Blue.Darken3);

                    profileCol.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Darken3);

                    profileCol.Item().PaddingTop(10).Background(Colors.Grey.Lighten4)
                        .Padding(10)
                        .Text(employee.ProfessionalProfile)
                        .FontSize(10)
                        .LineHeight(1.5f);
                });

                column.Item().PaddingTop(15);
            }

            // Datos de Contacto (resumen)
            column.Item().Background(Colors.Blue.Lighten4).Padding(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("DATOS DE CONTACTO").SemiBold().FontSize(11);
                    col.Item().PaddingTop(3).Text($"📧 {employee.Email}").FontSize(9);
                    if (!string.IsNullOrWhiteSpace(employee.Phone))
                    {
                        col.Item().Text($"📱 {employee.Phone}").FontSize(9);
                    }
                });
            });
        });
    }

    private void ComposeSection(IContainer container, string title,
        Func<List<(string Label, string Value)>> dataProvider)
    {
        container.Column(column =>
        {
            // Título de la sección
            column.Item().Text(title)
                .FontSize(14)
                .SemiBold()
                .FontColor(Colors.Blue.Darken3);

            column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Darken3);

            // Contenido
            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                var data = dataProvider();
                foreach (var (label, value) in data)
                {
                    table.Cell().Padding(3).Text(label + ":").SemiBold();
                    table.Cell().Padding(3).Text(value);
                }
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

            column.Item().PaddingTop(10).Text(text =>
            {
                text.Span("TalentoPlus S.A.S. - Sistema de Gestión de Empleados").FontSize(9)
                    .FontColor(Colors.Grey.Darken1);
            });

            column.Item().Text(text =>
            {
                text.Span("Generado el: ").FontSize(8).FontColor(Colors.Grey.Darken1);
                text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private string GetStatusText(EmployeeStatus status)
    {
        return status switch
        {
            EmployeeStatus.Activo => "Activo",
            EmployeeStatus.Inactivo => "Inactivo",
            EmployeeStatus.Vacaciones => "De Vacaciones",
            EmployeeStatus.Licencia => "En Licencia",
            _ => "Desconocido"
        };
    }

    private string GetEducationLevelText(EducationLevel level)
    {
        return level switch
        {
            EducationLevel.Bachillerato => "Bachillerato",
            EducationLevel.Tecnico => "Técnico",
            EducationLevel.Tecnologo => "Tecnólogo",
            EducationLevel.Universitario => "Universitario",
            EducationLevel.Posgrado => "Posgrado",
            EducationLevel.Maestria => "Maestría",
            EducationLevel.Doctorado => "Doctorado",
            _ => "No especificado"
        };
    }
}
