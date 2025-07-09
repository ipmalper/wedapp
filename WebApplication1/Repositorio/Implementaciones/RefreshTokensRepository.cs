using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositorio.Interfaces;

namespace WebApplication1.Repositorio.Implementaciones
{
    public class RefreshTokensRepository : IRefreshTokensRepository
    {
        private readonly IDbConnection _conexion;
        private readonly IConfiguration _config;
        public RefreshTokensRepository(IConfiguration configuration)
        {
            _config = configuration;
            _conexion = new SqlConnection(_config.GetConnectionString("ConexionSQLAzure"));
        }
        public bool ExisteUsuario(string email, string userName)
        {
            var count = _conexion.ExecuteScalar<int>(
                "spExisteUsuario",
                new { Email = email, UserName = userName },
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }

        public string GenerarJwtToken(RefreshTokens usuario)
        {
            var claims = new[]
         {
            new Claim("UserId", usuario.Id.ToString()),
            new Claim("UserName", usuario.UserName ?? string.Empty),
            new Claim("Email", usuario.Email ?? string.Empty)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims:claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshTokens? LoginUsuario(string password, string userName)
        {
            // ✅ 1. Trae al usuario solo por su UserName
            var refreshToken = _conexion.QueryFirstOrDefault<RefreshTokens>(
                "spLoginUsuario", 
                new { UserName = userName },
                commandType: CommandType.StoredProcedure 
            );

            // ❌ Si no se encontró el usuario
            if (refreshToken == null)
                return null;

            // ✅ 2. Verifica la contraseña hasheada con BCrypt
            bool isValid = BCrypt.Net.BCrypt.Verify(password, refreshToken.Password);
            if (!isValid)
                return null;

            // ✅ 3. Genera token y actualiza si es válido
            refreshToken.Token = Guid.NewGuid().ToString();
            refreshToken.ExpiresAt = DateTime.Now.AddMinutes(30);

            _conexion.Execute(
                "spActualizarToken",
                new
                {
                    refreshToken.Id,
                    refreshToken.Token,
                    refreshToken.ExpiresAt
                },
                commandType: CommandType.StoredProcedure
            );

            return refreshToken;


        }


        public RefreshTokens? ObtenerPorToken(string token)
        {
            return _conexion.QueryFirstOrDefault<RefreshTokens>(
                "spObtenerPorToken",
                new { Token = token },
                commandType: CommandType.StoredProcedure
            );
        }

        public bool RegistroUsuario(string email, string password, string userName)
        {
            if (ExisteUsuario(email, userName))
            {
                throw new Exception("El correo electrónico o el nombre de usuario ya están en uso.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            int rowsAffected = _conexion.Execute(
                "spRegistroUsuario",
                new
                {
                    Email = email,
                    Password = passwordHash,
                    UserName = userName,
                    CreatedAt = DateTime.Now
                },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }
    }
}
