using EduConnect_API.Models.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EduConnect_API.Utils
{
    public static class BoletimPdfGenerator
    {
        public static byte[] GerarPdf(BoletimDTO b, string nomeAluno, string nomeTurma)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    // ================= HEADER =================
                    page.Header()
                        .AlignCenter()
                        .Text("Boletim Escolar")
                        .Bold()
                        .FontSize(22);

                    // ================= CONTENT =================
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Aluno: {nomeAluno}");
                        col.Item().Text($"Turma: {nomeTurma}");
                        col.Item().Text($"Gerado em: {b.GeradoEm:dd/MM/yyyy}");
                        col.Item().PaddingVertical(15);

                        foreach (var d in b.Disciplinas)
                        {
                            // -------- Disciplina --------
                            col.Item().PaddingBottom(5)
                               .Text(d.NomeDisciplina)
                               .Bold()
                               .FontSize(14);


                            // -------- Atividades --------
                            if (d.Atividades.Any())
                            {
                                col.Item().Table(t =>
                                {
                                    t.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn();   // Atividade
                                        c.ConstantColumn(80); // Nota
                                        c.ConstantColumn(80); // Status
                                    });

                                    t.Header(h =>
                                    {
                                        h.Cell().BorderBottom(1).Padding(5).Text("Atividade").Bold();
                                        h.Cell().BorderBottom(1).Padding(5).Text("Nota").Bold();
                                        h.Cell().BorderBottom(1).Padding(5).Text("Status").Bold();
                                    });

                                    foreach (var a in d.Atividades)
                                    {
                                        t.Cell().Padding(5).Text(a.Titulo);
                                        t.Cell().Padding(5).Text(
                                            a.Nota.HasValue ? a.Nota.Value.ToString("0.0") : "-"
                                        );
                                        t.Cell().Padding(5).Text(
                                            a.Entregue ? "Entregue" : "Pendente"
                                        );
                                    }
                                });
                            }
                            else
                            {
                                col.Item().Text("Nenhuma atividade cadastrada.")
                                    .Italic()
                                    .FontSize(10);
                            }

                            // -------- Resumo --------
                            col.Item().PaddingTop(5).Row(row =>
                            {
                                row.RelativeItem().Text($"Total de atividades: {d.TotalAtividades}");
                                row.RelativeItem().Text($"Média: {d.Media:0.0}");
                                row.RelativeItem().Text($"Situação: {d.Situacao}");
                            });

                            col.Item().PaddingBottom(15);
                        }
                    });

                    // ================= FOOTER =================
                    page.Footer()
                        .AlignCenter()
                        .Text("EduConnect")
                        .FontSize(10);
                });
            });

            return doc.GeneratePdf();
        }
    }
}
