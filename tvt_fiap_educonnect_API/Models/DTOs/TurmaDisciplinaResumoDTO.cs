namespace EduConnect_API.Models.DTOs
{
    public class TurmaDisciplinaResumoDTO
    {
        public Guid TurmaDisciplinaId { get; set; }
        public Guid TurmaId { get; set; }
        public string TurmaNome { get; set; } = string.Empty;

        public Guid DisciplinaId { get; set; }
        public string DisciplinaNome { get; set; } = string.Empty;
    }
}