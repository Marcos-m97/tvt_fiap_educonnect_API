using System;

namespace EduConnect_API.Models.DTOs
{
    public class AlunoDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public string? Endereco { get; set; }
    }
}
