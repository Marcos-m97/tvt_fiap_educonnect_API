namespace EduConnect_API.Models.DTOs
{
    public class CriarAlunoDTO
    {
        public int UsuarioId { get; set; }

        public string CPF { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public string? Endereco { get; set; }
    }
}
