using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Asignar rol de Cliente por defecto
                await _userManager.AddToRoleAsync(user, "Cliente");
                
                _logger.LogInformation("Nuevo usuario registrado: {Email}", model.Email);

                // Enviar email de bienvenida
                try
                {
                    await _emailService.SendWelcomeEmailAsync(model.Email, model.FullName);
                    _logger.LogInformation("Email de bienvenida enviado a {Email}", model.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error al enviar email de bienvenida a {Email}", model.Email);
                    // No interrumpir el flujo si falla el email
                }

                // Redirigir a login con mensaje
                TempData["SuccessMessage"] = "Registro exitoso. Revisa tu correo para confirmar tu cuenta.";
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
}
