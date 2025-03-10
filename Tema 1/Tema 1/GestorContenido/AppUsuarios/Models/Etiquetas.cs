using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppUsuarios.Models
{
    public class Etiquetas
    {
        // Clave primaria de tipo entero.
        [Key]
        public int IdEtiqueta { get; set; }

        // Nombre de la etiqueta, de tipo cadena (string). Obligatorio con longitud máxima de 100 caracteres.
        [Required(ErrorMessage = "El nombre es obligatorio.")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto.
        public string Nombre { get; set; }

        [JsonIgnore]
        public Articulos? Articulo { get; set; }
        [JsonIgnore]
        public int? IdArticulo { get; set; }
        // Estado de la etiqueta, de tipo carácter (char). Obligatorio y debe ser un carácter en mayúscula.
        [Required(ErrorMessage = "El estado es obligatorio.")] // Valida que el campo no esté vacío.
        [RegularExpression("[A-Z]", ErrorMessage = "El estado debe ser un único carácter en mayúscula.")] // Valida el patrón de entrada.
        [DataType(DataType.Text)] // Especifica que es un texto (aunque es un char, puede tratarse como texto).
        public string Estado { get; set; }



        [JsonIgnore]
        public ICollection<ArticuloEtiqueta> ArticuloEtiqueta { get; set; }
    }
}

