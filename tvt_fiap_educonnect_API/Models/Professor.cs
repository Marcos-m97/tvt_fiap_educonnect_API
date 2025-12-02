using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    public class Professor
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK → Usuario
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Informações específicas do professor
        public string? Especialidade { get; set; }
        public string? Formacao { get; set; }   // opcional
        public string? CurriculoLattes { get; set; } // opcional

        // Disciplinas ministradas em turmas específicas
        public ICollection<TurmaDisciplina> TurmasDisciplinas { get; set; } = new List<TurmaDisciplina>();
    }
}