using WebApplication1.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repositorio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRefreshTokensRepository _repo;
        public AuthController(IRefreshTokensRepository repo)
        {
            _repo = repo;
        }


        // POST: /Account/Register
        [HttpPost]
        [Route("register")]
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
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var usuario = _repo.LoginUsuario(login.Password, login.UserName);
            if (usuario == null) return Unauthorized("Credenciales incorrectas");

            string jwt = _repo.GenerarJwtToken(usuario);

            return Ok(new
            {
                success = true,
                data = new
                {
                    token = jwt,
                    refreshToken = usuario.Token,
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
