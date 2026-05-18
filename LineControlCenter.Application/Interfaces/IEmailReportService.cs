using LineControlCenter.Application.DTOs;

namespace LineControlCenter.Application.Interfaces;

public interface IEmailReportService
{
    /// <summary>Sends a formatted FNI DPM report to one or more recipients.</summary>
    Task SendDpmReportAsync(DpmReportDto report);

    /// <summary>Sends a PENANG Safety incidents report.</summary>
    Task SendSafetyReportAsync(
        string toAddresses,
        string fiscalYearLabel,
        IReadOnlyList<LccSafetyTblDto> incidents,
        string? note = null);

    /// <summary>Sends an NCR or CAR (NcrCarDetailDialog) drill-down report.</summary>
    Task SendNcrCarReportAsync(
        string toAddresses,
        string sectionType,
        string groupTitle,
        IReadOnlyList<LccNcrcarsTblDto> items,
        string? note = null);

    /// <summary>Sends a CAR (JCAS) drill-down report.</summary>
    Task SendCarReportAsync(
        string toAddresses,
        string categoryName,
        IReadOnlyList<JcasMainTblDto> items,
        string? note = null);

    /// <summary>Sends a QRQC tickets drill-down report.</summary>
    Task SendQrqcReportAsync(
        string toAddresses,
        IReadOnlyList<LccQrqcTicketDto> tickets,
        string? note = null);
}
