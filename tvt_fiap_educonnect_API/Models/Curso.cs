using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    public class Curso
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Quantidade total de horas do curso
        public int CargaHoraria { get; set; }

        // 🔥 SOFT DELETE
        public bool Ativo { get; set; } = true;

        // Relacionamento 1:N → Curso tem muitas disciplinas
        public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();

        // Relacionamento 1:N → Curso tem muitas turmas
        public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
    }
}