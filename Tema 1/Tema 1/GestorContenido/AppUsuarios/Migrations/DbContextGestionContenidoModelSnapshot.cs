﻿// <auto-generated />
using AppUsuarios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AppUsuarios.Migrations
{
    [DbContext(typeof(DbContextGestionContenido))]
    partial class DbContextGestionContenidoModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AppUsuarios.Models.ArticuloAutor", b =>
                {
                    b.Property<int>("IdArticuloAutor")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdArticuloAutor"));

                    b.Property<int>("IdArticulo")
                        .HasColumnType("int");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.HasKey("IdArticuloAutor");

                    b.HasIndex("IdArticulo");

                    b.HasIndex("IdUsuario");

                    b.ToTable("ArticuloAutor");
                });

            modelBuilder.Entity("AppUsuarios.Models.Articulos", b =>
                {
                    b.Property<int>("IdArticulo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdArticulo"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("IdCategoria")
                        .HasColumnType("int");

                    b.Property<string>("ImagenURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdArticulo");

                    b.HasIndex("IdCategoria");

                    b.ToTable("Articulos");
                });

            modelBuilder.Entity("AppUsuarios.Models.Categorias", b =>
                {
                    b.Property<int>("IdCategoria")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCategoria"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(600)
                        .HasColumnType("nvarchar(600)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("IdSeccion")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UrlImagen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdCategoria");

                    b.HasIndex("IdSeccion");

                    b.ToTable("Categorias");
                });

            modelBuilder.Entity("AppUsuarios.Models.Etiquetas", b =>
                {
                    b.Property<int>("IdEtiqueta")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdEtiqueta"));

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("IdArticulo")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdEtiqueta");

                    b.HasIndex("IdArticulo");

                    b.ToTable("Etiquetas");
                });

            modelBuilder.Entity("AppUsuarios.Models.Secciones", b =>
                {
                    b.Property<int>("IdSeccion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdSeccion"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("ImagenURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdSeccion");

                    b.ToTable("Secciones");
                });

            modelBuilder.Entity("AppUsuarios.Models.Usuario", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUsuario"));

                    b.Property<string>("Clave")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Restablecer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rol")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdUsuario");

                    b.ToTable("Usuarios");

                    b.HasData(
                        new
                        {
                            IdUsuario = 1,
                            Clave = "abcdef",
                            Email = "Useradmin@gmail.com",
                            Estado = "Activo",
                            Nombre = "User",
                            Restablecer = "Realizado",
                            Rol = "Admin"
                        });
                });

            modelBuilder.Entity("AppUsuarios.Models.ArticuloAutor", b =>
                {
                    b.HasOne("AppUsuarios.Models.Articulos", "Articulo")
                        .WithMany("ArticuloAutor")
                        .HasForeignKey("IdArticulo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AppUsuarios.Models.Usuario", "Usuario")
                        .WithMany("ArticuloAutor")
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Articulo");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("AppUsuarios.Models.Articulos", b =>
                {
                    b.HasOne("AppUsuarios.Models.Categorias", "Categoria")
                        .WithMany("Articulo")
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");
                });

            modelBuilder.Entity("AppUsuarios.Models.Categorias", b =>
                {
                    b.HasOne("AppUsuarios.Models.Secciones", "Seccion")
                        .WithMany("Categoria")
                        .HasForeignKey("IdSeccion")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Seccion");
                });

            modelBuilder.Entity("AppUsuarios.Models.Etiquetas", b =>
                {
                    b.HasOne("AppUsuarios.Models.Articulos", "Articulo")
                        .WithMany("Etiqueta")
                        .HasForeignKey("IdArticulo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Articulo");
                });

            modelBuilder.Entity("AppUsuarios.Models.Articulos", b =>
                {
                    b.Navigation("ArticuloAutor");

                    b.Navigation("Etiqueta");
                });

            modelBuilder.Entity("AppUsuarios.Models.Categorias", b =>
                {
                    b.Navigation("Articulo");
                });

            modelBuilder.Entity("AppUsuarios.Models.Secciones", b =>
                {
                    b.Navigation("Categoria");
                });

            modelBuilder.Entity("AppUsuarios.Models.Usuario", b =>
                {
                    b.Navigation("ArticuloAutor");
                });
#pragma warning restore 612, 618
        }
    }
}
