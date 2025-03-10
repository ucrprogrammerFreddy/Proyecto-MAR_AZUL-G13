namespace Mar_Azul_API.DTO
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; } // ✅ Estado del usuario
        public string Rol { get; set; } // ✅ Rol del usuario

        
    }


}
