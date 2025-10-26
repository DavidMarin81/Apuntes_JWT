using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prueba02JWT.Dtos;
using Prueba02JWT.Services;
using System.Security.Claims;

namespace Prueba02JWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
            if (user == null)
                return BadRequest(new { message = "El email ya está en uso o el rol no existe." });

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
                return Unauthorized(new { message = "Credenciales incorrectas." });

            // Configurar cookie httpOnly
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, // permite cross-origin -> Cambiarlo a Strict en producción
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            return Ok(new { token });
        }

        [HttpGet("protected")]
        [Authorize] // Este endpoint requiere JWT
        public IActionResult ProtectedEndpoint()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new { message = "Acceso concedido", email, role });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { email, role });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Borra la cookie JWT
            Response.Cookies.Append("jwt", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1) // Expira inmediatamente
            });

            return Ok(new { message = "Sesión cerrada" });
        }

    }
}
