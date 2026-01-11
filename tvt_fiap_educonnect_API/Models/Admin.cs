using System;
using EduConnect_API.Models;

namespace EduConnect_API.Models
{
    public class Admin
    {
        public int Id { get; set; }   

        // FK → Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string? Departamento { get; set; }
        public string? Cargo { get; set; }
    }
}