using MailKit.Net.Smtp;
using MimeKit;

namespace PruebaDeDesempeño.Web.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string fullName);
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string fullName)
    {
        var subject = "¡Bienvenido a Prueba de Desempeño!";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%); padding: 30px; text-align: center; color: white; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .content {{ padding: 30px; }}
        .content h2 {{ color: #333; }}
        .content p {{ color: #666; line-height: 1.6; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #6366f1; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ background: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 14px; }}
        .highlight {{ background: #f0f9ff; border-left: 4px solid #6366f1; padding: 15px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>¡Bienvenido!</h1>
        </div>
        <div class=""content"">
            <h2>Hola {fullName},</h2>
            <p>¡Gracias por registrarte en nuestro sistema de gestión!</p>
            
            <div class=""highlight"">
                <p><strong>Tu cuenta ha sido creada exitosamente.</strong></p>
                <p>Ahora puedes acceder a todas las funcionalidades del sistema.</p>
            </div>
            
            <p>Con tu cuenta podrás:</p>
            <ul>
                <li>Gestionar productos y stock</li>
                <li>Administrar clientes</li>
                <li>Realizar y consultar ventas</li>
                <li>Generar reportes y estadísticas</li>
                <li>Usar nuestro asistente virtual con IA</li>
            </ul>
            
            <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
            
            <p>¡Que tengas un excelente día!</p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Prueba de Desempeño. Todos los derechos reservados.</p>
            <p style=""color: #999; font-size: 12px;"">Este es un correo automático, por favor no responder.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, htmlBody);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
            var senderEmail = emailSettings["SenderEmail"];
            var senderName = emailSettings["SenderName"];
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.Auto);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado exitosamente a {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email a {Email}", toEmail);
            // No lanzar excepción para no interrumpir el flujo de registro
        }
    }
}
