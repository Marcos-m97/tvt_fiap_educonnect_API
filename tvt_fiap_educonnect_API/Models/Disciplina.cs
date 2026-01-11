using System;

namespace EduConnect_API.Models
{
    public class Disciplina
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Carga horária da disciplina (ex: 40h, 80h)
        public int CargaHoraria { get; set; }

        // FK → Curso ao qual esta disciplina pertence
        public int CursoId { get; set; }
        public Curso Curso { get; set; }
    }
}