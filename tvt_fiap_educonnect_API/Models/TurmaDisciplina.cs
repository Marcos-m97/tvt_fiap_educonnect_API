using System;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa a associação entre uma turma, uma disciplina
    /// e um professor.
    ///
    /// No EduConnect, essa entidade define a grade acadêmica real de uma turma:
    /// qual disciplina será ministrada e qual professor será responsável.
    ///
    /// Exemplo:
    /// Turma: ADS 2025/1 Noite
    /// Disciplina: Banco de Dados
    /// Professor: João Silva
    /// </summary>
    public class TurmaDisciplina
    {
        /// <summary>
        /// Identificador único do vínculo no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chave estrangeira da turma vinculada.
        /// </summary>
        public int TurmaId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados da turma.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include.
        /// </summary>
        public Turma Turma { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira da disciplina vinculada.
        /// </summary>
        public int DisciplinaId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados da disciplina.
        /// </summary>
        public Disciplina Disciplina { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira do professor responsável pela disciplina na turma.
        /// </summary>
        public int ProfessorId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados do professor.
        /// </summary>
        public Professor Professor { get; set; } = null!;
    }
}