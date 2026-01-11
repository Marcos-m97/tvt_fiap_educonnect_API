namespace EduConnect_API.Models
{
    public class Boletim
    {
        public int Id { get; set; }

        public int AlunoId { get; set; }
        public int TurmaId { get; set; }
        public DateTime GeradoEm { get; set; }

        public List<BoletimDisciplina> Disciplinas { get; set; } = new();
    }
}
