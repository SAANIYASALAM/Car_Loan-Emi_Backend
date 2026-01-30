using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IUserDashboardService _userDashboardService;
        private readonly IStatementService _statementService;
        private readonly IPaymentCalendarService _paymentCalendarService;

        public DashboardController(
            IUserDashboardService userDashboardService,
            IStatementService statementService,
            IPaymentCalendarService paymentCalendarService)
        {
            _userDashboardService = userDashboardService;
            _statementService = statementService;
            _paymentCalendarService = paymentCalendarService;
        }

        /// <summary>
        /// Get user dashboard data
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDashboard(int userId)
        {
            var result = await _userDashboardService.GetDashboardDataAsync(userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get statement data for a specific year
        /// </summary>
        [HttpGet("statement/{userId}/{year}")]
        public async Task<IActionResult> GetStatementData(int userId, int year)
        {
            var result = await _statementService.GetStatementDataAsync(userId, year);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Download PDF statement for a specific year
        /// </summary>
        [HttpGet("statement/{userId}/{year}/pdf")]
        public async Task<IActionResult> DownloadPdfStatement(int userId, int year)
        {
            var pdfBytes = await _statementService.GeneratePdfStatementAsync(userId, year);
            
            if (pdfBytes.Length == 0)
                return BadRequest("PDF generation not implemented");
            
            return File(pdfBytes, "application/pdf", $"Statement_{userId}_{year}.pdf");
        }

        /// <summary>
        /// Get payment calendar for a loan
        /// </summary>
        [HttpGet("payment-calendar/{loanId}")]
        public async Task<IActionResult> GetPaymentCalendar(int loanId)
        {
            var result = await _paymentCalendarService.GeneratePaymentCalendarAsync(loanId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }
    }
}
