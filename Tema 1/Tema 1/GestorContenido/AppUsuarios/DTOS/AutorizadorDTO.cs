
namespace AppUsuarios.DTOS
{
    //  Clase que representa un autorizador.
    public class AutorizadorDTO
    {
        // Propiedades de la clase
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public string Observaciones { get; set; }
    }
}
