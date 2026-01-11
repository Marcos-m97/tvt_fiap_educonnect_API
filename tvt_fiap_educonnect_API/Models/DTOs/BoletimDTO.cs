namespace EduConnect_API.Models.DTOs
{
    public class BoletimDTO
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public int TurmaId { get; set; }
        public DateTime GeradoEm { get; set; }

        public List<BoletimDisciplinaDTO> Disciplinas { get; set; } = new();
    }
}
