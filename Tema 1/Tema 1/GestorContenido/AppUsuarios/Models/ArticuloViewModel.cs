using AppUsuarios.Models;
using System.ComponentModel.DataAnnotations;


namespace AppUsuarios.Models
{

    public class ArticuloViewModel
    {
        // Quitar [Required] en UrlImagen
        public int IdArticulo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public string Contenido { get; set; }

        // Se retira [Required] para evitar el error de validación
        // si no se ha asignado la ruta antes de subir la imagen.
        public string UrlImagen { get; set; } = "/img/default.jpg";

        [DataType(DataType.Date)]
        public DateTime FechaPublicacion { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public int IdCategoria { get; set; }

        public string Estado { get; set; } = "Borrador";
        public string Observaciones { get; set; }

        // Estas listas guardarán los IDs seleccionados en los <select multiple> de la vista
        public List<int> SelectedEtiquetas { get; set; } = new List<int>();
        public List<int> SelectedArticulosRelacionados { get; set; } = new List<int>();
        public List<int> SelectedAutores { get; set; } = new List<int>();
        public List<int> SelectedAutorizadores { get; set; } = new List<int>();

        // Estas listas se usan para rellenar los dropdowns (select) en la vista.
        // Se obtienen desde la API en la acción GET Create del controlador.
        public List<Categorias> Categorias { get; set; } = new List<Categorias>();
        public List<Etiquetas> Etiquetas { get; set; } = new List<Etiquetas>();
        public List<Articulos> ArticulosRelacionadosList { get; set; } = new List<Articulos>();
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();       // Para los Autores
        public List<Usuario> Autorizadores { get; set; } = new List<Usuario>();  // Para los Autorizadores


    }
}