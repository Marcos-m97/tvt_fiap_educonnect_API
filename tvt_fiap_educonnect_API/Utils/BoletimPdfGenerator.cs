using EduConnect_API.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EduConnect_API.Utils
{
    public static class BoletimPdfGenerator
    {
        public static byte[] GerarPdf(Boletim b)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    page.Header()
                        .AlignCenter()
                        .Text("Boletim Escolar")
                        .Bold()
                        .FontSize(22);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Aluno: {b.AlunoId}");
                        col.Item().Text($"Turma: {b.TurmaId}");
                        col.Item().Text($"Gerado em: {b.GeradoEm:dd/MM/yyyy}");
                        col.Item().PaddingVertical(10);

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();   // Disciplina
                                c.ConstantColumn(60); // Atividades
                                c.ConstantColumn(70); // Soma Nota
                                c.ConstantColumn(60); // Média
                                c.ConstantColumn(80); // Situação
                            });

                            t.Header(h =>
                            {
                                h.Cell().BorderBottom(1).Padding(5).Text("Disciplina").Bold();
                                h.Cell().BorderBottom(1).Padding(5).Text("Ativ.").Bold();
                                h.Cell().BorderBottom(1).Padding(5).Text("Soma").Bold();
                                h.Cell().BorderBottom(1).Padding(5).Text("Média").Bold();
                                h.Cell().BorderBottom(1).Padding(5).Text("Situação").Bold();
                            });

                            foreach (var d in b.Disciplinas)
                            {
                                t.Cell().Padding(5).Text(d.NomeDisciplina);
                                t.Cell().Padding(5).Text(d.TotalAtividades.ToString());
                                t.Cell().Padding(5).Text(d.Nota.ToString("0.0"));
                                t.Cell().Padding(5).Text(d.Media.ToString("0.0"));
                                t.Cell().Padding(5).Text(d.Situacao);
                            }
                        });
                    });

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
