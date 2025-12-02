namespace EduConnect_API.Models.DTOs
{
    public class CriarMatriculaDTO
    {
        public Guid TurmaId { get; set; }

        // dados da inscrição
        public string NomeCompleto { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
    }

}
