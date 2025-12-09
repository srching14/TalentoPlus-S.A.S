using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PruebaDeDesempeño.API.DTOs;
using PruebaDeDesempeño.Web.Services;

namespace PruebaDeDesempeño.API.Controllers;

[Route("api/chatbot")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatbotApiController : ControllerBase
{
    private readonly IChatbotService _chatbotService;
    private readonly ILogger<ChatbotApiController> _logger;

    public ChatbotApiController(IChatbotService chatbotService, ILogger<ChatbotApiController> logger)
    {
        _chatbotService = chatbotService;
        _logger = logger;
    }

    /// <summary>
    /// Envía una consulta al chatbot de IA
    /// </summary>
    [HttpPost("ask")]
    [ProducesResponseType(typeof(ApiResponse<ChatbotResponseDto>), 200)]
    public async Task<IActionResult> Ask([FromBody] ChatbotRequestDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest(ApiResponse<ChatbotResponseDto>.ErrorResponse("El mensaje no puede estar vacío"));
            }

            var response = await _chatbotService.ProcessMessageAsync(dto.Message);

            var chatbotResponse = new ChatbotResponseDto
            {
                Response = response,
                Timestamp = DateTime.UtcNow
            };

            return Ok(ApiResponse<ChatbotResponseDto>.SuccessResponse(chatbotResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en chatbot API");
            return StatusCode(500, ApiResponse<ChatbotResponseDto>.ErrorResponse("Error al procesar consulta"));
        }
    }
}
