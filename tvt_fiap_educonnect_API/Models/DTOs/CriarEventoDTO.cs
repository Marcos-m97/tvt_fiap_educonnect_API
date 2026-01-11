namespace EduConnect_API.Models.DTOs
{
    public class CriarEventoDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime? Fim { get; set; }
        public TipoEvento Tipo { get; set; }

        public int? TurmaId { get; set; }
        public int? TurmaDisciplinaId { get; set; }
    }
}
