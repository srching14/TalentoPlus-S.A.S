using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.ViewModels;
using PruebaDeDesempeño.Web.Services;

namespace PruebaDeDesempeño.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Verificar rol - solo Administrador puede acceder al panel
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Administrador"))
                {
                    _logger.LogInformation("Usuario {Email} inició sesión correctamente.", model.Email);

                    // Actualizar último login
                    user.LastLoginAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    return RedirectToLocal(returnUrl ?? "/Dashboard/Index");
                }
                else if (roles.Contains("Cliente"))
                {
                    // Cliente - redirigir a portal de clientes
                    _logger.LogInformation("Cliente {Email} inició sesión correctamente.", model.Email);

                    user.LastLoginAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    return RedirectToAction("Index", "ClientPortal");
                }
                else
                {
                    // Usuario sin rol específico
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "No tiene un rol asignado. Contacte al administrador.");
                    return View(model);
                }
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Cuenta bloqueada: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente. Intente más tarde.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el inicio de sesión");
            ModelState.AddModelError(string.Empty, "Ocurrió un error durante el inicio de sesión. Intente nuevamente.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            var generatedPassword = GenerateRandomPassword();
            var result = await _userManager.CreateAsync(user, generatedPassword);

            if (result.Succeeded)
            {
                // Asignar rol de Cliente por defecto
                await _userManager.AddToRoleAsync(user, "Cliente");

                _logger.LogInformation("Nuevo usuario registrado: {Email}", model.Email);

                // Enviar email de bienvenida con la contraseña
                try
                {
                    var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%); padding: 30px; text-align: center; color: white; }}
        .content {{ padding: 30px; }}
        .credentials {{ background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #6366f1; }}
        .password {{ font-size: 24px; font-weight: bold; color: #6366f1; letter-spacing: 2px; }}
        .footer {{ background: #1e293b; color: white; padding: 20px; text-align: center; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>¡Bienvenido(a) a TalentoPlus!</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>{model.FullName}</strong>,</p>
            <p>Tu cuenta ha sido creada exitosamente.</p>
            
            <div class='credentials'>
                <h3>🔐 Tus credenciales de acceso:</h3>
                <p><strong>Email:</strong> {model.Email}</p>
                <p><strong>Contraseña:</strong> <span class='password'>{generatedPassword}</span></p>
            </div>
            
            <p><strong>⚠️ Importante:</strong> Guarda esta contraseña en un lugar seguro.</p>
            
            <p>Gracias por unirte a nuestro equipo.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.Now.Year} TalentoPlus S.A.S. | Todos los derechos reservados</p>
        </div>
    </div>
</body>
</html>";

                    await _emailService.SendEmailAsync(model.Email, "🎉 Bienvenido a TalentoPlus - Tus credenciales",
                        htmlBody);
                    _logger.LogInformation("Email de bienvenida enviado a {Email}", model.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al enviar email de bienvenida a {Email}", model.Email);
                    // No interrumpir el flujo si falla el email
                }

                // Redirigir a login con mensaje
                TempData["SuccessMessage"] = "Registro exitoso. Se ha enviado la contraseña a tu correo.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el registro");
            ModelState.AddModelError(string.Empty, "Ocurrió un error durante el registro. Intente nuevamente.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Usuario cerró sesión.");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard", new { area = "" });
    }

    private static string GenerateRandomPassword(int length = 10)
    {
        const string upperCase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lowerCase = "abcdefghjkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%";

        var random = new Random();
        var password = new System.Text.StringBuilder();

        // Asegurar al menos uno de cada tipo
        password.Append(upperCase[random.Next(upperCase.Length)]);
        password.Append(lowerCase[random.Next(lowerCase.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(special[random.Next(special.Length)]);

        // Completar el resto
        const string allChars = upperCase + lowerCase + digits;
        for (int i = 4; i < length; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        // Mezclar
        return new string(password.ToString().ToCharArray().OrderBy(_ => random.Next()).ToArray());
    }
}
