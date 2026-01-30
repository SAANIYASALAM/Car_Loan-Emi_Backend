using CAR_LOAN_EMI.Helpers;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return ApiResponseDto<AuthResponseDto>.ErrorResponse("User with this email already exists");
                }

                // Hash the password
                var passwordHash = PasswordHasher.HashPassword(request.Password);

                // Create new user
                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Mobile = request.Mobile,
                    PasswordHash = passwordHash,
                    MonthlyIncome = request.MonthlyIncome,
                    EmploymentType = request.EmploymentType,
                    CreditScore = request.CreditScore,
                    KycStatus = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var createdUser = await _userRepository.CreateAsync(user);

                // Generate JWT token
                var token = _jwtHelper.GenerateToken(createdUser);
                var expiresAt = DateTime.UtcNow.AddMinutes(1440);

                var authResponse = new AuthResponseDto
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    User = new UserResponseDto
                    {
                        Id = createdUser.Id,
                        FullName = createdUser.FullName,
                        Email = createdUser.Email,
                        Mobile = createdUser.Mobile,
                        KycStatus = createdUser.KycStatus,
                        CreditScore = createdUser.CreditScore,
                        ProfileImage = createdUser.ProfileImage,
                        MonthlyIncome = createdUser.MonthlyIncome,
                        EmploymentType = createdUser.EmploymentType
                    }
                };

                return ApiResponseDto<AuthResponseDto>.SuccessResponse(authResponse, "User registered successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<AuthResponseDto>.ErrorResponse("Registration failed: " + ex.Message);
            }
        }

        public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    return ApiResponseDto<AuthResponseDto>.ErrorResponse("Invalid email or password");
                }

                // Verify password
                if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return ApiResponseDto<AuthResponseDto>.ErrorResponse("Invalid email or password");
                }

                if (!user.IsActive)
                {
                    return ApiResponseDto<AuthResponseDto>.ErrorResponse("Account is inactive");
                }

                // Generate JWT token
                var token = _jwtHelper.GenerateToken(user);
                var expiresAt = DateTime.UtcNow.AddMinutes(1440);

                var authResponse = new AuthResponseDto
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    User = new UserResponseDto
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
                    }
                };

                return ApiResponseDto<AuthResponseDto>.SuccessResponse(authResponse, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<AuthResponseDto>.ErrorResponse("Login failed: " + ex.Message);
            }
        }
    }
}
