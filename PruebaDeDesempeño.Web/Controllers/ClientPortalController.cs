using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.ViewModels;

namespace PruebaDeDesempeño.Web.Controllers;

[Authorize(Roles = "Cliente")]
public class ClientPortalController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<ClientPortalController> _logger;

    public ClientPortalController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<ClientPortalController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    // GET: ClientPortal
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ClientProfileViewModel
        {
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };

        return View(model);
    }

    // GET: ClientPortal/EditProfile
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ClientProfileViewModel
        {
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };

        return View(model);
    }

    // POST: ClientPortal/EditProfile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(ClientProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Restore read-only fields
        model.CreatedAt = user.CreatedAt;
        model.LastLoginAt = user.LastLoginAt;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Check if email is being changed
            bool emailChanged = !string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase);
            
            if (emailChanged)
            {
                // Check if new email is already taken
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está en uso.");
                    return View(model);
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                user.NormalizedEmail = model.Email.ToUpperInvariant();
                user.NormalizedUserName = model.Email.ToUpperInvariant();
            }

            // Update name
            user.FullName = model.FullName;

            // Handle password change if requested
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Debe ingresar su contraseña actual para cambiarla.");
                    return View(model);
                }

                var passwordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        if (error.Code == "PasswordMismatch")
                        {
                            ModelState.AddModelError("CurrentPassword", "La contraseña actual es incorrecta.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    return View(model);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("Cliente {Email} actualizó su perfil.", user.Email);
                
                // If email changed, sign out and redirect to login
                if (emailChanged)
                {
                    await _signInManager.SignOutAsync();
                    TempData["SuccessMessage"] = "Perfil actualizado. Por favor, inicia sesión con tu nuevo correo.";
                    return RedirectToAction("Login", "Account");
                }

                TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar perfil del cliente");
            ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el perfil.");
        }

        return View(model);
    }
}
