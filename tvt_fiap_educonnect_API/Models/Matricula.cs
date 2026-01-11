using System;

namespace EduConnect_API.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        // FK → Aluno
        public int AlunoId { get; set; }
        public Aluno Aluno { get; set; }

        // FK → Turma
        public int TurmaId { get; set; }
        public Turma Turma { get; set; }

        // Etapas (1-inscrição, 2-pagamento, 3-documentos, 4-efetivada)
        public MatriculaStatus Status { get; set; } = MatriculaStatus.Inscricao;

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime? AtualizadoEm { get; set; }

        // Arquivos enviados nas etapas 2 e 3
        public string? ComprovantePagamento { get; set; }
        public string? DocumentosPessoais { get; set; }
        public string? DocumentosEscolaridade { get; set; }
    }
}