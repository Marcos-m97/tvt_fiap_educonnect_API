namespace EduConnect_API.Models.DTOs
{
    public class CriarAtividadeDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataEntrega { get; set; }
        public string? UrlMaterial { get; set; }
        public TipoAtividade Tipo { get; set; }
        public int TurmaDisciplinaId { get; set; }
    }

}
