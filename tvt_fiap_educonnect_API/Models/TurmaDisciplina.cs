using System;

namespace EduConnect_API.Models
{
    public class TurmaDisciplina
    {
        public int Id { get; set; }

        // FK → Turma
        public int TurmaId { get; set; }
        public Turma Turma { get; set; }

        // FK → Disciplina
        public int DisciplinaId { get; set; }
        public Disciplina Disciplina { get; set; }

        // FK → Professor (USUARIO.TIPO = 2)
        public int ProfessorId { get; set; }
        public Professor Professor { get; set; }

    }
}