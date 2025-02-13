using System.ComponentModel.DataAnnotations;



//Este DTO contiene solo Email y Clave, eliminando datos que no son necesarios en el proceso de autenticación.
namespace AppUsuarios.DTOs
{
    public class UsuarioDTO
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La clave debe tener entre 6 y 50 caracteres.")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }


    }
}
