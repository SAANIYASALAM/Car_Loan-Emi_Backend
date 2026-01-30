using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmiController : ControllerBase
    {
        private readonly IEmiService _emiService;

        public EmiController(IEmiService emiService)
        {
            _emiService = emiService;
        }

        [HttpGet("loan/{loanId}")]
        public async Task<IActionResult> GetEmiPaymentsByLoan(int loanId)
        {
            var payments = await _emiService.GetEmiPaymentsByLoanIdAsync(loanId);
            return Ok(ApiResponseDto<object>.SuccessResponse(payments, "EMI payments retrieved successfully"));
        }

        [HttpGet("upcoming/{userId}")]
        public async Task<IActionResult> GetUpcomingPayments(int userId)
        {
            var payments = await _emiService.GetUpcomingPaymentsAsync(userId);
            return Ok(ApiResponseDto<object>.SuccessResponse(payments, "Upcoming payments retrieved successfully"));
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetPaymentHistory(int userId)
        {
            var payments = await _emiService.GetPaymentHistoryAsync(userId);
            return Ok(ApiResponseDto<object>.SuccessResponse(payments, "Payment history retrieved successfully"));
        }

        [HttpPost("pay")]
        public async Task<IActionResult> ProcessPayment([FromBody] EmiPaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid request data"));
            }

            var result = await _emiService.ProcessPaymentAsync(paymentDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
