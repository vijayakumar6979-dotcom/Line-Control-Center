using System.Net;
using System.Net.Mail;
using System.Text;
using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LineControlCenter.UI.Services;

public sealed class EmailReportService : IEmailReportService
{
    private readonly IConfiguration _config;

    public EmailReportService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendDpmReportAsync(DpmReportDto report)
    {
        var section     = _config.GetSection("Feedback");
        var smtpHost    = section["SmtpHost"]     ?? throw new InvalidOperationException("Feedback:SmtpHost not configured.");
        var smtpPort    = int.Parse(section["SmtpPort"] ?? "25");
        var smtpUser    = section["SmtpUser"]     ?? string.Empty;
        var smtpPass    = section["SmtpPassword"] ?? string.Empty;
        var enableSsl   = bool.Parse(section["EnableSsl"] ?? "false");
        var fromAddress = section["FromAddress"]  ?? "lcc-backend-noreply@jabil.com";
        var fromName    = section["FromName"]     ?? "Line Control Center";

        // ── Parse recipients ────────────────────────────────────────────────
        var recipients = report.ToAddresses
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(e => e.Contains('@'))
            .ToArray();

        if (recipients.Length == 0)
            throw new InvalidOperationException("No valid recipient email addresses provided.");

        // ── Build HTML body ─────────────────────────────────────────────────
        var html = BuildHtmlBody(report);

        // ── Send ────────────────────────────────────────────────────────────
        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl   = enableSsl,
            Credentials = string.IsNullOrEmpty(smtpUser)
                ? null
                : new NetworkCredential(smtpUser, smtpPass)
        };

        var subject = $"[LCC] FNI DPM Report — {report.PeriodLabel} | {report.DateRangeLabel}";
        if (!string.IsNullOrWhiteSpace(report.Family))
            subject += $" | {report.Family}";

        var mail = new MailMessage
        {
            From       = new MailAddress(fromAddress, fromName),
            Subject    = subject,
            Body       = html,
            IsBodyHtml = true
        };
        foreach (var addr in recipients)
            mail.To.Add(addr);

