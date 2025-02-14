using Microsoft.EntityFrameworkCore;

namespace Mar_Azul_API.Models
{

    public class DbContextEditorial : DbContext
    {
        // Constructor que recibe opciones de configuración para la base de datos.
        // Estas opciones son proporcionadas desde la configuración de la aplicación (en Program.cs).
        // Esto es esencial porque permite flexibilidad al definir el proveedor de base de datos (SQL Server, SQLite, etc.).
        //Es imprescindible, ya que sin esto, DbContext no sabe cómo conectarse a la base de datos.


        public DbContextEditorial(DbContextOptions<DbContextEditorial> options) : base(options) {

        }// Cierre del constructor

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Etiqueta> Etiquetas { get; set; }
        public DbSet<Secciones> Secciones { get; set; }
        // Método que se ejecuta al crear el modelo de la base de datos.
        // Se usa para configurar relaciones, restricciones y datos semilla.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Agregamos datos iniciales a la tabla Etiqueetas para que existan al momento de crear la base de datos.
            // Esto es útil para pruebas o configuraciones iniciales.
            // Si la aplicación no necesita datos iniciales, esta parte del código puede omitirse.
            modelBuilder.Entity<Etiqueta>().HasData
            (new Etiqueta()
            {
                IdEtiqueta=1,
                Nombre="Etiqueta nueva",
                Estado = 'A'       
            }
            );
        }// Cierre del método OnModelCreating
    } // Cierre de la clase
} // Cierre del namespace 
