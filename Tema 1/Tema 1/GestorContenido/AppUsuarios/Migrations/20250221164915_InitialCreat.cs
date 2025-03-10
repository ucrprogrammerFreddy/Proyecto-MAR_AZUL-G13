using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppUsuarios.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Secciones",
                columns: table => new
                {
                    IdSeccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImagenURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(1)", nullable: false)
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
                    Restablecer = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    UrlImagen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    IdSeccion = table.Column<int>(type: "int", nullable: true),
                    SeccionIdSeccion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IdCategoria);
                    table.ForeignKey(
                        name: "FK_Categorias_Secciones_SeccionIdSeccion",
                        column: x => x.SeccionIdSeccion,
                        principalTable: "Secciones",
                        principalColumn: "IdSeccion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Articulos",
                columns: table => new
                {
                    IdArticulo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdUsuario = table.Column<int>(type: "int", nullable: true),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    CategoriaIdCategoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.IdArticulo);
                    table.ForeignKey(
                        name: "FK_Articulos_Categorias_CategoriaIdCategoria",
                        column: x => x.CategoriaIdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticuloAutor",
                columns: table => new
                {
                    IdArticulo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticuloIdArticulo = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloAutor", x => x.IdArticulo);
                    table.ForeignKey(
                        name: "FK_ArticuloAutor_Articulos_ArticuloIdArticulo",
                        column: x => x.ArticuloIdArticulo,
                        principalTable: "Articulos",
                        principalColumn: "IdArticulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticuloAutor_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Etiquetas",
                columns: table => new
                {
                    IdEtiqueta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArticuloIdArticulo = table.Column<int>(type: "int", nullable: true),
                    IdArticulo = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiquetas", x => x.IdEtiqueta);
                    table.ForeignKey(
                        name: "FK_Etiquetas_Articulos_ArticuloIdArticulo",
                        column: x => x.ArticuloIdArticulo,
                        principalTable: "Articulos",
                        principalColumn: "IdArticulo");
                });

            migrationBuilder.CreateTable(
                name: "ArticuloEtiqueta",
                columns: table => new
                {
                    IdArticulo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticuloIdArticulo = table.Column<int>(type: "int", nullable: false),
                    IdEtiqueta = table.Column<int>(type: "int", nullable: false),
                    EtiquetaIdEtiqueta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloEtiqueta", x => x.IdArticulo);
                    table.ForeignKey(
                        name: "FK_ArticuloEtiqueta_Articulos_ArticuloIdArticulo",
                        column: x => x.ArticuloIdArticulo,
                        principalTable: "Articulos",
                        principalColumn: "IdArticulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticuloEtiqueta_Etiquetas_EtiquetaIdEtiqueta",
                        column: x => x.EtiquetaIdEtiqueta,
                        principalTable: "Etiquetas",
                        principalColumn: "IdEtiqueta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "IdUsuario", "Clave", "Email", "Estado", "Nombre", "Restablecer", "Rol" },
                values: new object[] { 1, "abcdef", "Useradmin@gmail.com", "Activo", "User", "Realizado", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloAutor_ArticuloIdArticulo",
                table: "ArticuloAutor",
                column: "ArticuloIdArticulo");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloAutor_UsuarioIdUsuario",
                table: "ArticuloAutor",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloEtiqueta_ArticuloIdArticulo",
                table: "ArticuloEtiqueta",
                column: "ArticuloIdArticulo");

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloEtiqueta_EtiquetaIdEtiqueta",
                table: "ArticuloEtiqueta",
                column: "EtiquetaIdEtiqueta");

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_CategoriaIdCategoria",
                table: "Articulos",
                column: "CategoriaIdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_SeccionIdSeccion",
                table: "Categorias",
                column: "SeccionIdSeccion");

            migrationBuilder.CreateIndex(
                name: "IX_Etiquetas_ArticuloIdArticulo",
                table: "Etiquetas",
                column: "ArticuloIdArticulo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticuloAutor");

            migrationBuilder.DropTable(
                name: "ArticuloEtiqueta");

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
