using System.ComponentModel.DataAnnotations;

namespace Mar_Azul_API.Models
{
    public class Usuario
    {



        // Clave primaria de tipo entero.
        [Key]
        public int IdUsuario { get; set; }

        // Nombre del usuario, de tipo cadena (string). Obligatorio con longitud máxima de 100 caracteres.
        [Required(ErrorMessage = "El nombre es obligatorio.")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto.
        public string Nombre { get; set; }

        // Email del usuario, de tipo cadena (string). Obligatorio con formato válido y longitud máxima de 150 caracteres.
        [Required(ErrorMessage = "El email es obligatorio.")] // Valida que el campo no esté vacío.
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")] // Valida que tenga un formato de correo electrónico.
        [StringLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres.")] // Define la longitud máxima.
        public string Email { get; set; }

        // Clave del usuario, de tipo cadena (string). Obligatoria con longitud entre 6 y 50 caracteres.
        [Required(ErrorMessage = "La clave es obligatoria.")] // Valida que el campo no esté vacío.
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La clave debe tener entre 6 y 50 caracteres.")] // Define la longitud mínima y máxima.
        [DataType(DataType.Password)] // Especifica que es una contraseña.
        public string Clave { get; set; }

        // Estado del usuario, de tipo carácter (string). Obligatorio con longitud entre 6 y 50 caracteres.
        [Required(ErrorMessage = "El estado es obligatorio.")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "El estado no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto (aunque es un char, puede tratarse como texto).
        public string Estado { get; set; }

        // Rol del usuario, de tipo cadena (string). Obligatorio con longitud máxima de 100 caracteres.
        [Required(ErrorMessage = "El rol es obligatorio.")] // Valida que el campo no esté vacío.
        [StringLength(100, ErrorMessage = "El rol no puede exceder los 100 caracteres.")] // Define la longitud máxima.
        [DataType(DataType.Text)] // Especifica que es un texto.
        public string Rol { get; set; }

        public string Restablecer { get; set; }

        public bool EsAutor { get; set; }
        public bool EsAutorizador { get; set; }
        //Relaciones correctas
        public ICollection<ArticuloAutor> ArticuloAutores { get; set; } = new List<ArticuloAutor>();
        public ICollection<ArticuloAutorizador> ArticuloAutorizadores { get; set; } = new List<ArticuloAutorizador>();

    }
}
