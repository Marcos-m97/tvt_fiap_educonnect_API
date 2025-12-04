using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tvt_fiap_educonnect_API.Migrations
{
    /// <inheritdoc />
    public partial class AddEventoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    TurmaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TurmaDisciplinaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CriadoPorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventos_TurmaDisciplinas_TurmaDisciplinaId",
                        column: x => x.TurmaDisciplinaId,
                        principalTable: "TurmaDisciplinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Eventos_Turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Eventos_Usuarios_CriadoPorId",
                        column: x => x.CriadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_CriadoPorId",
                table: "Eventos",
                column: "CriadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_TurmaDisciplinaId",
                table: "Eventos",
                column: "TurmaDisciplinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_TurmaId",
                table: "Eventos",
                column: "TurmaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Eventos");
        }
    }
}
