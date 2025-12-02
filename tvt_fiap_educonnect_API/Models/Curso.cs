using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    public class Curso
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Quantidade total de horas do curso
        public int CargaHoraria { get; set; }

        // Relacionamento 1:N → Curso tem muitas disciplinas
        public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();

        // Relacionamento 1:N → Curso tem muitas turmas
        public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
    }
}
