using System;

namespace EduConnect_API.Models
{
    public class Admin
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK → Usuario
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Dados opcionais
        public string? Departamento { get; set; }
        public string? Cargo { get; set; }
    }
}