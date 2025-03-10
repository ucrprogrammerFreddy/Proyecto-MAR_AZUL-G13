namespace Mar_Azul_API.DTO
{
    public class ArticuloDetalleDTO
    {

        public int IdArticulo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Contenido { get; set; }
        public string UrlImagen { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string Estado { get; set; }

        // 🔹 Campos Relacionados
        public string Categoria { get; set; }  // Nombre de la categoría asociada
        public string Autor { get; set; }      // Nombre del autor asociado
        public List<EtiquetaDTO> Etiquetas { get; set; } = new List<EtiquetaDTO>();  // Lista de etiquetas asociadas
    }

    public class EtiquetaDTO
    {
        public int IdEtiqueta { get; set; }
        public string Nombre { get; set; }
    }
}
