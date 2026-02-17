using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TimeROD.Core.DTOs;
using TimeROD.Core.Interfaces;
using System.Reflection;

namespace TimeROD.Infrastructure.Services;

public class ReportExportService : IReportExportService
{
    public ReportExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateExcel(object reporteData)
    {
        // Using reflection to access properties of the anonymous object or DTO
        // Ideally we should use a concrete DTO for the report data
        var type = reporteData.GetType();
        var asistenciasProp = type.GetProperty("Asistencias");
        var asistencias = (IEnumerable<AsistenciaDto>)asistenciasProp.GetValue(reporteData);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Reporte Asistencias");

            // Headers
            worksheet.Cell(1, 1).Value = "Empleado";
            worksheet.Cell(1, 2).Value = "Fecha";
            worksheet.Cell(1, 3).Value = "Entrada";
            worksheet.Cell(1, 4).Value = "Salida";
            worksheet.Cell(1, 5).Value = "Horas";
            worksheet.Cell(1, 6).Value = "Estado";
            worksheet.Cell(1, 7).Value = "Retardo (min)";
            worksheet.Cell(1, 8).Value = "Salida Anticipada (min)";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var a in asistencias)
            {
                worksheet.Cell(row, 1).Value = $"{a.EmpleadoNombre} {a.EmpleadoApellidos}";
                worksheet.Cell(row, 2).Value = a.Fecha.ToShortDateString();
                worksheet.Cell(row, 3).Value = a.HoraEntrada?.ToString("HH:mm:ss") ?? "-";
                worksheet.Cell(row, 4).Value = a.HoraSalida?.ToString("HH:mm:ss") ?? "-";
                worksheet.Cell(row, 5).Value = a.HorasTrabajadas?.ToString("F2") ?? "0";
                
                string estado = a.Tipo;
                if (a.LlegadaTardia) estado += " (Tarde)";
                if (a.SalidaAnticipada) estado += " (Salida Ant.)";
                
                worksheet.Cell(row, 6).Value = estado;
                worksheet.Cell(row, 7).Value = a.MinutosRetraso ?? 0;
                worksheet.Cell(row, 8).Value = a.MinutosAnticipados ?? 0;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

    public byte[] GeneratePdf(object reporteData)
    {
        var type = reporteData.GetType();
        var asistenciasProp = type.GetProperty("Asistencias");
        var asistencias = (IEnumerable<AsistenciaDto>)asistenciasProp.GetValue(reporteData);
        
        var totalHorasProp = type.GetProperty("TotalHorasTrabajadas");
        var totalHoras = totalHorasProp?.GetValue(reporteData)?.ToString() ?? "0";
        
        var llegadasTardiasProp = type.GetProperty("LlegadasTardias");
        var llegadasTardias = llegadasTardiasProp?.GetValue(reporteData)?.ToString() ?? "0";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text("Reporte de Asistencias - TimeROD")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Item().Text($"Generado: {DateTime.Now}");
                        x.Item().Text($"Total Horas: {totalHoras}");
                        x.Item().Text($"Llegadas Tardías: {llegadasTardias}");
                        x.Item().Padding(10); // Spacer

                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Empleado
                                columns.RelativeColumn(2); // Fecha
                                columns.RelativeColumn(2); // Entrada
                                columns.RelativeColumn(2); // Salida
                                columns.RelativeColumn(1); // Horas
                                columns.RelativeColumn(2); // Estado
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Empleado");
                                header.Cell().Element(CellStyle).Text("Fecha");
                                header.Cell().Element(CellStyle).Text("Entrada");
                                header.Cell().Element(CellStyle).Text("Salida");
                                header.Cell().Element(CellStyle).Text("Hrs");
                                header.Cell().Element(CellStyle).Text("Estado");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var a in asistencias)
                            {
                                table.Cell().Element(CellStyle).Text($"{a.EmpleadoNombre} {a.EmpleadoApellidos}");
                                table.Cell().Element(CellStyle).Text(a.Fecha.ToShortDateString());
                                table.Cell().Element(CellStyle).Text(a.HoraEntrada?.ToString("HH:mm") ?? "-");
                                table.Cell().Element(CellStyle).Text(a.HoraSalida?.ToString("HH:mm") ?? "-");
                                table.Cell().Element(CellStyle).Text(a.HorasTrabajadas?.ToString("F1") ?? "0");
                                
                                string estado = a.Tipo;
                                if (a.LlegadaTardia) estado = "Tarde/ " + estado;
                                else if (a.SalidaAnticipada) estado = "Sal.Ant/ " + estado;
                                
                                table.Cell().Element(CellStyle).Text(estado).FontSize(9);

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
