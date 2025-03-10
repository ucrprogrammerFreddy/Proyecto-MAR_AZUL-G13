using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AppUsuarios.Models
{
    public class Secciones
    {
        [Key]
        [JsonProperty("idSeccion")] // 🔹 Mapea el JSON `idSeccion` a `IdSeccion`
        public int IdSeccion { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [DataType(DataType.Text)]
        [JsonProperty("nombre")] // 🔹 Mapea el JSON `nombre` a `Nombre`
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres.")]
        [DataType(DataType.Text)]
        [JsonProperty("descripcion")] // 🔹 Mapea el JSON `descripcion` a `Descripcion`
        public string Descripcion { get; set; }

        [JsonProperty("imagenURL")] // 🔹 Mapea el JSON `imagenURL` a `ImagenURL`
        public string? ImagenURL { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [RegularExpression("[A-Z]", ErrorMessage = "El estado debe ser un único carácter en mayúscula.")]
        [DataType(DataType.Text)]
        [JsonProperty("estado")] // 🔹 Mapea el JSON `estado` a `Estado`
        public char Estado { get; set; }

        [JsonIgnore]
        [NotMapped]
        public IFormFile? ImagenFile { get; set; } // Archivo de imagen que subirá el usuario

        [JsonProperty("categoria")] // 🔹 Si la API devuelve categorías, asegúrate de mapearlas
        public ICollection<Categorias> Categoria { get; set; } = new List<Categorias>();
    }

}
