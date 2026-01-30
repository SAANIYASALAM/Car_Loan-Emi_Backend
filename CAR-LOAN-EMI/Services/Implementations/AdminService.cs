using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserRepository _userRepository;

        public AdminService(ILoanRepository loanRepository, IUserRepository userRepository)
        {
            _loanRepository = loanRepository;
            _userRepository = userRepository;
        }

        public async Task<ApiResponseDto<List<object>>> GetPendingLoansAsync()
        {
            try
            {
                var loans = await _loanRepository.GetPendingLoansAsync();
                
                var loanData = loans.Select(l => new
                {
                    l.LoanId,
                    l.UserId,
                    UserName = l.User.FullName,
                    UserEmail = l.User.Email,
                    l.CarPrice,
                    l.LoanAmount,
                    CarType = l.CarType.ToString(),
                    l.InterestRate,
                    l.Tenure,
                    l.EmiAmount,
                    l.DownPaymentPercent,
                    l.ApplicationDate,
                    Status = l.Status.ToString()
                }).Cast<object>().ToList();

                return ApiResponseDto<List<object>>.SuccessResponse(loanData, "Pending loans retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<object>>.ErrorResponse($"Failed to retrieve pending loans: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<object>> ApproveLoanAsync(int loanId)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                if (loan == null)
                    return ApiResponseDto<object>.ErrorResponse("Loan not found");

                if (loan.Status != LoanStatus.Pending)
                    return ApiResponseDto<object>.ErrorResponse("Only pending loans can be approved");

                loan.Status = LoanStatus.Approved;
                loan.ApprovalDate = DateTime.UtcNow;
                loan.DisbursementDate = DateTime.UtcNow;
                
                await _loanRepository.UpdateAsync(loan);

                return ApiResponseDto<object>.SuccessResponse(new { 
                    loanId = loan.LoanId,
                    status = loan.Status.ToString(),
                    approvalDate = loan.ApprovalDate 
                }, "Loan approved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<object>.ErrorResponse($"Failed to approve loan: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<object>> RejectLoanAsync(int loanId, string reason)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                if (loan == null)
                    return ApiResponseDto<object>.ErrorResponse("Loan not found");

                if (loan.Status != LoanStatus.Pending)
                    return ApiResponseDto<object>.ErrorResponse("Only pending loans can be rejected");

                loan.Status = LoanStatus.Rejected;
                loan.RejectionReason = reason;
                
                await _loanRepository.UpdateAsync(loan);

                return ApiResponseDto<object>.SuccessResponse(new { 
                    loanId = loan.LoanId,
                    status = loan.Status.ToString(),
                    rejectionReason = loan.RejectionReason 
                }, "Loan rejected successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<object>.ErrorResponse($"Failed to reject loan: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<object>>> GetAllLoansAsync(LoanFilterDto? filter = null)
        {
            try
            {
                var loans = await _loanRepository.GetAllLoansAsync(filter);
                
                var loanData = loans.Select(l => new
                {
                    l.LoanId,
                    l.UserId,
                    UserName = l.User.FullName,
                    UserEmail = l.User.Email,
                    l.CarPrice,
                    l.LoanAmount,
                    CarType = l.CarType.ToString(),
                    l.InterestRate,
                    l.Tenure,
                    l.EmiAmount,
                    l.DownPaymentPercent,
                    l.ApplicationDate,
                    l.ApprovalDate,
                    l.DisbursementDate,
                    Status = l.Status.ToString(),
                    l.RejectionReason
                }).Cast<object>().ToList();

                return ApiResponseDto<List<object>>.SuccessResponse(loanData, "Loans retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<object>>.ErrorResponse($"Failed to retrieve loans: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<object>>> GetAllUsersAsync(UserFilterDto? filter = null)
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync(filter);
                
                var userData = users.Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Mobile,
                    u.KycStatus,
                    u.CreditScore,
                    u.MonthlyIncome,
                    u.EmploymentType,
                    u.IsActive,
                    u.Role,
                    u.CreatedAt,
                    LoansCount = u.Loans.Count
                }).Cast<object>().ToList();

                return ApiResponseDto<List<object>>.SuccessResponse(userData, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<object>>.ErrorResponse($"Failed to retrieve users: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<object>>> GetUserLoansAsync(int userId)
        {
            try
            {
                var loans = await _loanRepository.GetLoansByUserIdAsync(userId);
                
                var loanData = loans.Select(l => new
                {
                    l.LoanId,
                    l.CarPrice,
                    l.LoanAmount,
                    CarType = l.CarType.ToString(),
                    l.InterestRate,
                    l.Tenure,
                    l.EmiAmount,
                    l.ApplicationDate,
                    l.ApprovalDate,
                    Status = l.Status.ToString(),
                    l.RemainingEmis,
                    l.PaidAmount
                }).Cast<object>().ToList();

                return ApiResponseDto<List<object>>.SuccessResponse(loanData, "User loans retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<object>>.ErrorResponse($"Failed to retrieve user loans: {ex.Message}");
            }
        }
    }
}
