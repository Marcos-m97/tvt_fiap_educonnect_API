namespace EduConnect_API.Models.DTOs
{
    public class EventoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime? Fim { get; set; }
        public TipoEvento Tipo { get; set; }

        public int? TurmaId { get; set; }
        public string? TurmaNome { get; set; }

        public int? TurmaDisciplinaId { get; set; }
        public string? DisciplinaNome { get; set; }
    }
}