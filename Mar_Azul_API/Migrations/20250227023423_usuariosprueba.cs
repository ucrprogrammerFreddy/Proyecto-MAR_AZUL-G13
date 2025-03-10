using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mar_Azul_API.Migrations
{
    /// <inheritdoc />
    public partial class usuariosprueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "IdUsuario",
                keyValue: 2,
                columns: new[] { "Email", "Nombre", "Rol" },
                values: new object[] { "escritor1@example.com", "Escritor1", "Escritor" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "IdUsuario", "Clave", "Email", "EsAutor", "EsAutorizador", "Estado", "Nombre", "Restablecer", "Rol" },
                values: new object[] { 3, "123456", "autorizador1@example.com", false, false, "Activo", "Autorizador1", "", "Autorizador" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "IdUsuario",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "IdUsuario",
                keyValue: 2,
                columns: new[] { "Email", "Nombre", "Rol" },
                values: new object[] { "user1@example.com", "Usuario1", "Usuario" });
        }
    }
}
