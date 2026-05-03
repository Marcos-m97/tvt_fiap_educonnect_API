using System;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa um evento acadêmico ou administrativo no EduConnect.
    ///
    /// No sistema, um evento pode ser usado para registrar informações importantes
    /// no calendário acadêmico, como provas, atividades, aulas extras, reuniões
    /// ou avisos gerais.
    ///
    /// O evento pode ser geral ou vinculado opcionalmente a uma turma ou a uma
    /// TurmaDisciplina específica, permitindo flexibilidade para diferentes
    /// contextos acadêmicos.
    /// </summary>
    public class Evento
    {
        /// <summary>
        /// Identificador único do evento no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Título principal do evento.
        ///
        /// Esse campo é exibido no calendário ou nas telas de eventos.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional do evento.
        ///
        /// Pode conter detalhes adicionais, instruções, observações
        /// ou informações complementares para alunos, professores ou administradores.
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Data e horário de início do evento.
        /// </summary>
        public DateTime Inicio { get; set; }

        /// <summary>
        /// Data e horário de término do evento.
        ///
        /// Campo opcional, pois alguns eventos podem possuir apenas data/hora de início
        /// ou representar um marco acadêmico sem duração definida.
        /// </summary>
        public DateTime? Fim { get; set; }

        /// <summary>
        /// Tipo do evento.
        ///
        /// Permite classificar o evento como geral, prova, atividade,
        /// aula extra ou reunião.
        /// </summary>
        public TipoEvento Tipo { get; set; }

        // ============================================================
        // 1. RELACIONAMENTO OPCIONAL COM TURMA
        // ============================================================

        /// <summary>
        /// Chave estrangeira opcional da turma relacionada ao evento.
        ///
        /// Quando preenchido, indica que o evento pertence a uma turma específica.
        /// Quando nulo, o evento pode ser geral ou estar vinculado apenas
        /// a outro contexto.
        /// </summary>
        public int? TurmaId { get; set; }

        /// <summary>
        /// Propriedade de navegação opcional para acessar os dados da turma.
        /// </summary>
        public Turma? Turma { get; set; }

        // ============================================================
        // 2. RELACIONAMENTO OPCIONAL COM TURMA/DISCIPLINA
        // ============================================================

        /// <summary>
        /// Chave estrangeira opcional para a associação entre turma, disciplina
        /// e professor.
        ///
        /// Quando preenchido, indica que o evento está relacionado a uma disciplina
        /// específica dentro de uma turma.
        /// </summary>
        public int? TurmaDisciplinaId { get; set; }

        /// <summary>
        /// Propriedade de navegação opcional para acessar o contexto acadêmico
        /// da disciplina dentro da turma.
        /// </summary>
        public TurmaDisciplina? TurmaDisciplina { get; set; }

        // ============================================================
        // 3. AUDITORIA DE CRIAÇÃO
        // ============================================================

        /// <summary>
        /// Identificador do usuário que criou o evento.
        ///
        /// Esse campo permite rastrear quem registrou o evento no sistema.
        /// </summary>
        public int CriadoPorId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar o usuário que criou o evento.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include. Na criação do evento,
        /// normalmente informamos apenas o CriadoPorId.
        /// </summary>
        public Usuario CriadoPor { get; set; } = null!;
    }

    /// <summary>
    /// Enum que representa os tipos possíveis de evento no EduConnect.
    ///
    /// Essa classificação facilita a organização visual e funcional do calendário,
    /// permitindo diferenciar eventos gerais, provas, atividades, aulas extras
    /// e reuniões.
    /// </summary>
    public enum TipoEvento
    {
        /// <summary>
        /// Evento geral, sem uma categoria acadêmica específica.
        /// </summary>
        Geral = 1,

        /// <summary>
        /// Evento relacionado a uma prova.
        /// </summary>
        Prova = 2,

        /// <summary>
        /// Evento relacionado a uma atividade acadêmica.
        /// </summary>
        Atividade = 3,

        /// <summary>
        /// Evento relacionado a uma aula extra.
        /// </summary>
        AulaExtra = 4,

        /// <summary>
        /// Evento relacionado a uma reunião.
        /// </summary>
        Reuniao = 5
    }
}