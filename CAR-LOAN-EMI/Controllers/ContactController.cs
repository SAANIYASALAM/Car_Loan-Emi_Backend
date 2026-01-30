using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Submit a contact message
        /// </summary>
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitContactMessage([FromBody] ContactMessageDto dto)
        {
            var result = await _contactService.SubmitContactMessageAsync(dto);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get all messages for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserMessages(int userId)
        {
            var result = await _contactService.GetUserMessagesAsync(userId);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Get all contact messages with optional filtering (Admin only)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllMessages([FromQuery] ContactFilterDto? filter)
        {
            var result = await _contactService.GetAllMessagesAsync(filter);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        /// <summary>
        /// Respond to a contact message (Admin only)
        /// </summary>
        [HttpPut("{messageId}/respond")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RespondToMessage(int messageId, [FromBody] string response)
        {
            var result = await _contactService.RespondToMessageAsync(messageId, response);
            
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }
    }
}
