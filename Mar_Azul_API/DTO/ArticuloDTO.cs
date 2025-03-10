namespace Mar_Azul_API.Models
{
    // NO MODIFICAR
    public class ArticuloDTO
    {
        public int IdArticulo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Contenido { get; set; }
        public string UrlImagen { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int IdCategoria { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }  // Para ArticuloAutorizador
        public List<int> IdEtiquetas { get; set; }
        public List<int> IdArticulosRelacionados { get; set; }
        public List<int> IdAutores { get; set; }
        public List<int> IdAutorizadores { get; set; }
    }
}
