using WebApplication1.Models;

namespace WebApplication1.Repositorio.Interfaces
{
    public interface IRefreshTokensRepository
    {
        // Registra un nuevo usuario en la base de datos
        // Ejemplo: RegistroUsuario("ana@email.com", "contraseña123");
        // Retorna true si fue exitoso, false si el email ya existe
        bool RegistroUsuario(string email, string password, string userName);

        // Verifica las credenciales para iniciar sesión
        // Ejemplo: LoginUsuario("ana@email.com", "contraseña123");
        // Retorna el objeto Usuario si las credenciales son válidas, null si no coinciden
        RefreshTokens? LoginUsuario(string password, string userName);

        // Verifica si un email ya está registrado (útil para evitar duplicados)
        // Ejemplo: bool existe = ExisteUsuario("ana@email.com");
        bool ExisteUsuario(string email, string userName);


        RefreshTokens? ObtenerPorToken(string token);
    }
}
