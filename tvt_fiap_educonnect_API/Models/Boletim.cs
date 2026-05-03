namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa o boletim geral de um aluno em uma turma.
    ///
    /// No EduConnect, o boletim consolida o desempenho acadêmico do aluno,
    /// agrupando os resultados por disciplina e detalhando as atividades
    /// avaliadas dentro de cada uma.
    ///
    /// A estrutura do boletim é composta por:
    /// - Boletim: visão geral do aluno na turma;
    /// - BoletimDisciplina: resultado por disciplina;
    /// - BoletimAtividade: detalhe das atividades avaliadas.
    /// </summary>
    public class Boletim
    {
        /// <summary>
        /// Identificador único do boletim no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do aluno ao qual o boletim pertence.
        /// </summary>
        public int AlunoId { get; set; }

        /// <summary>
        /// Identificador da turma relacionada ao boletim.
        ///
        /// O boletim é gerado considerando o desempenho do aluno
        /// dentro de uma turma específica.
        /// </summary>
        public int TurmaId { get; set; }

        /// <summary>
        /// Data e hora em que o boletim foi gerado.
        ///
        /// Esse campo permite identificar quando os dados acadêmicos
        /// foram consolidados.
        /// </summary>
        public DateTime GeradoEm { get; set; }

        /// <summary>
        /// Lista de resultados por disciplina.
        ///
        /// Cada item representa o desempenho do aluno em uma disciplina
        /// da turma, incluindo média, nota, situação e atividades relacionadas.
        /// </summary>
        public List<BoletimDisciplina> Disciplinas { get; set; } = new();
    }
}