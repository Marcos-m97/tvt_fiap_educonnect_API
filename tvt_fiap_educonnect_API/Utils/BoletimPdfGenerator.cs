using EduConnect_API.Models;
using QuestPDF.Fluent;

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

                    page.Header().Text($"Boletim Escolar").Bold().FontSize(20);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Aluno: {b.AlunoId}");
                        col.Item().Text($"Turma: {b.TurmaId}");
                        col.Item().Text($"Gerado em: {b.GeradoEm:dd/MM/yyyy}");

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.ConstantColumn(60);
                                c.ConstantColumn(80);
                                c.ConstantColumn(100);
                            });

                            t.Header(h =>
                            {
                                h.Cell().Text("Disciplina").Bold();
                                h.Cell().Text("Nota").Bold();
                                h.Cell().Text("Média").Bold();
                                h.Cell().Text("Situação").Bold();
                            });

                            foreach (var d in b.Disciplinas)
                            {
                                t.Cell().Text(d.NomeDisciplina);
                                t.Cell().Text(d.Nota.ToString("0.0"));
                                t.Cell().Text(d.Media.ToString("0.0"));
                                t.Cell().Text(d.Situacao);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text("EduConnect");
                });
            });

            return doc.GeneratePdf();
        }
    }
}
