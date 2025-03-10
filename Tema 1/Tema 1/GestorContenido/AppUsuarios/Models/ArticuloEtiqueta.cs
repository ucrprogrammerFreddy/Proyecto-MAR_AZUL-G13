using System.ComponentModel.DataAnnotations;

namespace AppUsuarios.Models
{
    public class ArticuloEtiqueta
    {

        [Key]
        public int IdArticulo { get; set; }
        public Articulos Articulo { get; set; }

        public int IdEtiqueta { get; set; }
        public Etiquetas Etiqueta { get; set; }

    }
}
