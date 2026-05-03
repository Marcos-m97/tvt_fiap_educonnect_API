using System;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa a entrega de uma atividade feita por um aluno.
    ///
    /// No EduConnect, a EntregaAtividade conecta um aluno a uma atividade específica,
    /// armazenando o arquivo/resposta enviada, a data de envio, a nota atribuída
    /// e o feedback do professor.
    ///
    /// Essa entidade é essencial para o fluxo acadêmico de avaliação, pois permite
    /// acompanhar quais alunos entregaram uma atividade e quais entregas já foram
    /// corrigidas.
    /// </summary>
    public class EntregaAtividade
    {
        /// <summary>
        /// Identificador único da entrega no banco de dados.
        /// </summary>
        public int Id { get; set; }

        // ============================================================
        // 1. RELACIONAMENTO COM ATIVIDADE
        // ============================================================

        /// <summary>
        /// Chave estrangeira da atividade entregue.
        ///
        /// Indica a qual atividade esta entrega pertence.
        /// </summary>
        public int AtividadeId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados da atividade.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include. Na criação da entrega,
        /// normalmente informamos apenas o AtividadeId.
        /// </summary>
        public Atividade Atividade { get; set; } = null!;

        // ============================================================
        // 2. RELACIONAMENTO COM ALUNO
        // ============================================================

        /// <summary>
        /// Chave estrangeira do aluno que realizou a entrega.
        /// </summary>
        public int AlunoId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados do aluno.
        ///
        /// Permite consultar informações do aluno e do usuário vinculado,
        /// como nome e e-mail, quando a relação é carregada pelo Repository.
        /// </summary>
        public Aluno Aluno { get; set; } = null!;

        // ============================================================
        // 3. DADOS DA ENTREGA
        // ============================================================

        /// <summary>
        /// Caminho ou referência do arquivo enviado pelo aluno.
        ///
        /// Esse campo pode armazenar o path do arquivo salvo no servidor
        /// ou outra forma de referência definida pelo serviço de armazenamento.
        /// </summary>
        public string? Arquivo { get; set; }

        /// <summary>
        /// Data em que a atividade foi enviada pelo aluno.
        ///
        /// O valor padrão é definido em UTC para manter consistência no backend.
        /// </summary>
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

        // ============================================================
        // 4. AVALIAÇÃO DO PROFESSOR
        // ============================================================

        /// <summary>
        /// Nota atribuída pelo professor à entrega.
        ///
        /// O campo é opcional porque a entrega pode existir antes de ser corrigida.
        /// </summary>
        public decimal? Nota { get; set; }

        /// <summary>
        /// Comentário ou feedback do professor sobre a entrega.
        ///
        /// Esse campo permite registrar observações qualitativas da correção,
        /// orientações de melhoria ou justificativa da nota.
        /// </summary>
        public string? FeedbackProfessor { get; set; }
    }
}