using CAR_LOAN_EMI.Models.DTOs;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IAdminService
    {
        // Loan Management
        Task<ApiResponseDto<List<object>>> GetPendingLoansAsync();
        Task<ApiResponseDto<object>> ApproveLoanAsync(int loanId);
        Task<ApiResponseDto<object>> RejectLoanAsync(int loanId, string reason);
        Task<ApiResponseDto<List<object>>> GetAllLoansAsync(LoanFilterDto? filter = null);
        
        // User Management
        Task<ApiResponseDto<List<object>>> GetAllUsersAsync(UserFilterDto? filter = null);
        Task<ApiResponseDto<List<object>>> GetUserLoansAsync(int userId);
    }
}
