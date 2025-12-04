using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tvt_fiap_educonnect_API.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalAtividades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalAtividades",
                table: "BoletinsDisciplinas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAtividades",
                table: "BoletinsDisciplinas");
        }
    }
}
