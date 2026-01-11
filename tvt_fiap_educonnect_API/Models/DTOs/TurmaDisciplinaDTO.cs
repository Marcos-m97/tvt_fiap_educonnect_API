namespace tvt_fiap_educonnect_API.Models.DTOs
{
    public class TurmaDisciplinaDTO
    {
        public int Id { get; set; }

        public int TurmaId { get; set; }
        public string TurmaNome { get; set; } = string.Empty;

        public int DisciplinaId { get; set; }
        public string DisciplinaNome { get; set; } = string.Empty;

        public int ProfessorId { get; set; }
        public string ProfessorNome { get; set; } = string.Empty;
    }

}
