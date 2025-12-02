using System;

namespace EduConnect_API.Models
{
    public class TurmaDisciplina
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK → Turma
        public Guid TurmaId { get; set; }
        public Turma Turma { get; set; }

        // FK → Disciplina
        public Guid DisciplinaId { get; set; }
        public Disciplina Disciplina { get; set; }

        // FK → Professor (USUARIO.TIPO = 2)
        public Guid ProfessorId { get; set; }
        public Professor Professor { get; set; }

    }
}