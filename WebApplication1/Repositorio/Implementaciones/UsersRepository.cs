using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.Models;
using WebApplication1.Repositorio.Interfaces;
using Dapper;
using WebApplication1.Helpers;

namespace WebApplication1.Repositorio.Implementaciones
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IDbConnection _bd;

        public UsersRepository(IConfiguration configuration)
        {

            _bd = new SqlConnection(configuration.GetConnectionString("ConexionSQLAzure"));
        }
        public void ActivarUser(int id)
        {
            _bd.Execute("spActivarUser", new { UserId = id }, commandType: CommandType.StoredProcedure);
        }


        public Users ActualizarUser(Users users)
        {
            _bd.Execute("spActualizarUser", new
            {
                users.UserId,
                users.Nombres,
                users.ApellidoPaterno,
                users.ApellidoMaterno,
                users.Telefono,
                users.Email,
                users.PasswordHash,
                users.FechaCreacion,
                users.Activo
            }, commandType: CommandType.StoredProcedure);

            return users;
        }


        public Users AgregarUser(Users users)
        {
            users.FechaCreacion = DateTime.Now;
            users.PasswordHash = PasswordHelper.HashPassword(users.PasswordHash);

            var parametros = new DynamicParameters();
            parametros.Add("@Nombres", users.Nombres);
            parametros.Add("@ApellidoPaterno", users.ApellidoPaterno);
            parametros.Add("@ApellidoMaterno", users.ApellidoMaterno);
            parametros.Add("@Telefono", users.Telefono);
            parametros.Add("@Email", users.Email);
            parametros.Add("@PasswordHash", users.PasswordHash);
            parametros.Add("@FechaCreacion", users.FechaCreacion);
            parametros.Add("@Activo", users.Activo);
            parametros.Add("@NuevoId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            _bd.Execute("spAgregarUser", parametros, commandType: CommandType.StoredProcedure);

            users.UserId = parametros.Get<int>("@NuevoId");
            return users;
        }


        public void BorrarUser(int id)
        {
            _bd.Execute("spBorrarUser", new { UserId = id }, commandType: CommandType.StoredProcedure);
        }


        public void DesactivarUser(int id)
        {
            _bd.Execute("spDesactivarUser", new { UserId = id }, commandType: CommandType.StoredProcedure);
        }


        public bool EmailExiste(string email)
        {
            var count = _bd.ExecuteScalar<int>(
                "spEmailExiste",
                new { Email = email },
                commandType: CommandType.StoredProcedure
            );
            return count > 0;
        }


        public Users? GetUser(int id)
        {
            return _bd.QuerySingleOrDefault<Users>(
                "spObtenerUserPorId",
                new { UserId = id },
                commandType: CommandType.StoredProcedure
            );
        }


        public List<Users> GetUsers()
        {
            return _bd.Query<Users>("spGetUsers", commandType: CommandType.StoredProcedure).ToList();
        }


        public bool TelefonoExiste(string telefono)
        {
            var count = _bd.ExecuteScalar<int>(
                "spTelefonoExiste",
                new { Telefono = telefono },
                commandType: CommandType.StoredProcedure
            );
            return count > 0;
        }

    }
}
