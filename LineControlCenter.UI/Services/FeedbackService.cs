using System.Net;
using System.Net.Mail;
using System.Text;
using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LineControlCenter.UI.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IConfiguration _config;

    public FeedbackService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendFeedbackAsync(FeedbackDto feedback)
    {
        var section     = _config.GetSection("Feedback");
        var adminEmails = section.GetSection("AdminEmails").Get<string[]>();
        if (adminEmails is null || adminEmails.Length == 0)
            throw new InvalidOperationException("Feedback:AdminEmails is not configured.");
        var smtpHost    = section["SmtpHost"]     ?? throw new InvalidOperationException("Feedback:SmtpHost not configured.");
        var smtpPort    = int.Parse(section["SmtpPort"] ?? "587");
        var smtpUser    = section["SmtpUser"]     ?? string.Empty;
        var smtpPass    = section["SmtpPassword"] ?? string.Empty;
        var enableSsl   = bool.Parse(section["EnableSsl"] ?? "true");
        var fromAddress = section["FromAddress"]  ?? smtpUser;
        var fromName    = section["FromName"]     ?? "Line Control Center";

        var stars = new string('★', feedback.Rating) + new string('☆', 5 - feedback.Rating);

        var body = new StringBuilder();
        body.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        body.AppendLine("  LINE CONTROL CENTER — USER FEEDBACK");
        body.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        body.AppendLine();
        body.AppendLine($"  Submitted : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        //body.AppendLine($"  NT Login  : {feedback.SubmittedByNtId}");
        body.AppendLine($"  Name      : {feedback.SubmittedByName}");
        body.AppendLine($"  Email     : {(string.IsNullOrEmpty(feedback.SubmittedByEmail) ? "(not found)" : feedback.SubmittedByEmail)}");
        body.AppendLine($"  Page      : {feedback.Page}");
        body.AppendLine($"  Category  : {feedback.Category}");
        body.AppendLine($"  Rating    : {stars} ({feedback.Rating}/5)");
        body.AppendLine();
        body.AppendLine("  Message:");
        body.AppendLine("  " + feedback.Message.Replace("\n", "\n  "));
        body.AppendLine();
        body.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl   = enableSsl,
            Credentials = string.IsNullOrEmpty(smtpUser)
                ? null
                : new NetworkCredential(smtpUser, smtpPass)
        };

        var mail = new MailMessage
        {
            From       = new MailAddress(fromAddress, fromName),
            Subject    = $"[LCC-BACKEND Feedback] {feedback.Category} — {feedback.Rating}/5 ★",
            Body       = body.ToString(),
            IsBodyHtml = false
        };
        foreach (var email in adminEmails)
            mail.To.Add(email);

        await client.SendMailAsync(mail);
    }
}
