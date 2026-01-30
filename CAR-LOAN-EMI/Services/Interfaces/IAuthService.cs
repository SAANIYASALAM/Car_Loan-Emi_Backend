using CAR_LOAN_EMI.Models.DTOs;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    }
}
