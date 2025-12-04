namespace EduConnect_API.Models.DTOs
{
    public class BoletimDTO
    {
        public Guid Id { get; set; }
        public Guid AlunoId { get; set; }
        public Guid TurmaId { get; set; }
        public DateTime GeradoEm { get; set; }

        public List<BoletimDisciplinaDTO> Disciplinas { get; set; } = new();
    }
}
