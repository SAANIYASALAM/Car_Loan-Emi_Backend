using CAR_LOAN_EMI.Models.DTOs;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IStatementService
    {
        Task<ApiResponseDto<StatementDataDto>> GetStatementDataAsync(int userId, int year);
        Task<byte[]> GeneratePdfStatementAsync(int userId, int year);
    }
}
