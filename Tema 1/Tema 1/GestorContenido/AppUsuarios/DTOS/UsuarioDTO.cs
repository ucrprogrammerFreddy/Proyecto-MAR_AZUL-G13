using System.ComponentModel.DataAnnotations;



//Este DTO contiene solo Email y Clave, eliminando datos que no son necesarios en el proceso de autenticación.
namespace AppUsuarios.DTOS
{


    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; } //  Estado del usuario
        public string Rol { get; set; } //  Rol del usuario
        public string Clave { get; set; }
    }


}
