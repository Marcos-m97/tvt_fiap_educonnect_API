using System;

namespace EduConnect_API.Models
{
    public class EntregaAtividade
    {
        public int Id { get; set; }

        // FK → Atividade
        public int AtividadeId { get; set; }
        public Atividade Atividade { get; set; }

        // FK → Aluno
        public int AlunoId { get; set; }
        public Aluno Aluno { get; set; }

        // Arquivo enviado (pode ser path ou Base64)
        public string? Arquivo { get; set; }

        // Data de envio
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

        // Nota atribuída pelo professor
        public decimal? Nota { get; set; }

        // Comentário do professor
        public string? FeedbackProfessor { get; set; }
    }
}