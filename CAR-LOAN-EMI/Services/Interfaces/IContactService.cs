using CAR_LOAN_EMI.Models.DTOs;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IContactService
    {
        Task<ApiResponseDto<object>> SubmitContactMessageAsync(ContactMessageDto dto);
        Task<ApiResponseDto<List<object>>> GetUserMessagesAsync(int userId);
        Task<ApiResponseDto<List<object>>> GetAllMessagesAsync(ContactFilterDto? filter = null);
        Task<ApiResponseDto<object>> RespondToMessageAsync(int messageId, string response);
    }
}
