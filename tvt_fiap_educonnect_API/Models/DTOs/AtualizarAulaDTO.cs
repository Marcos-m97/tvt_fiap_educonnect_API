namespace EduConnect_API.Models.DTOs
{
    public class AtualizarAulaDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? UrlVideo { get; set; }
        public string? Observacoes { get; set; }
    }
}