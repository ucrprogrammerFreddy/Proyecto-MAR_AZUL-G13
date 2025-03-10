namespace Mar_Azul_API.DTO
{
    public class CategoriaDTO
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; } // ✅ Agregada la descripción
        public string UrlImagen { get; set; } // ✅ Agregada la imagen
        public char Estado { get; set; } // ✅ Agregado el estado
        public int IdSeccion { get; set; } // ✅ Relación con la sección
    }
}

