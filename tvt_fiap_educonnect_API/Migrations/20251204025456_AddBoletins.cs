using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tvt_fiap_educonnect_API.Migrations
{
    /// <inheritdoc />
    public partial class AddBoletins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boletins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlunoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurmaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeradoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boletins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BoletinsDisciplinas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoletimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeDisciplina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nota = table.Column<double>(type: "float", nullable: false),
                    Media = table.Column<double>(type: "float", nullable: false),
                    Situacao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoletinsDisciplinas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoletinsDisciplinas_Boletins_BoletimId",
                        column: x => x.BoletimId,
                        principalTable: "Boletins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoletinsDisciplinas_BoletimId",
                table: "BoletinsDisciplinas",
                column: "BoletimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoletinsDisciplinas");

            migrationBuilder.DropTable(
                name: "Boletins");
        }
    }
}
