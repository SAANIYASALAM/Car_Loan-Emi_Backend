using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForLoan([FromBody] LoanApplicationDto application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid request data"));
            }

            var result = await _loanService.ApplyForLoanAsync(application);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserLoans(int userId)
        {
            var loans = await _loanService.GetUserLoansAsync(userId);
            return Ok(ApiResponseDto<object>.SuccessResponse(loans, "Loans retrieved successfully"));
        }

        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoanDetails(int loanId)
        {
            var loan = await _loanService.GetLoanByIdAsync(loanId);
            
            if (loan == null)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("Loan not found"));
            }

            return Ok(ApiResponseDto<object>.SuccessResponse(loan, "Loan details retrieved successfully"));
        }

        [HttpPost("check-eligibility")]
        public async Task<IActionResult> CheckEligibility([FromBody] LoanApplicationDto application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid request data"));
            }

            var result = await _loanService.CheckEligibilityAsync(application);
            return Ok(result);
        }

        [HttpGet("rules/{carType}")]
        public IActionResult GetLoanRules(CarType carType)
        {
            var rules = _loanService.GetLoanRulesAsync(carType);
            return Ok(ApiResponseDto<LoanRuleDto>.SuccessResponse(rules, "Loan rules retrieved successfully"));
        }

        [HttpPost("calculate-emi")]
        public IActionResult CalculateEmi([FromBody] EmiCalculationDto calculation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid request data"));
            }

            var emi = _loanService.CalculateEmi(calculation);
            return Ok(ApiResponseDto<decimal>.SuccessResponse(emi, "EMI calculated successfully"));
        }
    }
}
