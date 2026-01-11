using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    public class Turma
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        // Ex: "ADS - Turma 2025/1 Noite"

        public string Periodo { get; set; } = string.Empty;
        // Ex: "Manhã", "Tarde", "Noite", "EAD"

        public string Semestre { get; set; } = string.Empty;
        // Ex: "2025/1", "2025/2"

        // FK → Curso ao qual a turma pertence
        public int CursoId { get; set; }
        public Curso Curso { get; set; }

        // Relacionamento com os alunos matriculados
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

        // Relação com disciplinas da turma
        public ICollection<TurmaDisciplina> TurmaDisciplinas { get; set; } = new List<TurmaDisciplina>();
    }
}
