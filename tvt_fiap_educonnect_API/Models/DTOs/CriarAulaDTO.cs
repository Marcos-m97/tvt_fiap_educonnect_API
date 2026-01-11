using System;

namespace EduConnect_API.Models.DTOs
{
    public class CriarAulaDTO
    {
        public int TurmaDisciplinaId { get; set; }

        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Link do vídeo (YouTube, Stream, etc.)
        public string UrlVideo { get; set; } = string.Empty;

        // Campo opcional
        public string? Observacoes { get; set; }
    }
}