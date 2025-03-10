using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace Mar_Azul_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DbContextEditorial _context;

        public LoginController(DbContextEditorial context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            Console.WriteLine($"📤 Solicitud de login recibida con Email: {loginRequest.Email}");

            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Clave))
            {
                Console.WriteLine("⚠️ Error: Faltan datos en la solicitud.");
                return BadRequest(new { message = "⚠️ Email y Clave son obligatorios." });
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (usuario == null)
            {
                Console.WriteLine("❌ Error: Usuario no encontrado en la base de datos.");
                return Unauthorized(new { message = "❌ Usuario o clave incorrectos." });
            }

            if (usuario.Clave != loginRequest.Clave)
            {
                Console.WriteLine("❌ Error: Clave incorrecta.");
                return Unauthorized(new { message = "❌ Usuario o clave incorrectos." });
            }

            Console.WriteLine($"✅ Usuario autenticado en API: {usuario.Email} con Rol: {usuario.Rol}");

            return new JsonResult(new
            {
                message = "✅ Login exitoso.",
                redirectUrl = usuario.Rol switch
                {
                    "Administrador" => "/Admin/Index",
                    "Autorizador" => "/Autorizador/Index",
                    "Escritor" => "/Escritor/Index",
                    _ => "/Usuarios/Login"
                },
                usuario = new
                {
                    usuario.IdUsuario,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.Rol,
                    usuario.Estado
                }
            })
            {
                StatusCode = 200,
                ContentType = "application/json"
            };
        }













        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "✅ Logout exitoso." });
        }
    }
}
