using WebApplication1.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repositorio.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IRefreshTokensRepository _repo;
        public AuthController(IRefreshTokensRepository repo)
        {
            _repo = repo;
        }


        // POST: /Account/Register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto register)
        {
            try
            {
                bool registrado = _repo.RegistroUsuario(register.Email, register.Password, register.UserName);
                return Ok(registrado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // POST: /Account/Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var usuario = _repo.LoginUsuario(login.Password, login.UserName);
            if (usuario == null) return Unauthorized("Credenciales incorrectas");

            return Ok(new
            {
                success = true,
                data = new
                {
                    token = usuario.Token,
                    expiresAt = usuario.ExpiresAt,
                    email = usuario.Email
                }
            });

        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            return Ok(new { message = "Logout exitoso" });
        }
    }
}
