using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tvt_fiap_educonnect_API.Migrations
{
    /// <inheritdoc />
    public partial class AddAtivoToDisciplina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Disciplinas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Disciplinas");
        }
    }
}
