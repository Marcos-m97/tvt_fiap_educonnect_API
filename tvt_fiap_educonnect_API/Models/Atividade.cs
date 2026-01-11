using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    public class Atividade
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Data limite para envio
        public DateTime DataEntrega { get; set; }

        // Tipo da atividade (prova, trabalho, exercício, etc.)
        public TipoAtividade Tipo { get; set; }

        // FK → TurmaDisciplina (atividade vinculada à disciplina da turma)
        public int TurmaDisciplinaId { get; set; }
        public TurmaDisciplina TurmaDisciplina { get; set; }

        // Entregas dos alunos
        public ICollection<EntregaAtividade> Entregas { get; set; } = new List<EntregaAtividade>();
    }
}