using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PruebaDeDesempeño.API.DTOs;
using PruebaDeDesempeño.Web.Models;
using PruebaDeDesempeño.Web.Services;

namespace PruebaDeDesempeño.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthApiController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthApiController> _logger;

    public AuthApiController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<AuthApiController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Login de usuario y generación de JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Credenciales inválidas"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Credenciales inválidas"));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles.ToList());

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList(),
                Expiration = DateTime.UtcNow.AddHours(24)
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login exitoso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login API");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("Error al iniciar sesión"));
        }
    }

    /// <summary>
    /// Registro de nuevo usuario
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("El email ya está registrado"));
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(
                    "Error al crear usuario",
                    result.Errors.Select(e => e.Description).ToList()));
            }

            // Asignar rol Cliente por defecto
            await _userManager.AddToRoleAsync(user, "Cliente");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles.ToList());

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList(),
                Expiration = DateTime.UtcNow.AddHours(24)
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Registro exitoso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en registro API");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("Error al registrar usuario"));
        }
    }
}
