

namespace Mar_Azul_API.Models
{
    public class ArticuloRelacion
    {

        public int IdArticuloPrincipal { get; set; }
        public Articulos ArticuloPrincipal { get; set; }

        public int IdArticuloRelacionado { get; set; }
        public Articulos ArticuloRelacionado { get; set; }

    }
}
