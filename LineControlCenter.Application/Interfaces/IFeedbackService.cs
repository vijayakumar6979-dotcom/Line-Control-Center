using LineControlCenter.Application.DTOs;

namespace LineControlCenter.Application.Interfaces;

public interface IFeedbackService
{
    Task SendFeedbackAsync(FeedbackDto feedback);
}
