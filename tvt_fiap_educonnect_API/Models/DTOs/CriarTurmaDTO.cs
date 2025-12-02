namespace EduConnect_API.Models.DTOs
{
    public class CriarTurmaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;
        public Guid CursoId { get; set; }
    }
}