        await client.SendMailAsync(mail);
    }

    // ── HTML builder ────────────────────────────────────────────────────────

    private static string BuildHtmlBody(DpmReportDto r)
    {
        var sb = new StringBuilder();

        // ── Wrapper ──
        sb.Append(@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<style>
  body        { font-family: 'Segoe UI', Arial, sans-serif; background:#f0f4f8; margin:0; padding:20px; }
  .card       { background:#ffffff; border-radius:12px; padding:24px 28px; margin-bottom:20px;
                box-shadow:0 2px 8px rgba(0,0,0,0.08); }
  .header     { background:linear-gradient(135deg,#0d1f3c,#0a3a5c); border-radius:12px;
                padding:24px 28px; margin-bottom:20px; color:#fff; }
  h1          { margin:0 0 4px; font-size:20px; letter-spacing:2px; color:#00b4d8; }
  .sub        { color:#7ab3cc; font-size:12px; letter-spacing:1px; }
  h2          { font-size:13px; letter-spacing:2px; color:#0a3a5c; margin:0 0 14px;
                text-transform:uppercase; border-bottom:2px solid #00b4d8; padding-bottom:6px; }
  table       { width:100%; border-collapse:collapse; font-size:12px; }
  th          { background:#0d1f3c; color:#00b4d8; text-align:left; padding:8px 10px;
                letter-spacing:1px; font-size:11px; }
  td          { padding:7px 10px; border-bottom:1px solid #e8eef4; color:#334155; }
  tr:hover td { background:#f8fafc; }
  .kpi-grid   { display:grid; grid-template-columns:repeat(3,1fr); gap:14px; }
  .kpi        { background:#f0f7ff; border-radius:8px; padding:14px 16px;
                border-left:4px solid #00b4d8; }
  .kpi-label  { font-size:10px; color:#64748b; letter-spacing:1px; text-transform:uppercase; }
  .kpi-value  { font-size:24px; font-weight:700; color:#0a3a5c; margin-top:4px; }
  .badge-pass { color:#16a34a; font-weight:600; }
  .badge-fail { color:#dc2626; font-weight:600; }
  .note-box   { background:#fffbeb; border:1px solid #fcd34d; border-radius:8px;
                padding:12px 16px; font-size:12px; color:#92400e; margin-bottom:20px; }
  .footer     { font-size:10px; color:#94a3b8; text-align:center; margin-top:20px; }
</style>
</head>
<body>
");

        // ── Header ──
        sb.Append($@"
<div class='header'>
  <h1>FNI DPM REPORT · {r.PeriodLabel.ToUpperInvariant()}</h1>
  <div class='sub'>{r.DateRangeLabel} &nbsp;·&nbsp; {r.Customer}");
        if (!string.IsNullOrEmpty(r.Family))
            sb.Append($" &nbsp;·&nbsp; {r.Family}");
        sb.Append($@"</div>
  <div class='sub' style='margin-top:4px;'>Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>
</div>
");

        // ── Optional user note ──
        if (!string.IsNullOrWhiteSpace(r.Note))
        {
            sb.Append($@"
<div class='note-box'>
  <strong>📝 Note from sender:</strong><br>{System.Net.WebUtility.HtmlEncode(r.Note)}
</div>
");
        }

        // ── KPI Summary ──
        var prevLabel   = r.PeriodLabel == "WTD" ? "PREV WEEK" : "YESTERDAY";
        var rollingLabel = r.PeriodLabel == "WTD" ? "3-WEEK AVG" : "7-DAY AVG";
        sb.Append($@"
<div class='card'>
  <h2>KPI Summary</h2>
  <div class='kpi-grid'>
    <div class='kpi'>
      <div class='kpi-label'>{r.PeriodLabel} DPM</div>
      <div class='kpi-value' style='color:{DpmColor(r.CurrentDpm)};'>{r.CurrentDpm:F0}</div>
    </div>
    <div class='kpi'>
      <div class='kpi-label'>{prevLabel} DPM</div>
      <div class='kpi-value'>{r.PreviousDpm:F0}</div>
    </div>
    <div class='kpi'>
      <div class='kpi-label'>{rollingLabel}</div>
      <div class='kpi-value' style='color:#d97706;'>{r.RollingAvgDpm:F0}</div>
    </div>
  </div>
</div>
");

        // ── Family Ranking ──
        if (r.FamilyRanking.Count > 0)
        {
            sb.Append(@"
<div class='card'>
  <h2>Family DPM Ranking</h2>
  <table>
    <thead><tr>
      <th>#</th><th>Family</th><th>Total</th><th>Pass</th><th>Fail</th><th>DPM</th>
    </tr></thead>
    <tbody>
");
            var rank = 1;
            foreach (var f in r.FamilyRanking.OrderByDescending(x => x.DpmRate))
            {
                sb.Append($@"
      <tr>
        <td>{rank++}</td>
        <td>{System.Net.WebUtility.HtmlEncode(f.Family)}</td>
        <td>{f.Total}</td>
        <td class='badge-pass'>{f.Pass}</td>
        <td class='badge-fail'>{f.Fail}</td>
        <td style='font-weight:700; color:{DpmColor(f.DpmRate)};'>{f.DpmRate:F0}</td>
      </tr>");
            }
            sb.Append(@"
    </tbody>
  </table>
</div>
");
        }

        // ── Raw Data ──
        if (r.RawDetails.Count > 0)
        {
            sb.Append($@"
<div class='card'>
  <h2>Raw Data ({r.RawDetails.Count} records)</h2>
  <table>
    <thead><tr>
      <th>Serial Number</th><th>Start Time</th><th>End Time</th>
      <th>Status</th><th>Family</th><th>Step</th>
    </tr></thead>
    <tbody>
");
            foreach (var d in r.RawDetails)
            {
                var statusStyle = d.Status?.Equals("PASS", StringComparison.OrdinalIgnoreCase) == true
                    ? "class='badge-pass'" : "class='badge-fail'";
                sb.Append($@"
      <tr>
        <td>{System.Net.WebUtility.HtmlEncode(d.SerialNumber ?? "-")}</td>
        <td>{d.StartTime?.ToString("MM/dd HH:mm:ss") ?? "-"}</td>
        <td>{d.EndTime?.ToString("MM/dd HH:mm:ss") ?? "-"}</td>
        <td {statusStyle}>{System.Net.WebUtility.HtmlEncode(d.Status)}</td>
        <td>{System.Net.WebUtility.HtmlEncode(d.Family ?? "-")}</td>
        <td>{System.Net.WebUtility.HtmlEncode(d.StepInstance ?? "-")}</td>
      </tr>");
            }
            sb.Append(@"
    </tbody>
  </table>
</div>
");
        }

        sb.Append(@"
<div class='footer'>This report was generated by Line Control Center — Backend &nbsp;·&nbsp; Do not reply to this email.</div>
</body>
</html>");

        return sb.ToString();
    }

    private static string DpmColor(double dpm) =>
        dpm <= 500  ? "#16a34a" :
        dpm <= 1000 ? "#d97706" : "#dc2626";

    // ════════════════════════════════════════════════════════════════════════
    // SAFETY REPORT
    // ════════════════════════════════════════════════════════════════════════

    public async Task SendSafetyReportAsync(
        string toAddresses,
        string fiscalYearLabel,
        IReadOnlyList<LccSafetyTblDto> incidents,
        string? note = null)
    {
        var (smtpClient, from, fromName) = BuildSmtpClient();
        var recipients = ParseRecipients(toAddresses);

        var subject = $"[LCC] Safety Incidents Report · {fiscalYearLabel} · {incidents.Count} Total";
        var html    = BuildSafetyHtml(fiscalYearLabel, incidents, note);

        using var mail = new MailMessage { From = new MailAddress(from, fromName), Subject = subject, Body = html, IsBodyHtml = true };
        foreach (var r in recipients) mail.To.Add(r);
        await smtpClient.SendMailAsync(mail);
        smtpClient.Dispose();
    }

    private static string BuildSafetyHtml(string fiscalYearLabel, IReadOnlyList<LccSafetyTblDto> incidents, string? note)
    {
        var sb = new StringBuilder();
        AppendHtmlHead(sb, "#f59e0b");

        sb.Append($@"
<div class='header' style='border-left:4px solid #f59e0b;'>
  <h1 style='color:#f59e0b;'>⚠ SAFETY INCIDENTS REPORT</h1>
  <div class='sub'>{WebUtility.HtmlEncode(fiscalYearLabel)} &nbsp;·&nbsp; {incidents.Count} TOTAL &nbsp;·&nbsp; Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>
</div>");

        if (!string.IsNullOrWhiteSpace(note))
            sb.Append($"<div class='note-box'><strong>📝 Note:</strong><br>{WebUtility.HtmlEncode(note)}</div>");

        // KPI strip
        var recordable  = incidents.Count(x => x.RecordableInjuryOrIllness == true);
        var highPot     = incidents.Count(x => x.HighPotential == true);
        var lostTime    = incidents.Where(x => x.LostTimeDays > 0).Sum(x => x.LostTimeDays ?? 0);
        sb.Append($@"
<div class='card'>
  <h2>KPI Summary</h2>
  <div class='kpi-grid'>
    <div class='kpi' style='border-left-color:#f59e0b;'>
      <div class='kpi-label'>TOTAL INCIDENTS</div>
      <div class='kpi-value' style='color:#f59e0b;'>{incidents.Count}</div>
    </div>
    <div class='kpi' style='border-left-color:#dc2626;'>
      <div class='kpi-label'>RECORDABLE</div>
      <div class='kpi-value' style='color:{(recordable > 0 ? "#dc2626" : "#16a34a")};'>{recordable}</div>
    </div>
    <div class='kpi' style='border-left-color:#d97706;'>
      <div class='kpi-label'>HIGH POTENTIAL</div>
      <div class='kpi-value' style='color:{(highPot > 0 ? "#d97706" : "#16a34a")};'>{highPot}</div>
    </div>
    <div class='kpi' style='border-left-color:#7c3aed;'>
      <div class='kpi-label'>TOTAL LOST TIME DAYS</div>
      <div class='kpi-value' style='color:{(lostTime > 0 ? "#dc2626" : "#16a34a")};'>{lostTime}</div>
    </div>
  </div>
</div>");

        // Detail table
        sb.Append($@"
<div class='card'>
  <h2>Incident Details ({incidents.Count} records)</h2>
  <table>
    <thead><tr>
      <th>Safety No.</th><th>Incident Date</th><th>Title</th><th>Type</th>
      <th>Status</th><th>Severity</th><th>Recordable</th><th>High Pot.</th>
      <th>Lost Days</th><th>CAPA/IP No.</th>
    </tr></thead><tbody>");

        foreach (var r in incidents.OrderByDescending(x => x.IncidentDatetime))
        {
            var severityColor = r.Severity?.ToUpperInvariant() switch
            {
                "HIGH" or "CRITICAL" => "#dc2626",
                "MEDIUM"             => "#d97706",
                _                    => "#16a34a"
            };
            sb.Append($@"
      <tr>
        <td><strong>{WebUtility.HtmlEncode(r.SafetyNoId)}</strong></td>
        <td>{r.IncidentDatetime?.ToString("dd MMM yyyy") ?? "-"}</td>
        <td>{WebUtility.HtmlEncode(r.IncidentTitle ?? "-")}</td>
        <td>{WebUtility.HtmlEncode(r.TypeOfIncident ?? "-")}</td>
        <td><span style='color:{StatusColor(r.Status)};font-weight:600;'>{WebUtility.HtmlEncode(r.Status ?? "-")}</span></td>
        <td style='color:{severityColor};font-weight:600;'>{WebUtility.HtmlEncode(r.Severity ?? "-")}</td>
        <td style='color:{(r.RecordableInjuryOrIllness == true ? "#dc2626" : "#16a34a")};font-weight:600;'>{(r.RecordableInjuryOrIllness == true ? "YES" : "NO")}</td>
        <td style='color:{(r.HighPotential == true ? "#d97706" : "#16a34a")};font-weight:600;'>{(r.HighPotential == true ? "YES" : "NO")}</td>
        <td style='color:{(r.LostTimeDays > 0 ? "#dc2626" : "#334155")};'>{r.LostTimeDays ?? 0}</td>
        <td>{WebUtility.HtmlEncode(r.CapaOrIpNo ?? "-")}</td>
      </tr>");
        }
        sb.Append("</tbody></table></div>");
        AppendHtmlFooter(sb);
        return sb.ToString();
    }

    // ════════════════════════════════════════════════════════════════════════
    // NCR / CAR (NcrCarDetail) REPORT
    // ════════════════════════════════════════════════════════════════════════

    public async Task SendNcrCarReportAsync(
        string toAddresses,
        string sectionType,
        string groupTitle,
        IReadOnlyList<LccNcrcarsTblDto> items,
        string? note = null)
    {
        var (smtpClient, from, fromName) = BuildSmtpClient();
        var recipients = ParseRecipients(toAddresses);
        var accentColor = sectionType == "NCR" ? "#ff6b35" : "#aa55ff";

        var subject = $"[LCC] {sectionType} Report · {groupTitle} · {items.Count} Records";
        var html    = BuildNcrCarHtml(sectionType, groupTitle, accentColor, items, note);

        using var mail = new MailMessage { From = new MailAddress(from, fromName), Subject = subject, Body = html, IsBodyHtml = true };
        foreach (var r in recipients) mail.To.Add(r);
        await smtpClient.SendMailAsync(mail);
        smtpClient.Dispose();
    }

    private static string BuildNcrCarHtml(string sectionType, string groupTitle, string accentColor,
        IReadOnlyList<LccNcrcarsTblDto> items, string? note)
    {
        static bool IsClosed(LccNcrcarsTblDto x) =>
            x.Status?.Equals("Closed Completed",    StringComparison.OrdinalIgnoreCase) == true ||
            x.Status?.Equals("Closed-Cancellation", StringComparison.OrdinalIgnoreCase) == true;

        var sb = new StringBuilder();
        AppendHtmlHead(sb, accentColor);

        sb.Append($@"
<div class='header' style='border-left:4px solid {accentColor};'>
  <h1 style='color:{accentColor};'>{WebUtility.HtmlEncode(sectionType)} DRILL-DOWN REPORT</h1>
  <div class='sub'>{WebUtility.HtmlEncode(groupTitle)} &nbsp;·&nbsp; {items.Count} RECORDS &nbsp;·&nbsp; Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>
</div>");

        if (!string.IsNullOrWhiteSpace(note))
            sb.Append($"<div class='note-box'><strong>📝 Note:</strong><br>{WebUtility.HtmlEncode(note)}</div>");

        var openCount   = items.Count(x => !IsClosed(x));
        var closedCount = items.Count(IsClosed);
        var overdueCount = items.Count(x => !IsClosed(x) && !string.IsNullOrWhiteSpace(x.ClosureAging));

        sb.Append($@"
<div class='card'>
  <h2>KPI Summary</h2>
  <div class='kpi-grid'>
    <div class='kpi' style='border-left-color:{accentColor};'>
      <div class='kpi-label'>TOTAL</div>
      <div class='kpi-value' style='color:{accentColor};'>{items.Count}</div>
    </div>
    <div class='kpi' style='border-left-color:#dc2626;'>
      <div class='kpi-label'>OPEN</div>
      <div class='kpi-value' style='color:{(openCount > 0 ? "#dc2626" : "#16a34a")};'>{openCount}</div>
    </div>
    <div class='kpi' style='border-left-color:#16a34a;'>
      <div class='kpi-label'>CLOSED</div>
      <div class='kpi-value' style='color:#16a34a;'>{closedCount}</div>
    </div>
    <div class='kpi' style='border-left-color:#d97706;'>
      <div class='kpi-label'>OVERDUE</div>
      <div class='kpi-value' style='color:{(overdueCount > 0 ? "#d97706" : "#16a34a")};'>{overdueCount}</div>
    </div>
  </div>
</div>");

        sb.Append($@"
<div class='card'>
  <h2>Records ({items.Count})</h2>
  <table>
    <thead><tr>
      <th>No.</th><th>Type</th><th>Issue Date</th><th>Problem Category</th>
      <th>Bay</th><th>Dept.</th><th>Owner</th><th>Status</th><th>Closure Status</th>
    </tr></thead><tbody>");

        foreach (var r in items.OrderByDescending(x => x.IssueDate))
        {
            sb.Append($@"
      <tr>
        <td><strong>{WebUtility.HtmlEncode(r.NcrcarNo ?? "-")}</strong></td>
        <td style='color:{accentColor};font-weight:600;'>{WebUtility.HtmlEncode(r.NcrType ?? "-")}</td>
        <td>{r.IssueDate:dd MMM yyyy}</td>
        <td>{WebUtility.HtmlEncode(r.ProblemStatementCategory ?? "-")}</td>
        <td>{WebUtility.HtmlEncode(r.Bay ?? "-")}</td>
        <td>{WebUtility.HtmlEncode(r.Department ?? "-")}</td>
        <td>{WebUtility.HtmlEncode(r.CarOwner ?? "-")}</td>
        <td><span style='color:{StatusColor(r.Status)};font-weight:600;'>{WebUtility.HtmlEncode(r.Status ?? "-")}</span></td>
        <td>{WebUtility.HtmlEncode(r.ClosureStatus ?? "-")}</td>
      </tr>");
        }
        sb.Append("</tbody></table></div>");
        AppendHtmlFooter(sb);
        return sb.ToString();
    }

    // ════════════════════════════════════════════════════════════════════════
    // CAR (JCAS) REPORT
    // ════════════════════════════════════════════════════════════════════════

    public async Task SendCarReportAsync(
        string toAddresses,
        string categoryName,
        IReadOnlyList<JcasMainTblDto> items,
        string? note = null)
    {
        var (smtpClient, from, fromName) = BuildSmtpClient();
        var recipients = ParseRecipients(toAddresses);

        var subject = $"[LCC] CAR Report · {categoryName} · {items.Count} Records";
        var html    = BuildCarHtml(categoryName, items, note);

        using var mail = new MailMessage { From = new MailAddress(from, fromName), Subject = subject, Body = html, IsBodyHtml = true };
        foreach (var r in recipients) mail.To.Add(r);
        await smtpClient.SendMailAsync(mail);
        smtpClient.Dispose();
    }

    private static string BuildCarHtml(string categoryName, IReadOnlyList<JcasMainTblDto> items, string? note)
    {
        static bool IsCompleted(JcasMainTblDto x) =>
            x.Phase?.Equals("Complete",  StringComparison.OrdinalIgnoreCase) == true ||
            x.Phase?.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) == true;

        const string accent = "#00d4ff";
        var sb = new StringBuilder();
        AppendHtmlHead(sb, accent);

        var openCount     = items.Count(x => !IsCompleted(x));
        var closedCount   = items.Count(IsCompleted);
        var overdueCount  = items.Count(x => !IsCompleted(x) && x.D7DueDate.HasValue && x.D7DueDate.Value < DateOnly.FromDateTime(DateTime.Today));

        sb.Append($@"
<div class='header' style='border-left:4px solid {accent};'>
  <h1 style='color:{accent};'>CAR DRILL-DOWN REPORT</h1>
  <div class='sub'>{WebUtility.HtmlEncode(categoryName)} &nbsp;·&nbsp; {items.Count} RECORDS &nbsp;·&nbsp; Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>
</div>");

        if (!string.IsNullOrWhiteSpace(note))
            sb.Append($"<div class='note-box'><strong>📝 Note:</strong><br>{WebUtility.HtmlEncode(note)}</div>");

        sb.Append($@"
<div class='card'>
  <h2>KPI Summary</h2>
  <div class='kpi-grid'>
    <div class='kpi' style='border-left-color:{accent};'>
      <div class='kpi-label'>TOTAL</div>
      <div class='kpi-value' style='color:{accent};'>{items.Count}</div>
    </div>
    <div class='kpi' style='border-left-color:#dc2626;'>
      <div class='kpi-label'>OPEN</div>
      <div class='kpi-value' style='color:{(openCount > 0 ? "#dc2626" : "#16a34a")};'>{openCount}</div>
    </div>
    <div class='kpi' style='border-left-color:#16a34a;'>
      <div class='kpi-label'>CLOSED / CANCELLED</div>
      <div class='kpi-value' style='color:#16a34a;'>{closedCount}</div>
    </div>
    <div class='kpi' style='border-left-color:#d97706;'>
      <div class='kpi-label'>OVERDUE (D7)</div>
      <div class='kpi-value' style='color:{(overdueCount > 0 ? "#d97706" : "#16a34a")};'>{overdueCount}</div>
    </div>
  </div>
</div>");

        sb.Append($@"
<div class='card'>
  <h2>CAR Records ({items.Count})</h2>
  <table>
    <thead><tr>
      <th>JCAS No.</th><th>Customer</th><th>Created</th><th>Failure Mode</th>
      <th>Phase</th><th>Status</th><th>Severity</th><th>D7 Due</th><th>Owner</th>
    </tr></thead><tbody>");

        foreach (var r in items.OrderByDescending(x => x.JcasCreatedDate))
        {
            var isOverdue  = !IsCompleted(r) && r.D7DueDate.HasValue && r.D7DueDate.Value < DateOnly.FromDateTime(DateTime.Today);
            sb.Append($@"
      <tr>
        <td><strong style='color:{accent};'>{WebUtility.HtmlEncode(r.JcasRecordNumber)}</strong></td>
        <td>{WebUtility.HtmlEncode(r.CustomerName ?? "-")}</td>
        <td>{r.JcasCreatedDate?.ToString("dd MMM yyyy") ?? "-"}</td>
        <td>{WebUtility.HtmlEncode(r.FailureMode ?? "-")}</td>
        <td>{WebUtility.HtmlEncode(r.Phase ?? "-")}</td>
        <td><span style='color:{StatusColor(r.Status)};font-weight:600;'>{WebUtility.HtmlEncode(r.Status ?? "-")}</span></td>
        <td>{WebUtility.HtmlEncode(r.Severity ?? "-")}</td>
        <td style='color:{(isOverdue ? "#dc2626" : "#334155")};font-weight:{(isOverdue ? "700" : "400")};'>{r.D7DueDate?.ToString("dd MMM yyyy") ?? "-"}{(isOverdue ? " ⚠" : "")}</td>
        <td>{WebUtility.HtmlEncode(r.JcasOwner ?? "-")}</td>
      </tr>");
        }
        sb.Append("</tbody></table></div>");
        AppendHtmlFooter(sb);
        return sb.ToString();
    }

    // ════════════════════════════════════════════════════════════════════════
    // QRQC REPORT
    // ════════════════════════════════════════════════════════════════════════

    public async Task SendQrqcReportAsync(
        string toAddresses,
        IReadOnlyList<LccQrqcTicketDto> tickets,
        string? note = null)
    {
        var (smtpClient, from, fromName) = BuildSmtpClient();
        var recipients = ParseRecipients(toAddresses);

        var subject = $"[LCC] QRQC Tickets Report · {DateTime.Today:dd MMM yyyy} · {tickets.Count} Tickets";
        var html    = BuildQrqcHtml(tickets, note);

        using var mail = new MailMessage { From = new MailAddress(from, fromName), Subject = subject, Body = html, IsBodyHtml = true };
        foreach (var r in recipients) mail.To.Add(r);
        await smtpClient.SendMailAsync(mail);
        smtpClient.Dispose();
    }

    private static string BuildQrqcHtml(IReadOnlyList<LccQrqcTicketDto> tickets, string? note)
    {
        const string accent = "#00d4ff";
        var sb = new StringBuilder();
        AppendHtmlHead(sb, accent);

        var openCount   = tickets.Count(x => x.Status?.Equals("Open",   StringComparison.OrdinalIgnoreCase) == true);
        var closedCount = tickets.Count(x => x.Status?.Equals("Closed", StringComparison.OrdinalIgnoreCase) == true);
        var avgAging    = tickets.Count > 0 ? Math.Round(tickets.Average(x => x.AgingDays), 1) : 0;

        sb.Append($@"
<div class='header' style='border-left:4px solid {accent};'>
  <h1 style='color:{accent};'>QRQC TICKETS REPORT</h1>
  <div class='sub'>CURRENT WEEK · {tickets.Count} TICKETS &nbsp;·&nbsp; Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>
</div>");

        if (!string.IsNullOrWhiteSpace(note))
            sb.Append($"<div class='note-box'><strong>📝 Note:</strong><br>{WebUtility.HtmlEncode(note)}</div>");

        sb.Append($@"
<div class='card'>
  <h2>KPI Summary</h2>
  <div class='kpi-grid'>
    <div class='kpi' style='border-left-color:{accent};'>
      <div class='kpi-label'>TOTAL TICKETS</div>
      <div class='kpi-value' style='color:{accent};'>{tickets.Count}</div>
    </div>
    <div class='kpi' style='border-left-color:#dc2626;'>
      <div class='kpi-label'>OPEN</div>
      <div class='kpi-value' style='color:{(openCount > 0 ? "#dc2626" : "#16a34a")};'>{openCount}</div>
    </div>
    <div class='kpi' style='border-left-color:#16a34a;'>
      <div class='kpi-label'>CLOSED</div>
      <div class='kpi-value' style='color:#16a34a;'>{closedCount}</div>
    </div>
    <div class='kpi' style='border-left-color:#d97706;'>
      <div class='kpi-label'>AVG AGING DAYS</div>
      <div class='kpi-value' style='color:{(avgAging > 7 ? "#dc2626" : avgAging > 3 ? "#d97706" : "#16a34a")};'>{avgAging}</div>
    </div>
  </div>
</div>");

        sb.Append($@"
<div class='card'>
  <h2>Ticket Details ({tickets.Count})</h2>
  <table>
    <thead><tr>
      <th>Ticket ID</th><th>QRAP ID</th><th>Created</th><th>Customer</th>
      <th>Process</th><th>Bay</th><th>Symptom</th><th>Status</th><th>Aging</th>
    </tr></thead><tbody>");

        foreach (var r in tickets.OrderByDescending(x => x.CreationDate))
        {
            var agingColor = r.AgingDays > 14 ? "#dc2626" : r.AgingDays > 7 ? "#d97706" : "#16a34a";
            sb.Append($@"
      <tr>
        <td><strong style='color:{accent};'>{r.TicketId}</strong></td>
        <td>{WebUtility.HtmlEncode(r.QrapId ?? "-")}</td>
        <td>{r.CreationDate:dd MMM yyyy}</td>
        <td>{WebUtility.HtmlEncode(r.Customer ?? "-")}</td>
        <td>{WebUtility.HtmlEncode(r.Process ?? "-")}</td>
        <td>{WebUtility.HtmlEncode(r.Bay ?? "-")}</td>
        <td>{WebUtility.HtmlEncode(r.Symptom ?? "-")}</td>
        <td><span style='color:{StatusColor(r.Status)};font-weight:600;'>{WebUtility.HtmlEncode(r.Status ?? "-")}</span></td>
        <td style='color:{agingColor};font-weight:600;'>{r.AgingDays}d</td>
      </tr>");
        }
        sb.Append("</tbody></table></div>");
        AppendHtmlFooter(sb);
        return sb.ToString();
    }

    // ════════════════════════════════════════════════════════════════════════
    // SHARED HELPERS
    // ════════════════════════════════════════════════════════════════════════

    private (SmtpClient client, string fromAddress, string fromName) BuildSmtpClient()
    {
        var section     = _config.GetSection("Feedback");
        var smtpHost    = section["SmtpHost"]     ?? throw new InvalidOperationException("Feedback:SmtpHost not configured.");
        var smtpPort    = int.Parse(section["SmtpPort"] ?? "25");
        var smtpUser    = section["SmtpUser"]     ?? string.Empty;
        var smtpPass    = section["SmtpPassword"] ?? string.Empty;
        var enableSsl   = bool.Parse(section["EnableSsl"] ?? "false");
        var fromAddress = section["FromAddress"]  ?? "lcc-backend-noreply@jabil.com";
        var fromName    = section["FromName"]     ?? "Line Control Center";

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl   = enableSsl,
            Credentials = string.IsNullOrEmpty(smtpUser)
                ? null
                : new NetworkCredential(smtpUser, smtpPass)
        };
        return (client, fromAddress, fromName);
    }

    private static string[] ParseRecipients(string toAddresses)
    {
        var recipients = toAddresses
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(e => e.Contains('@'))
            .ToArray();
        if (recipients.Length == 0)
            throw new InvalidOperationException("No valid recipient email addresses provided.");
        return recipients;
    }

    private static void AppendHtmlHead(StringBuilder sb, string accentColor)
    {
        sb.Append($@"<!DOCTYPE html><html><head><meta charset='utf-8'>
<style>
  body        {{ font-family:'Segoe UI',Arial,sans-serif; background:#f0f4f8; margin:0; padding:20px; }}
  .card       {{ background:#fff; border-radius:12px; padding:24px 28px; margin-bottom:20px; box-shadow:0 2px 8px rgba(0,0,0,0.08); }}
  .header     {{ background:linear-gradient(135deg,#0d1f3c,#0a3a5c); border-radius:12px; padding:24px 28px; margin-bottom:20px; color:#fff; }}
  h1          {{ margin:0 0 4px; font-size:20px; letter-spacing:2px; }}
  .sub        {{ color:#7ab3cc; font-size:12px; letter-spacing:1px; }}
  h2          {{ font-size:13px; letter-spacing:2px; color:#0a3a5c; margin:0 0 14px; text-transform:uppercase; border-bottom:2px solid {accentColor}; padding-bottom:6px; }}
  table       {{ width:100%; border-collapse:collapse; font-size:12px; }}
  th          {{ background:#0d1f3c; color:{accentColor}; text-align:left; padding:8px 10px; letter-spacing:1px; font-size:11px; }}
  td          {{ padding:7px 10px; border-bottom:1px solid #e8eef4; color:#334155; }}
  tr:hover td {{ background:#f8fafc; }}
  .kpi-grid   {{ display:grid; grid-template-columns:repeat(4,1fr); gap:14px; }}
  .kpi        {{ background:#f0f7ff; border-radius:8px; padding:14px 16px; border-left:4px solid {accentColor}; }}
  .kpi-label  {{ font-size:10px; color:#64748b; letter-spacing:1px; text-transform:uppercase; }}
  .kpi-value  {{ font-size:24px; font-weight:700; color:#0a3a5c; margin-top:4px; }}
  .note-box   {{ background:#fffbeb; border:1px solid #fcd34d; border-radius:8px; padding:12px 16px; font-size:12px; color:#92400e; margin-bottom:20px; }}
  .footer     {{ font-size:10px; color:#94a3b8; text-align:center; margin-top:20px; }}
</style></head><body>");
    }

    private static void AppendHtmlFooter(StringBuilder sb)
    {
        sb.Append("<div class='footer'>This report was generated by Line Control Center — Backend &nbsp;·&nbsp; Do not reply to this email.</div></body></html>");
    }

    private static string StatusColor(string? status) =>
        status?.ToUpperInvariant() switch
        {
            "OPEN"               => "#dc2626",
            "CLOSED"             => "#16a34a",
            "CLOSED COMPLETED"   => "#16a34a",
            "CLOSED-CANCELLATION"=> "#64748b",
            "IN PROGRESS"        => "#d97706",
            "PENDING APPROVAL"   => "#f59e0b",
            "COMPLETE"           => "#16a34a",
            "CANCELLED"          => "#64748b",
            _                    => "#334155"
        };
}
