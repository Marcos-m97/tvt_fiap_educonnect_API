namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa o detalhe de uma atividade dentro de uma disciplina
    /// no boletim do aluno.
    ///
    /// No EduConnect, essa entidade permite mostrar quais atividades foram
    /// consideradas no boletim, se foram entregues e qual nota foi atribuída.
    /// </summary>
    public class BoletimAtividade
    {
        /// <summary>
        /// Identificador único do registro de atividade no boletim.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chave estrangeira que vincula esta atividade ao resultado
        /// de uma disciplina no boletim.
        /// </summary>
        public int BoletimDisciplinaId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar a disciplina do boletim
        /// à qual esta atividade pertence.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include.
        /// </summary>
        public BoletimDisciplina BoletimDisciplina { get; set; } = null!;

        /// <summary>
        /// Identificador da atividade acadêmica original.
        ///
        /// Esse campo permite manter referência com a atividade cadastrada
        /// no sistema.
        /// </summary>
        public int AtividadeId { get; set; }

        /// <summary>
        /// Título da atividade exibido no boletim.
        ///
        /// Esse valor é armazenado no boletim para registrar o nome da atividade
        /// no momento da geração.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Nota obtida pelo aluno na atividade.
        ///
        /// O campo é opcional porque pode existir atividade sem entrega
        /// ou ainda não corrigida.
        /// </summary>
        public double? Nota { get; set; }

        /// <summary>
        /// Indica se o aluno entregou a atividade.
        ///
        /// Esse campo facilita a exibição do boletim, permitindo diferenciar
        /// atividades entregues, pendentes ou não avaliadas.
        /// </summary>
        public bool Entregue { get; set; }
    }
}