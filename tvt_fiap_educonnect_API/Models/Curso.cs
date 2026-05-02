using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa um curso disponível na plataforma EduConnect.
    ///
    /// No contexto do sistema, o curso é uma estrutura acadêmica principal.
    /// Ele agrupa disciplinas e pode possuir várias turmas associadas.
    ///
    /// Exemplo:
    /// Um curso como "Análise e Desenvolvimento de Sistemas" pode conter
    /// disciplinas como Banco de Dados, Programação Web e Engenharia de Software,
    /// além de diferentes turmas vinculadas ao longo do tempo.
    /// </summary>
    public class Curso
    {
        /// <summary>
        /// Identificador único do curso no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do curso exibido para administradores, professores e alunos.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição geral do curso.
        ///
        /// Pode ser utilizada para apresentar o objetivo do curso,
        /// sua proposta pedagógica ou informações complementares.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Carga horária total do curso.
        ///
        /// Representa a quantidade total de horas previstas para conclusão
        /// da formação.
        /// </summary>
        public int CargaHoraria { get; set; }

        /// <summary>
        /// Indica se o curso está ativo no sistema.
        ///
        /// O EduConnect utiliza soft delete para cursos, ou seja,
        /// o registro não é removido fisicamente do banco. Em vez disso,
        /// o campo Ativo é alterado para false.
        ///
        /// Essa abordagem preserva o histórico acadêmico de turmas,
        /// matrículas e disciplinas relacionadas ao curso.
        /// </summary>
        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Lista de disciplinas associadas ao curso.
        ///
        /// Representa o relacionamento 1:N, onde um curso pode possuir
        /// várias disciplinas em sua grade acadêmica.
        /// </summary>
        public ICollection<Disciplina> Disciplinas { get; set; } = new List<Disciplina>();

        /// <summary>
        /// Lista de turmas associadas ao curso.
        ///
        /// Representa o relacionamento 1:N, onde um curso pode possuir
        /// várias turmas abertas em diferentes períodos ou edições.
        /// </summary>
        public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
    }
}