namespace EduConnect_API.Models.DTOs
{
    public class MatriculaDTO
    {
        public Guid Id { get; set; }

        public Guid AlunoId { get; set; }
        public string AlunoNome { get; set; } = string.Empty;

        public Guid TurmaId { get; set; }
        public string TurmaNome { get; set; } = string.Empty;

        public MatriculaStatus Status { get; set; }

        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}