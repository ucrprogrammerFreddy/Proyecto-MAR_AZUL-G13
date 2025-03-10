using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mar_Azul_API.Models
{
    public class DbContextEditorial : DbContext
    {
        // Constructor que recibe opciones de configuración para la base de datos.
        public DbContextEditorial(DbContextOptions<DbContextEditorial> options) : base(options)
        {


        }


        public DbSet<Secciones> Secciones { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<Etiqueta> Etiquetas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Articulos> Articulos { get; set; }
        public DbSet<ArticuloEtiqueta> ArticuloEtiquetas { get; set; }
        public DbSet<ArticuloAutor> ArticuloAutores { get; set; }
        public DbSet<ArticuloRelacion> ArticuloRelacionados { get; set; }
        public DbSet<ArticuloAutorizador> ArticuloAutorizadores { get; set; }
        // Método para configurar relaciones, restricciones y datos semilla.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            // aqui se define las relaciones entre las tablas de muchos a muchos
            // ✅ Relación Sección -> Categoría
            base.OnModelCreating(modelBuilder);

            // ✅ Relación Sección -> Categoría
            modelBuilder.Entity<Categorias>()
                .HasOne(c => c.Seccion)
                .WithMany(s => s.Categoria)
                .HasForeignKey(c => c.IdSeccion);

            // ✅ Relación Artículo - Autor (Muchos a Muchos)
            modelBuilder.Entity<ArticuloAutor>()
                .HasKey(aa => new { aa.IdArticulo, aa.IdUsuario });

            modelBuilder.Entity<ArticuloAutor>()
                .HasOne(aa => aa.Articulo)
                .WithMany(a => a.ArticuloAutores)
                .HasForeignKey(aa => aa.IdArticulo);

            modelBuilder.Entity<ArticuloAutor>()
                .HasOne(aa => aa.Usuario)
                .WithMany(u => u.ArticuloAutores)
                .HasForeignKey(aa => aa.IdUsuario);

            // ✅ Relación Artículo - Etiqueta (Muchos a Muchos)
            modelBuilder.Entity<ArticuloEtiqueta>()
                .HasKey(ae => new { ae.IdArticulo, ae.IdEtiqueta });

            modelBuilder.Entity<ArticuloEtiqueta>()
                .HasOne(ae => ae.Articulo)
                .WithMany(a => a.ArticuloEtiquetas)
                .HasForeignKey(ae => ae.IdArticulo);

            modelBuilder.Entity<ArticuloEtiqueta>()
                .HasOne(ae => ae.Etiqueta)
                .WithMany(e => e.ArticuloEtiquetas)
                .HasForeignKey(ae => ae.IdEtiqueta);

            // ✅ Relación Artículo - Autorizador
            modelBuilder.Entity<ArticuloAutorizador>()
                .HasKey(aa => new { aa.IdArticulo, aa.IdUsuario });

            modelBuilder.Entity<ArticuloAutorizador>()
                .HasOne(aa => aa.Articulo)
                .WithMany(a => a.ArticuloAutorizadores)
                .HasForeignKey(aa => aa.IdArticulo);

            modelBuilder.Entity<ArticuloAutorizador>()
                .HasOne(aa => aa.Usuario)
                .WithMany(u => u.ArticuloAutorizadores)
                .HasForeignKey(aa => aa.IdUsuario);

            modelBuilder.Entity<ArticuloRelacion>()
    .HasKey(ar => new { ar.IdArticuloPrincipal, ar.IdArticuloRelacionado });

            modelBuilder.Entity<ArticuloRelacion>()
                .HasOne(ar => ar.ArticuloPrincipal)
                .WithMany(a => a.ArticulosRelacionados)  // Define una lista de artículos relacionados en Articulos
                .HasForeignKey(ar => ar.IdArticuloPrincipal)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ArticuloRelacion>()
                .HasOne(ar => ar.ArticuloRelacionado)
                .WithMany()  // No se define navegación inversa para evitar ciclos
                .HasForeignKey(ar => ar.IdArticuloRelacionado)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Articulos>()
           .HasOne(a => a.Categoria)
           .WithMany(c => c.Articulo)
           .HasForeignKey(a => a.IdCategoria)
           .OnDelete(DeleteBehavior.Cascade);


            // 🌱 **Datos Semilla**
            // 🔹 Primero las Secciones
            modelBuilder.Entity<Secciones>().HasData(
       new Secciones { IdSeccion = 1, Nombre = "Tecnología", Descripcion = "Sección de artículos sobre tecnología.", ImagenURL = "https://example.com/tecnologia.jpg", Estado = "A" },
       new Secciones { IdSeccion = 2, Nombre = "Ciencia", Descripcion = "Sección de artículos sobre ciencia.", ImagenURL = "https://example.com/ciencia.jpg", Estado = "I" }
   );

            // 🔹 Luego las Categorías (Después de las Secciones)
            modelBuilder.Entity<Categorias>().HasData(
                new Categorias { IdCategoria = 1, Nombre = "Software", Descripcion = "Artículos de software y programación.", UrlImagen = "/imagenes_categorias/software.jpg", Estado = 'A', IdSeccion = 1 },
                new Categorias { IdCategoria = 2, Nombre = "Astronomía", Descripcion = "Artículos sobre el espacio y el universo.", UrlImagen = "/imagenes_categorias/astronomia.jpg", Estado = 'A', IdSeccion = 2 }
            );

            // 🔹 Datos semilla para Etiquetas
            modelBuilder.Entity<Etiqueta>().HasData(
                new Etiqueta { IdEtiqueta = 1, Nombre = "Innovación", Estado = "A" },
                new Etiqueta { IdEtiqueta = 2, Nombre = "Ciencia", Estado = "A" }
            );

            // 🔹 Datos semilla para Usuarios
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { IdUsuario = 1, Nombre = "Admin", Email = "admin@example.com", Clave = "123456", Estado = "Activo", Rol = "Administrador", Restablecer = "" },
                new Usuario { IdUsuario = 2, Nombre = "Escritor1", Email = "escritor1@example.com", Clave = "123456", Estado = "Activo", Rol = "Escritor", Restablecer = "" },
                new Usuario { IdUsuario = 3, Nombre = "Autorizador1", Email = "autorizador1@example.com", Clave = "123456", Estado = "Activo", Rol = "Autorizador", Restablecer = "" }
            );


            modelBuilder.Entity<Articulos>().HasData(
      new Articulos
      {
          IdArticulo = 1,
          Nombre = "Artículo de Prueba 1",
          Descripcion = "Descripción del artículo de prueba 1",
          Contenido = "Contenido completo del artículo de prueba 1",
          UrlImagen = "",
          FechaPublicacion = new DateTime(2023, 1, 1),
          IdCategoria = 1,
          Estado = "Publicado"
      },
      new Articulos
      {
          IdArticulo = 2,
          Nombre = "Artículo de Prueba 2",
          Descripcion = "Descripción del artículo de prueba 2",
          Contenido = "Contenido completo del artículo de prueba 2",
          UrlImagen = "",
          FechaPublicacion = new DateTime(2023, 2, 1),
          IdCategoria = 2,
          Estado = "Publicado"
      }
  );

        } // Cierre del método OnModelCreating
    } // Cierre de la clase
} // Cierre del namespace
