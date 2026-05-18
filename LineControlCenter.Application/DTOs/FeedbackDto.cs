namespace LineControlCenter.Application.DTOs;

public class FeedbackDto
{
    public string Category        { get; set; } = string.Empty;
    public int    Rating          { get; set; }
    public string Message         { get; set; } = string.Empty;
    public string Page            { get; set; } = "Dashboard";
    public string SubmittedByNtId { get; set; } = "Anonymous";
    public string SubmittedByName { get; set; } = "Anonymous";
    public string SubmittedByEmail{ get; set; } = string.Empty;
}
