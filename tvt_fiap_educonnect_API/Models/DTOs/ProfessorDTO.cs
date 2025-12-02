using System;

namespace EduConnect_API.Models.DTOs
{
    public class ProfessorDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public string? Especialidade { get; set; }
        public string? Formacao { get; set; }
        public string? CurriculoLattes { get; set; }
    }
}