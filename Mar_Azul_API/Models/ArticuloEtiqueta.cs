
using Mar_Azul_API.Models;

namespace Mar_Azul_API.Models
{
    public class ArticuloEtiqueta
    {
        public int IdArticulo { get; set; }
        public Articulos Articulo { get; set; }

        public int IdEtiqueta { get; set; }
        public Etiqueta Etiqueta { get; set; }


    }
}
