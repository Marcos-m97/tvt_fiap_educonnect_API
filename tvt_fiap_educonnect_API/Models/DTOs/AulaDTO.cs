using System;

namespace EduConnect_API.Models.DTOs
{
    public class AulaDTO
    {
        public int Id { get; set; }

        public int TurmaDisciplinaId { get; set; }

        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Vídeo complementar (YouTube, Vimeo etc)
        public string UrlVideo { get; set; } = string.Empty;

        // Vídeo principal da aula (MP4 hospedado no servidor)
        public string? VideoAula { get; set; }

        // Material de apoio (PDF)
        public string? MaterialApoio { get; set; }

        public string? Observacoes { get; set; }

        public DateTime CriadoEm { get; set; }

        public string CriadoPor { get; set; } = string.Empty;
    }
}