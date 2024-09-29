using Microsoft.AspNetCore.Mvc;
using test_jwt.Models;
using test_jwt.Services;

namespace test_jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        // Almacenamiento de usuarios en memoria
        private readonly Dictionary<string, string> _usuarios = new Dictionary<string, string>
        {
            { "usuario1", "password1" },
            { "usuario2", "password2" },
            { "string", "string" }
        };

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (_usuarios.ContainsKey(request.Username) && _usuarios[request.Username] == request.Password)
            {
                var token = _jwtService.GenerateSecurityToken(request.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Mensaje = "Credenciales inválidas." });
        }
    }
}
