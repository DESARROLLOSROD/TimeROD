using TimeROD.Core.DTOs;

namespace TimeROD.Core.Interfaces;

public interface IReportExportService
{
    byte[] GenerateExcel(object reporteData);
    byte[] GeneratePdf(object reporteData);
}
