using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<ApiResponseDto<object>> SubmitContactMessageAsync(ContactMessageDto dto)
        {
            try
            {
                var message = new ContactMessage
                {
                    UserId = dto.UserId,
                    Name = dto.Name,
                    Email = dto.Email,
                    Subject = dto.Subject,
                    Message = dto.Message,
                    Status = ContactStatus.New,
                    CreatedAt = DateTime.UtcNow
                };

                await _contactRepository.CreateAsync(message);

                return ApiResponseDto<object>.SuccessResponse(new
                {
                    message.MessageId,
                    Status = message.Status.ToString(),
                    message.CreatedAt
                }, "Message submitted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<object>.ErrorResponse($"Failed to submit message: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<object>>> GetUserMessagesAsync(int userId)
        {
            try
            {
                var messages = await _contactRepository.GetUserMessagesAsync(userId);
                
                var messageData = messages.Select(m => new
                {
                    m.MessageId,
                    m.Subject,
                    m.Message,
                    Status = m.Status.ToString(),
                    m.CreatedAt,
                    m.Response,
                    m.RespondedAt
                }).Cast<object>().ToList();

                return ApiResponseDto<List<object>>.SuccessResponse(messageData, "User messages retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<object>>.ErrorResponse($"Failed to retrieve user messages: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<object>>> GetAllMessagesAsync(ContactFilterDto? filter = null)
        {
            try
            {
                var messages = await _contactRepository.GetAllMessagesAsync(filter);
                
                var messageData = messages.Select(m => new
                {
                    m.MessageId,
                    m.UserId,
                    UserName = m.User?.FullName,
                    m.Name,
                    m.Email,
                    m.Subject,
                    m.Message,
                    Status = m.Status.ToString(),
                    m.CreatedAt,
                    m.Response,
                    m.RespondedAt
                }).Cast<object>().ToList();

                return ApiResponseDto<List<object>>.SuccessResponse(messageData, "Messages retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<object>>.ErrorResponse($"Failed to retrieve messages: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<object>> RespondToMessageAsync(int messageId, string response)
        {
            try
            {
                var message = await _contactRepository.GetByIdAsync(messageId);
                if (message == null)
                    return ApiResponseDto<object>.ErrorResponse("Message not found");

                message.Response = response;
                message.RespondedAt = DateTime.UtcNow;
                message.Status = ContactStatus.Resolved;

                await _contactRepository.UpdateAsync(message);

                return ApiResponseDto<object>.SuccessResponse(new
                {
                    messageId,
                    Status = message.Status.ToString(),
                    message.RespondedAt
                }, "Response sent successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<object>.ErrorResponse($"Failed to respond to message: {ex.Message}");
            }
        }
    }
}
