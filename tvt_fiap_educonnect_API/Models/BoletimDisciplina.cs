namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa o desempenho do aluno em uma disciplina
    /// dentro de um boletim.
    ///
    /// No EduConnect, cada boletim pode possuir várias disciplinas.
    /// Para cada disciplina, o sistema consolida informações como nota,
    /// média, situação acadêmica, total de atividades e detalhes das atividades.
    /// </summary>
    public class BoletimDisciplina
    {
        /// <summary>
        /// Identificador único do registro de disciplina no boletim.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chave estrangeira que vincula esta disciplina ao boletim geral.
        /// </summary>
        public int BoletimId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar o boletim ao qual
        /// esta disciplina pertence.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include.
        /// </summary>
        public Boletim Boletim { get; set; } = null!;

        /// <summary>
        /// Nome da disciplina exibido no boletim.
        ///
        /// Esse valor é armazenado no boletim para registrar o nome da disciplina
        /// no momento da geração.
        /// </summary>
        public string NomeDisciplina { get; set; } = string.Empty;

        /// <summary>
        /// Nota consolidada do aluno na disciplina.
        /// </summary>
        public double Nota { get; set; }

        /// <summary>
        /// Média utilizada como referência para aprovação.
        ///
        /// Pode representar a média mínima exigida ou a média calculada,
        /// dependendo da regra definida no service.
        /// </summary>
        public double Media { get; set; }

        /// <summary>
        /// Situação acadêmica do aluno na disciplina.
        ///
        /// Exemplo: Aprovado, Reprovado, Em andamento ou Sem atividades.
        /// </summary>
        public string Situacao { get; set; } = string.Empty;

        /// <summary>
        /// Total de atividades consideradas no cálculo da disciplina.
        /// </summary>
        public int TotalAtividades { get; set; }

        /// <summary>
        /// Lista de atividades avaliadas dentro da disciplina.
        ///
        /// Cada item representa uma atividade da disciplina e informa
        /// se foi entregue e qual nota foi obtida.
        /// </summary>
        public List<BoletimAtividade> Atividades { get; set; } = new();
    }
}