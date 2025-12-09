using System.Text;
using System.Text.Json;

namespace PruebaDeDesempeño.Web.Services;

/// <summary>
/// Servicio para interactuar con Google Gemini API
/// </summary>
public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GeminiService> _logger;

    private const string GeminiApiUrl =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent";

    public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration,
        ILogger<GeminiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                  ?? configuration["Gemini:ApiKey"]
                  ?? throw new InvalidOperationException("GEMINI_API_KEY no está configurada");
        _logger = logger;
    }

    /// <summary>
    /// Interpreta una pregunta en lenguaje natural y extrae el intent y parámetros
    /// </summary>
    public async Task<QueryIntent> InterpretQueryAsync(string question)
    {
        try
        {
            var prompt = $@"Eres un asistente que interpreta preguntas sobre empleados de una empresa.
Analiza la siguiente pregunta y devuelve ÚNICAMENTE un JSON con esta estructura exacta:
{{
  ""intent"": ""tipo de consulta (count_by_position, count_by_department, count_by_status, salary_stats, recent_hires, general_stats)"",
  ""parameters"": {{
    ""position"": ""cargo si se menciona"",
    ""department"": ""departamento si se menciona"",
    ""status"": ""estado si se menciona (Activo, Inactivo, Vacaciones, Licencia)"",
    ""timeframe"": ""marco temporal si se menciona""
  }}
}}

Reglas:
- Si pregunta por cantidad de un cargo específico → intent: count_by_position, parameters.position: nombre del cargo
- Si pregunta por departamento → intent: count_by_department, parameters.department: nombre del departamento  
- Si pregunta por estado → intent: count_by_status, parameters.status: estado
- Si pregunta estadísticas generales → intent: general_stats
- Si pregunta salarios → intent: salary_stats
- Si pregunta recién contratados → intent: recent_hires

Pregunta del usuario: ""{question}""

Responde SOLO con el JSON, sin texto adicional.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{GeminiApiUrl}?key={_apiKey}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error de Gemini API: {Error}", error);
                return new QueryIntent { Intent = "unknown", Parameters = new QueryParameters() };
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

            if (geminiResponse?.Candidates?.Length > 0)
            {
                var text = geminiResponse.Candidates[0]?.Content?.Parts?[0]?.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    // Limpiar el texto para extraer solo el JSON
                    var cleanedText = text.Trim();
                    if (cleanedText.StartsWith("```json"))
                    {
                        cleanedText = cleanedText.Substring(7);
                    }

                    if (cleanedText.EndsWith("```"))
                    {
                        cleanedText = cleanedText.Substring(0, cleanedText.Length - 3);
                    }

                    cleanedText = cleanedText.Trim();

                    var intent = JsonSerializer.Deserialize<QueryIntent>(cleanedText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (intent != null)
                    {
                        _logger.LogInformation("Intent interpretado: {Intent}", intent.Intent);
                        return intent;
                    }
                }
            }

            return new QueryIntent { Intent = "unknown", Parameters = new QueryParameters() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al interpretar query con Gemini");
            return new QueryIntent { Intent = "unknown", Parameters = new QueryParameters() };
        }
    }

    // Clases para deserialización
    private class GeminiResponse
    {
        public Candidate[]? Candidates { get; set; }
    }

    private class Candidate
    {
        public Content? Content { get; set; }
    }

    private class Content
    {
        public Part[]? Parts { get; set; }
    }

    private class Part
    {
        public string? Text { get; set; }
    }
}

/// <summary>
/// Representa el intent extraído de una pregunta
/// </summary>
public class QueryIntent
{
    public string Intent { get; set; } = "unknown";
    public QueryParameters Parameters { get; set; } = new();
}

/// <summary>
/// Parámetros extraídos de la pregunta
/// </summary>
public class QueryParameters
{
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? Status { get; set; }
    public string? Timeframe { get; set; }
}
