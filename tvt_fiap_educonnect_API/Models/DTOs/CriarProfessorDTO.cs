namespace EduConnect_API.Models.DTOs
{
    public class CriarProfessorDTO
    {
        public Guid UsuarioId { get; set; }
        public string? Especialidade { get; set; }
        public string? Formacao { get; set; }
        public string? CurriculoLattes { get; set; }
    }
}