using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAnalyticsService _analyticsService;

        public AdminController(IAdminService adminService, IAnalyticsService analyticsService)
        {
            _adminService = adminService;
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Get all pending loan applications
        /// </summary>
        [HttpGet("loans/pending")]
        public async Task<IActionResult> GetPendingLoans()
        {
            var result = await _adminService.GetPendingLoansAsync();
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Approve a loan application
        /// </summary>
        [HttpPut("loans/{loanId}/approve")]
        public async Task<IActionResult> ApproveLoan(int loanId)
        {
            var result = await _adminService.ApproveLoanAsync(loanId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Reject a loan application with reason
        /// </summary>
        [HttpPut("loans/{loanId}/reject")]
        public async Task<IActionResult> RejectLoan(int loanId, [FromBody] RejectionReasonDto reason)
        {
            var result = await _adminService.RejectLoanAsync(loanId, reason.Reason);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get all loans with optional filtering
        /// </summary>
        [HttpGet("loans")]
        public async Task<IActionResult> GetAllLoans([FromQuery] LoanFilterDto? filter)
        {
            var result = await _adminService.GetAllLoansAsync(filter);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get monthly revenue analytics
        /// </summary>
        [HttpGet("analytics/monthly-revenue")]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int months = 12)
        {
            var result = await _analyticsService.GetMonthlyRevenueAsync(months);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get car type distribution analytics
        /// </summary>
        [HttpGet("analytics/car-type-distribution")]
        public async Task<IActionResult> GetCarTypeDistribution()
        {
            var result = await _analyticsService.GetCarTypeDistributionAsync();
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        [HttpGet("analytics/dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _analyticsService.GetDashboardStatsAsync();
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get all users with optional filtering
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterDto? filter)
        {
            var result = await _adminService.GetAllUsersAsync(filter);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get all loans for a specific user
        /// </summary>
        [HttpGet("users/{userId}/loans")]
        public async Task<IActionResult> GetUserLoans(int userId)
        {
            var result = await _adminService.GetUserLoansAsync(userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }
    }
}
