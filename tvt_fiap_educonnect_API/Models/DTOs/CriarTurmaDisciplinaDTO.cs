namespace EduConnect_API.Models.DTOs
{
    public class CriarTurmaDisciplinaDTO
    {
        public Guid TurmaId { get; set; }
        public Guid DisciplinaId { get; set; }
        public Guid ProfessorId { get; set; }
    }

}
