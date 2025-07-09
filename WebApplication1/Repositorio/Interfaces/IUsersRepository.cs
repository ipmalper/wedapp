using WebApplication1.Models;

namespace WebApplication1.Repositorio.Interfaces
{
    public interface IUsersRepository
    {
        //aqui van los metodos
        Users? GetUser(int id);
        List<Users> GetUsers();
        Users AgregarUser(Users users);
        Users ActualizarUser(Users users);
        //Validaciones de Existencia
        bool EmailExiste(string email);
        bool TelefonoExiste(string telefono);
        //Borrado logico
        void BorrarUser(int id);
        void ActivarUser(int id);
        void DesactivarUser(int id);
        bool BloquearUser(int usuarioId, string editorUserName, out string mensaje);
        bool DesbloquearUser(int usuarioId, string editorUserName, out string mensaje);

    }
}

