using System;
using EduConnect_API.Models;

namespace EduConnect_API.Models
{
    public class Aula
    {
        public int Id { get; set; }

        // ============================
        // RELACIONAMENTO
        // ============================
        public int TurmaDisciplinaId { get; set; }
        public TurmaDisciplina TurmaDisciplina { get; set; } = null!;

        // ============================
        // DADOS DA AULA
        // ============================
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Vídeo complementar (YouTube, Stream, Drive etc.)
        public string UrlVideo { get; set; } = string.Empty;

        // Vídeo principal da aula (MP4 salvo no servidor)
        public string? VideoAula { get; set; }

        // Caminho do material de apoio (PDF)
        public string? MaterialApoio { get; set; }

        // Campo livre (opcional)
        public string? Observacoes { get; set; }

        // ============================
        // CONTROLE
        // ============================
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Para auditoria simples (Professor/Admin)
        public string CriadoPor { get; set; } = string.Empty;
    }
}