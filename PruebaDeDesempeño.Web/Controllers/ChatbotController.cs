using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaDeDesempeño.Web.Services;

namespace PruebaDeDesempeño.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class ChatbotController : Controller
{
    private readonly IChatbotService _chatbotService;
    private readonly ILogger<ChatbotController> _logger;

    public ChatbotController(IChatbotService chatbotService, ILogger<ChatbotController> logger)
    {
        _chatbotService = chatbotService;
        _logger = logger;
    }

    // GET: Chatbot
    public IActionResult Index()
    {
        return View();
    }

    // POST: Chatbot/SendMessage
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "El mensaje no puede estar vacío" });
        }

        try
        {
            var response = await _chatbotService.ProcessMessageAsync(request.Message);
            return Json(new { response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar mensaje del chatbot");
            return StatusCode(500, new { error = "Error al procesar el mensaje" });
        }
    }
}

public class ChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
}
