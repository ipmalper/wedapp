using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public String Nombres { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public String ApellidoPaterno { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public String ApellidoMaterno { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public String PasswordHash { get; set; } = string.Empty;
        [Required]
        [Phone(ErrorMessage = "Número de teléfono no válido")]
        public String Telefono { get; set; } = string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
        public String Email { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
    }
}
