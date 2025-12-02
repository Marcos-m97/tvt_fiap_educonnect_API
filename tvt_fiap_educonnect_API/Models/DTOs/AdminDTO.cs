using System;

namespace EduConnect_API.Models.DTOs
{
    public class AdminDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? Departamento { get; set; }
        public string? Cargo { get; set; }
    }
}