using System;

namespace EduConnect_API.Models.DTOs
{
    public class AdminDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? Departamento { get; set; }
        public string? Cargo { get; set; }
    }

}