using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoanRepository _loanRepository;

        public UserService(IUserRepository userRepository, ILoanRepository loanRepository)
        {
            _userRepository = userRepository;
            _loanRepository = loanRepository;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Mobile = user.Mobile,
                KycStatus = user.KycStatus,
                CreditScore = user.CreditScore,
                ProfileImage = user.ProfileImage,
                MonthlyIncome = user.MonthlyIncome,
                EmploymentType = user.EmploymentType
            };
        }

        public async Task<ApiResponseDto<UserResponseDto>> UpdateUserAsync(int userId, UpdateUserDto updateDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponseDto<UserResponseDto>.ErrorResponse("User not found");
                }

                // Update user properties
                if (!string.IsNullOrEmpty(updateDto.FullName))
                    user.FullName = updateDto.FullName;

                if (!string.IsNullOrEmpty(updateDto.Mobile))
                    user.Mobile = updateDto.Mobile;

                if (updateDto.MonthlyIncome.HasValue)
                    user.MonthlyIncome = updateDto.MonthlyIncome.Value;

                if (!string.IsNullOrEmpty(updateDto.EmploymentType))
                    user.EmploymentType = updateDto.EmploymentType;

                if (!string.IsNullOrEmpty(updateDto.ProfileImage))
                    user.ProfileImage = updateDto.ProfileImage;

                var updatedUser = await _userRepository.UpdateAsync(user);

                var userResponse = new UserResponseDto
                {
                    Id = updatedUser.Id,
                    FullName = updatedUser.FullName,
                    Email = updatedUser.Email,
                    Mobile = updatedUser.Mobile,
                    KycStatus = updatedUser.KycStatus,
                    CreditScore = updatedUser.CreditScore,
                    ProfileImage = updatedUser.ProfileImage,
                    MonthlyIncome = updatedUser.MonthlyIncome,
                    EmploymentType = updatedUser.EmploymentType
                };

                return ApiResponseDto<UserResponseDto>.SuccessResponse(userResponse, "User updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<UserResponseDto>.ErrorResponse("User update failed: " + ex.Message);
            }
        }

        public async Task<ApiResponseDto<object>> GetUserDashboardAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponseDto<object>.ErrorResponse("User not found");
                }

                var loans = await _loanRepository.GetLoansByUserIdAsync(userId);
                var activeLoans = loans.Where(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Approved).ToList();

                var totalPaid = loans.Sum(l => l.PaidAmount);
                var totalLoanAmount = loans.Sum(l => l.LoanAmount);
                var remainingAmount = totalLoanAmount - totalPaid;

                // Calculate loan health score (0-100)
                decimal loanHealthScore = 100;
                if (totalLoanAmount > 0)
                {
                    loanHealthScore = (totalPaid / totalLoanAmount) * 100;
                }

                var dashboard = new
                {
                    user = new UserResponseDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Mobile = user.Mobile,
                        KycStatus = user.KycStatus,
                        CreditScore = user.CreditScore,
                        ProfileImage = user.ProfileImage,
                        MonthlyIncome = user.MonthlyIncome,
                        EmploymentType = user.EmploymentType
                    },
                    activeLoans = activeLoans,
                    totalLoans = loans.Count,
                    totalPaid = totalPaid,
                    remainingAmount = remainingAmount,
                    loanHealthScore = Math.Round(loanHealthScore, 2)
                };

                return ApiResponseDto<object>.SuccessResponse(dashboard, "Dashboard data retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<object>.ErrorResponse("Failed to retrieve dashboard: " + ex.Message);
            }
        }
    }
}
