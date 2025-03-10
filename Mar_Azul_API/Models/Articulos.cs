using Mar_Azul_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using Mar_Azul_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Articulos
{
    [Key]
    public int IdArticulo { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string Contenido { get; set; }
    public string UrlImagen { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public int IdCategoria { get; set; }
    public string Estado { get; set; }


    // 🔹 Agregar la propiedad de navegación correctamente
    [ForeignKey("IdCategoria")]
    public Categorias Categoria { get; set; } // ✅ Propiedad de navegación correcta

    // Relaciones existentes
    public ICollection<ArticuloAutor> ArticuloAutores { get; set; } = new List<ArticuloAutor>();
    public ICollection<ArticuloEtiqueta> ArticuloEtiquetas { get; set; } = new List<ArticuloEtiqueta>();
    public ICollection<ArticuloRelacion> ArticulosRelacionados { get; set; } = new List<ArticuloRelacion>();
    public ICollection<ArticuloAutorizador> ArticuloAutorizadores { get; set; } = new List<ArticuloAutorizador>();
}
