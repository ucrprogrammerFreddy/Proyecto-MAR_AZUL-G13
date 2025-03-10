namespace AppUsuarios.DTOS
{
    // Clase que representa un artículo con detalles.
    public class ArticuloDetalleDTO
    {
        // Propiedades de la clase
        public int IdArticulo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Contenido { get; set; }
        public string UrlImagen { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }

        //  Campos Relacionados
        public string Categoria { get; set; }
        public string Autor { get; set; }
        public List<EtiquetaDTO> Etiquetas { get; set; } = new List<EtiquetaDTO>();
    }

    public class EtiquetaDTO
    {
        public int IdEtiqueta { get; set; }
        public string Nombre { get; set; }
    }

}
