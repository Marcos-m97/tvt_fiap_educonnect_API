using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa uma atividade acadêmica cadastrada pelo professor.
    ///
    /// No EduConnect, a atividade é vinculada a uma TurmaDisciplina, ou seja,
    /// a uma disciplina ofertada em uma turma específica.
    ///
    /// Essa entidade permite que professores criem avaliações, trabalhos,
    /// exercícios ou outros tipos de tarefas para os alunos matriculados.
    /// As entregas realizadas pelos alunos ficam relacionadas por meio da
    /// coleção Entregas.
    /// </summary>
    public class Atividade
    {
        /// <summary>
        /// Identificador único da atividade no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Título da atividade exibido para professores e alunos.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição ou instruções da atividade.
        ///
        /// Pode conter o enunciado, orientações de entrega, critérios de avaliação
        /// ou qualquer informação necessária para o aluno realizar a atividade.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data limite para envio da atividade.
        ///
        /// Esse campo é usado pelo frontend para exibir o prazo de entrega
        /// e orientar o aluno sobre o período disponível para submissão.
        /// </summary>
        public DateTime DataEntrega { get; set; }

        /// <summary>
        /// URL opcional de material relacionado à atividade.
        ///
        /// Pode ser usada para apontar para um documento, formulário,
        /// link externo ou outro recurso complementar.
        /// </summary>
        public string? UrlMaterial { get; set; }

        /// <summary>
        /// Tipo da atividade.
        ///
        /// Permite classificar a atividade como prova, trabalho, exercício
        /// ou outro tipo definido pelo enum TipoAtividade.
        /// </summary>
        public TipoAtividade Tipo { get; set; }

        /// <summary>
        /// Chave estrangeira que vincula a atividade a uma TurmaDisciplina.
        ///
        /// Isso indica que a atividade pertence a uma disciplina específica
        /// dentro de uma turma específica.
        /// </summary>
        public int TurmaDisciplinaId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar o contexto acadêmico da atividade.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include. Na criação da atividade,
        /// normalmente informamos apenas o TurmaDisciplinaId.
        /// </summary>
        public TurmaDisciplina TurmaDisciplina { get; set; } = null!;

        /// <summary>
        /// Lista de entregas feitas pelos alunos para esta atividade.
        ///
        /// Essa relação permite acompanhar quais alunos enviaram a atividade,
        /// data de envio, resposta, arquivo entregue e nota atribuída.
        /// </summary>
        public ICollection<EntregaAtividade> Entregas { get; set; } = new List<EntregaAtividade>();
    }
}