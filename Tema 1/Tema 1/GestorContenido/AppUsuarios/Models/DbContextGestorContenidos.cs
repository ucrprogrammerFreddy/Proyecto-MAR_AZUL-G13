using Microsoft.EntityFrameworkCore;

namespace AppUsuarios.Models
{
    // Contexto de base de datos que gestiona la interacción con la base de datos mediante Entity Framework Core.
    public class DbContextGestionContenido : DbContext
    {
        // Constructor que recibe opciones de configuración para la base de datos.
        // Estas opciones son proporcionadas desde la configuración de la aplicación (en Program.cs).
        // Esto es esencial porque permite flexibilidad al definir el proveedor de base de datos (SQL Server, SQLite, etc.).
        //Es imprescindible, ya que sin esto, DbContext no sabe cómo conectarse a la base de datos.
        public DbContextGestionContenido(DbContextOptions<DbContextGestionContenido> options) : base(options)
        {
        }// Cierre del constructor

        // Representa la tabla de usuarios en la base de datos.
        // Esencial para permitir consultas y manipulaciones de datos en la tabla "Usuarios".
        // Es imprescindible, ya que sin esta propiedad, la aplicación no podría interactuar con la tabla.
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Articulos> Articulos { get; set; }
        public DbSet<Categorias>Categorias { get; set; }
        public DbSet<Etiquetas> Etiquetas { get; set; }
        public DbSet<Secciones> Secciones { get; set; }
        public DbSet<ArticuloAutor> ArticuloAutor { get; set; }


        // Método que se ejecuta al crear el modelo de la base de datos.
        // Se usa para configurar relaciones, restricciones y datos semilla.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            // relacion Articulo autor usuario
            modelBuilder.Entity<ArticuloAutor>()
                     .HasOne(dp => dp.Usuario)
                     .WithMany(p => p.ArticuloAutor)
                     .HasForeignKey(dp => dp.IdUsuario);


            // Relacion Articulo Autor - Articulos

            // Relación DetallePedido - Producto
            modelBuilder.Entity<ArticuloAutor>()
                .HasOne(dp => dp.Articulo)
                .WithMany(aa => aa.ArticuloAutor)
                .HasForeignKey(dp => dp.IdArticulo);

            // Relación Etiqueta Artículo
            modelBuilder.Entity<Etiquetas>()
                .HasOne(e => e.Articulo)
                .WithMany(a => a.Etiqueta)
                .HasForeignKey(e => e.IdArticulo);
               
            // Categoria -articulo
            modelBuilder.Entity<Articulos>()
                .HasOne(a => a.Categoria)
                .WithMany(c => c.Articulo)
                .HasForeignKey(a => a.IdCategoria);

            // relacion categoria seccion
            modelBuilder.Entity<Categorias>()
              .HasOne(c => c.Seccion)
              .WithMany(s => s.Categoria)
              .HasForeignKey(a => a.IdSeccion);






            // Agregamos datos iniciales a la tabla Usuarios para que existan al momento de crear la base de datos.
            // Esto es útil para pruebas o configuraciones iniciales.
            // Si la aplicación no necesita datos iniciales, esta parte del código puede omitirse.
            modelBuilder.Entity<Usuario>().HasData
            (new Usuario()
            {
                IdUsuario = 1,
                Nombre = "User",
                Email = "Useradmin@gmail.com",
                Clave = "abcdef", // Contraseña no segura (solo como ejemplo)
                Estado = "Activo",
                Rol = "Admin",
                Restablecer = "Realizado"
            }
            );
        }// Cierre del método OnModelCreating
    } // Cierre de la clase
} // Cierre del namespace
