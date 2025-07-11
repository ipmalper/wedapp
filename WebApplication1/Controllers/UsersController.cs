﻿using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Repositorio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Repositorio.Implementaciones;

namespace WebApplication1.Controllers
{
    
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IUsersRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _repo.GetUsers();
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(usersDto);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _repo.GetUser(id);
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUserDto dto)
        {
            if (_repo.TelefonoExiste(dto.Telefono))
                return BadRequest("Este número de teléfono ya está registrado.");

            if (_repo.EmailExiste(dto.Email))
                return BadRequest("Este correo ya está registrado.");

            var user = _mapper.Map<Users>(dto);
            _repo.AgregarUser(user);

            return Ok(new { mensaje = "Usuario creado correctamente" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (id != dto.UserId)
                return BadRequest("El ID no coincide.");

            var userExistente = _repo.GetUser(id);
            if (userExistente == null)
                return NotFound();

            var actualizado = _mapper.Map(dto, userExistente);
            _repo.ActualizarUser(actualizado);

            return Ok(new { mensaje = "Usuario actualizado correctamente" });
        }

        [HttpPut("activar/{id}")]
        public IActionResult Activar(int id)
        {
            _repo.ActivarUser(id);
            return Ok(new { mensaje = "Usuario activado" });
        }

        [HttpPut("desactivar/{id}")]
        public IActionResult Desactivar(int id)
        {
            _repo.DesactivarUser(id);
            return Ok(new { mensaje = "Usuario desactivado" });
        }

        [HttpDelete("{id}")]
        public IActionResult Borrar(int id)
        {
            var user = _repo.GetUser(id);
            if (user == null)
                return NotFound();

            _repo.DesactivarUser(id);
            return Ok(new { mensaje = "Usuario desactivado o eliminado" });
        }

        [HttpPost("{id}/lock")]
        [Authorize]
        public IActionResult LockUser(int id)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;


            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { message = "Usuario no identificado." });
            }

            string mensaje;
            var exito = _repo.BloquearUser(id, userName, out mensaje);

            if (!exito)
            {
                return Conflict(new { message = mensaje });
            }

            return Ok(new { message = mensaje });
        }

        [HttpPost("{id}/unlock")]
        [Authorize]
        public IActionResult UnlockUser(int id)
        {
            // Recuperar el nombre de usuario desde el token (claim)
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { message = "Usuario no identificado en el token." });
            }

            string mensaje;
            var exito = _repo.DesbloquearUser(id, userName, out mensaje);

            if (!exito)
            {
                return Conflict(new { message = mensaje });
            }

            return Ok(new { message = mensaje });
        }
    }
}
