using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    public class Aluno
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK → Usuario
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Dados pessoais
        public string CPF { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public string? Endereco { get; set; }

        // Matrículas em turmas
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

        // Entregas de atividades
        public ICollection<EntregaAtividade> Entregas { get; set; } = new List<EntregaAtividade>();
    }
}