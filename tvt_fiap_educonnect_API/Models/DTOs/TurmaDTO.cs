namespace EduConnect_API.Models.DTOs
{
    public class TurmaDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;

        public Guid CursoId { get; set; }
        public string CursoNome { get; set; } = string.Empty;
    }
}
