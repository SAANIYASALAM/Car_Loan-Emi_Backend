using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponseDto<UserResponseDto>.SuccessResponse(user, "User profile retrieved successfully"));
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UpdateUserDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid request data"));
            }

            var result = await _userService.UpdateUserAsync(userId, updateDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{userId}/dashboard")]
        public async Task<IActionResult> GetUserDashboard(int userId)
        {
            var result = await _userService.GetUserDashboardAsync(userId);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
