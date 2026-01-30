using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KycController : ControllerBase
    {
        private readonly IKycService _kycService;

        public KycController(IKycService kycService)
        {
            _kycService = kycService;
        }

        /// <summary>
        /// Upload KYC document
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] KycDocumentUploadDto dto)
        {
            var result = await _kycService.UploadDocumentAsync(dto);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get all KYC documents for a user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserKycDocuments(int userId)
        {
            var result = await _kycService.GetUserKycDocumentsAsync(userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get KYC status for a user
        /// </summary>
        [HttpGet("status/{userId}")]
        public async Task<IActionResult> GetKycStatus(int userId)
        {
            var result = await _kycService.GetKycStatusAsync(userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Verify a KYC document (Admin only)
        /// </summary>
        [HttpPut("{documentId}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyDocument(int documentId)
        {
            var result = await _kycService.VerifyDocumentAsync(documentId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }
    }
}
