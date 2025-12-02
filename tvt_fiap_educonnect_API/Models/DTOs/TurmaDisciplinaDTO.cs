namespace tvt_fiap_educonnect_API.Models.DTOs
{
    public class TurmaDisciplinaDTO
    {
        public Guid Id { get; set; }

        public Guid TurmaId { get; set; }
        public string TurmaNome { get; set; } = string.Empty;

        public Guid DisciplinaId { get; set; }
        public string DisciplinaNome { get; set; } = string.Empty;

        public Guid ProfessorId { get; set; }
        public string ProfessorNome { get; set; } = string.Empty;
    }

}
