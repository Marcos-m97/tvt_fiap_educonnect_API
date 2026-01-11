namespace EduConnect_API.Models.DTOs
{
    public class MeContextoDTO
    {
        public string TipoUsuario { get; set; } = string.Empty;

        // =========================
        // ALUNO
        // =========================
        public int? AlunoId { get; set; }
        public int? TurmaId { get; set; }
        public string? TurmaNome { get; set; }
        public string? CursoNome { get; set; }

        public List<DisciplinaResumoDTO>? Disciplinas { get; set; }

        // =========================
        // PROFESSOR
        // =========================
        public int? ProfessorId { get; set; }
        public List<TurmaDisciplinaResumoDTO>? TurmasDisciplinas { get; set; }
    }
}
