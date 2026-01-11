namespace EduConnect_API.Models.DTOs
{
    public class TurmaDisciplinaResumoDTO
    {
        public int TurmaDisciplinaId { get; set; }
        public int TurmaId { get; set; }
        public string TurmaNome { get; set; } = string.Empty;

        public int DisciplinaId { get; set; }
        public string DisciplinaNome { get; set; } = string.Empty;
    }
}