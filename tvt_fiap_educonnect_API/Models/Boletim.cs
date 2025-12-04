namespace EduConnect_API.Models
{
    public class Boletim
    {
        public Guid Id { get; set; }
        public Guid AlunoId { get; set; }
        public Guid TurmaId { get; set; }
        public DateTime GeradoEm { get; set; }

        public List<BoletimDisciplina> Disciplinas { get; set; } = new();
    }
}