using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mar_Azul_API.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Etiquetas",
                columns: table => new
                {
                    IdEtiqueta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiquetas", x => x.IdEtiqueta);
                });

            migrationBuilder.CreateTable(
                name: "Secciones",
                columns: table => new
                {
                    IdSeccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImagenURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secciones", x => x.IdSeccion);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Restablecer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsAutor = table.Column<bool>(type: "bit", nullable: false),
                    EsAutorizador = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    UrlImagen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    IdSeccion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IdCategoria);
                    table.ForeignKey(
                        name: "FK_Categorias_Secciones_IdSeccion",
                        column: x => x.IdSeccion,
                        principalTable: "Secciones",
                        principalColumn: "IdSeccion");
                });

            migrationBuilder.CreateTable(
                name: "Articulos",
                columns: table => new
                {
                    IdArticulo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlImagen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.IdArticulo);
                    table.ForeignKey(
                        name: "FK_Articulos_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticuloAutores",
                columns: table => new
                {
                    IdArticulo = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloAutores", x => new { x.IdArticulo, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_ArticuloAutores_Articulos_IdArticulo",
                        column: x => x.IdArticulo,
                        principalTable: "Articulos",
                        principalColumn: "IdArticulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticuloAutores_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticuloAutorizadores",
                columns: table => new
                {
                    IdArticulo = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaAutorizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloAutorizadores", x => new { x.IdArticulo, x.IdUsuario });
                    table.ForeignKey(
                        name: "FK_ArticuloAutorizadores_Articulos_IdArticulo",
                        column: x => x.IdArticulo,
                        principalTable: "Articulos",
                        principalColumn: "IdArticulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticuloAutorizadores_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticuloEtiquetas",
                columns: table => new
                {
                    IdArticulo = table.Column<int>(type: "int", nullable: false),
                    IdEtiqueta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloEtiquetas", x => new { x.IdArticulo, x.IdEtiqueta });
                    table.ForeignKey(
                        name: "FK_ArticuloEtiquetas_Articulos_IdArticulo",
                        column: x => x.IdArticulo,
                        principalTable: "Articulos",
                        principalColumn: "IdArticulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticuloEtiquetas_Etiquetas_IdEtiqueta",
                        column: x => x.IdEtiqueta,
                        principalTable: "Etiquetas",
                        principalColumn: "IdEtiqueta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticuloRelacionados",
                columns: table => new
                {
                    IdArticuloPrincipal = table.Column<int>(type: "int", nullable: false),
                    IdArticuloRelacionado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloRelacionados", x => new { x.IdArticuloPrincipal, x.IdArticuloRelacionado });
                    table.ForeignKey(
                        name: "FK_ArticuloRelacionados_Articulos_IdArticuloPrincipal",
                        column: x => x.IdArticuloPrincipal,
                        principalTable: "Articulos",
                        principalColumn: "IdArticulo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticuloRelacionados_Articulos_IdArticuloRelacionado",
                        column: x => x.IdArticuloRelacionado,
                        principalTable: "Articulos",
                        principalColumn: "IdArticulo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Etiquetas",
                columns: new[] { "IdEtiqueta", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1, "A", "Innovación" },
                    { 2, "A", "Ciencia" }
                });

            migrationBuilder.InsertData(
                table: "Secciones",
                columns: new[] { "IdSeccion", "Descripcion", "Estado", "ImagenURL", "Nombre" },
                values: new object[,]
                {
                    { 1, "Sección de artículos sobre tecnología.", "A", "https://example.com/tecnologia.jpg", "Tecnología" },
                    { 2, "Sección de artículos sobre ciencia.", "I", "https://example.com/ciencia.jpg", "Ciencia" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "IdUsuario", "Clave", "Email", "EsAutor", "EsAutorizador", "Estado", "Nombre", "Restablecer", "Rol" },
                values: new object[,]
                {
                    { 1, "123456", "admin@example.com", false, false, "Activo", "Admin", "", "Administrador" },
                    { 2, "123456", "user1@example.com", false, false, "Activo", "Usuario1", "", "Usuario" }
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "IdCategoria", "Descripcion", "Estado", "IdSeccion", "Nombre", "UrlImagen" },
                values: new object[,]
                {
                    { 1, "Artículos de software y programación.", "A", 1, "Software", "/imagenes_categorias/software.jpg" },
                    { 2, "Artículos sobre el espacio y el universo.", "A", 2, "Astronomía", "/imagenes_categorias/astronomia.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Articulos",
                columns: new[] { "IdArticulo", "Contenido", "Descripcion", "Estado", "FechaPublicacion", "IdCategoria", "Nombre", "UrlImagen" },
                values: new object[,]
                {
                    { 1, "Contenido completo del artículo de prueba 1", "Descripción del artículo de prueba 1", "Publicado", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Artículo de Prueba 1", "" },
                    { 2, "Contenido completo del artículo de prueba 2", "Descripción del artículo de prueba 2", "Publicado", new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Artículo de Prueba 2", "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloAutores_IdUsuario",
                table: "ArticuloAutores",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloAutorizadores_IdUsuario",
                table: "ArticuloAutorizadores",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloEtiquetas_IdEtiqueta",
                table: "ArticuloEtiquetas",
                column: "IdEtiqueta");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloRelacionados_IdArticuloRelacionado",
                table: "ArticuloRelacionados",
                column: "IdArticuloRelacionado");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_IdCategoria",
                table: "Articulos",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_IdSeccion",
                table: "Categorias",
                column: "IdSeccion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticuloAutores");

            migrationBuilder.DropTable(
                name: "ArticuloAutorizadores");

            migrationBuilder.DropTable(
                name: "ArticuloEtiquetas");

            migrationBuilder.DropTable(
                name: "ArticuloRelacionados");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Etiquetas");

            migrationBuilder.DropTable(
                name: "Articulos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Secciones");
        }
    }
}
