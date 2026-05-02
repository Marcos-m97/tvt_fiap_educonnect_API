using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa uma turma dentro do EduConnect.
    ///
    /// A turma é uma oferta concreta de um curso em determinado período e semestre.
    /// Ela permite organizar os alunos matriculados e as disciplinas que serão
    /// ministradas naquele contexto acadêmico.
    ///
    /// Exemplo:
    /// Um curso de ADS pode possuir a turma "ADS - 2025/1 Noite",
    /// vinculada ao semestre 2025/1 e ao período noturno.
    /// </summary>
    public class Turma
    {
        /// <summary>
        /// Identificador único da turma no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da turma exibido nas telas acadêmicas.
        ///
        /// Exemplo: "ADS - Turma 2025/1 Noite".
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Período de oferta da turma.
        ///
        /// Exemplos: "Manhã", "Tarde", "Noite" ou "EAD".
        /// </summary>
        public string Periodo { get; set; } = string.Empty;

        /// <summary>
        /// Semestre de referência da turma.
        ///
        /// Exemplos: "2025/1" ou "2025/2".
        /// </summary>
        public string Semestre { get; set; } = string.Empty;

        /// <summary>
        /// Chave estrangeira que vincula a turma a um curso.
        /// </summary>
        public int CursoId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar o curso vinculado.
        ///
        /// Essa propriedade é opcional no código porque a turma pode ser criada
        /// informando apenas o CursoId. Quando necessário, o Entity Framework carrega
        /// o curso relacionado usando Include.
        /// </summary>
        public Curso? Curso { get; set; }

        /// <summary>
        /// Lista de matrículas vinculadas à turma.
        ///
        /// Essa relação representa os alunos que solicitaram ou efetivaram matrícula
        /// nessa turma.
        /// </summary>
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

        /// <summary>
        /// Lista de relações entre turma, disciplina e professor.
        ///
        /// Essa coleção define quais disciplinas serão ofertadas na turma
        /// e quais professores estarão vinculados a elas.
        /// </summary>
        public ICollection<TurmaDisciplina> TurmaDisciplinas { get; set; } = new List<TurmaDisciplina>();

        /// <summary>
        /// Indica se a turma está ativa no sistema.
        ///
        /// O EduConnect utiliza soft delete para preservar histórico acadêmico.
        /// Em vez de remover a turma fisicamente do banco, o campo Ativo
        /// é alterado para false.
        /// </summary>
        public bool Ativo { get; set; } = true;
    }
}