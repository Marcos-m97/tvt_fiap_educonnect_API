using System;

namespace EduConnect_API.Models
{
    public class Evento
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }

        public DateTime Inicio { get; set; }
        public DateTime? Fim { get; set; }

        public TipoEvento Tipo { get; set; }

        // Relacionamentos opcionais:
        public int? TurmaId { get; set; }
        public Turma? Turma { get; set; }

        public int? TurmaDisciplinaId { get; set; }
        public TurmaDisciplina? TurmaDisciplina { get; set; }

        public int CriadoPorId { get; set; }
        public Usuario CriadoPor { get; set; }
    }

    public enum TipoEvento
    {
        Geral = 1,
        Prova = 2,
        Atividade = 3,
        AulaExtra = 4,
        Reuniao = 5
    }
}

