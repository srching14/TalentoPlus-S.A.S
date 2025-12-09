namespace PruebaDeDesempeño.API.DTOs;

public class ChatbotRequestDto
{
    public string Message { get; set; } = string.Empty;
}

public class ChatbotResponseDto
{
    public string Response { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
