using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tvt_fiap_educonnect_API.Migrations
{
    /// <inheritdoc />
    public partial class AddBoletimAtividades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoletimAtividades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoletimDisciplinaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtividadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nota = table.Column<double>(type: "float", nullable: true),
                    Entregue = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoletimAtividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoletimAtividades_BoletinsDisciplinas_BoletimDisciplinaId",
                        column: x => x.BoletimDisciplinaId,
                        principalTable: "BoletinsDisciplinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoletimAtividades_BoletimDisciplinaId",
                table: "BoletimAtividades",
                column: "BoletimDisciplinaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoletimAtividades");
        }
    }
}
