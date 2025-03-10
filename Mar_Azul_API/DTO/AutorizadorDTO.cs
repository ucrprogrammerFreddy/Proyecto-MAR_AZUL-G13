namespace Mar_Azul_API.DTO
{
    public class AutorizadorDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public string Observaciones { get; set; }

    }
}
