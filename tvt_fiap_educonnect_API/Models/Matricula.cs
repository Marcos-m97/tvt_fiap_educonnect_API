using System;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa a matrícula de um aluno em uma turma.
    ///
    /// No EduConnect, a matrícula é o fluxo que conecta o aluno a uma turma
    /// e controla as etapas necessárias até que ele tenha acesso completo
    /// à plataforma acadêmica.
    ///
    /// O processo passa por etapas como inscrição, pagamento, envio de documentos
    /// e efetivação. Essas etapas são controladas pelo campo Status.
    /// </summary>
    public class Matricula
    {
        /// <summary>
        /// Identificador único da matrícula no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chave estrangeira que vincula a matrícula ao aluno.
        ///
        /// Cada matrícula pertence a um aluno específico.
        /// </summary>
        public int AlunoId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados do aluno matriculado.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include.
        /// Na criação da matrícula, normalmente informamos apenas o AlunoId.
        /// </summary>
        public Aluno Aluno { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira que vincula a matrícula a uma turma.
        ///
        /// A turma representa a oferta acadêmica em que o aluno deseja ingressar.
        /// </summary>
        public int TurmaId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados da turma vinculada.
        ///
        /// O Entity Framework pode carregar essa relação para exibir informações
        /// como nome da turma, curso, período e semestre.
        /// </summary>
        public Turma Turma { get; set; } = null!;

        /// <summary>
        /// Status atual da matrícula no fluxo acadêmico.
        ///
        /// Esse campo controla em qual etapa o aluno está:
        /// Inscrição, Pagamento, Documentos ou Efetivada.
        ///
        /// Por padrão, uma nova matrícula começa em Inscrição.
        /// </summary>
        public MatriculaStatus Status { get; set; } = MatriculaStatus.Inscricao;

        /// <summary>
        /// Data de criação da matrícula.
        ///
        /// O valor padrão é definido em UTC para manter consistência no backend.
        /// </summary>
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização da matrícula.
        ///
        /// Esse campo é atualizado quando há mudança de status ou envio de arquivos,
        /// como comprovante de pagamento e documentos.
        /// </summary>
        public DateTime? AtualizadoEm { get; set; }

        /// <summary>
        /// Caminho do arquivo de comprovante de pagamento enviado pelo aluno.
        ///
        /// No fluxo de matrícula, esse arquivo representa a etapa de pagamento.
        /// </summary>
        public string? ComprovantePagamento { get; set; }

        /// <summary>
        /// Caminho do arquivo com documentos pessoais enviados pelo aluno.
        ///
        /// Exemplo: RG, CPF ou outro documento exigido pela instituição.
        /// </summary>
        public string? DocumentosPessoais { get; set; }

        /// <summary>
        /// Caminho do arquivo com documentos de escolaridade enviados pelo aluno.
        ///
        /// Exemplo: histórico escolar, certificado ou comprovante de conclusão.
        /// </summary>
        public string? DocumentosEscolaridade { get; set; }
    }
}