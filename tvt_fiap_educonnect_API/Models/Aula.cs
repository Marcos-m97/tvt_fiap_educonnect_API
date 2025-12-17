using System;
using EduConnect_API.Models;

namespace EduConnect_API.Models
{
    public class Aula
    {
        public Guid Id { get; set; }

        // ============================
        // RELACIONAMENTO
        // ============================
        public Guid TurmaDisciplinaId { get; set; }
        public TurmaDisciplina TurmaDisciplina { get; set; } = null!;

        // ============================
        // DADOS DA AULA
        // ============================
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Link do vídeo (YouTube, Stream, Drive, etc.)
        public string UrlVideo { get; set; } = string.Empty;

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