using EduConnect_API.Models;

/// <summary>
/// Entidade que representa uma disciplina dentro da estrutura acadêmica do EduConnect.
///
/// No sistema, a disciplina pertence a um curso e pode posteriormente ser associada
/// a turmas e professores por meio da entidade TurmaDisciplina.
///
/// Exemplo:
/// Um curso como "Análise e Desenvolvimento de Sistemas" pode possuir disciplinas
/// como "Banco de Dados", "Programação Web" e "Engenharia de Software".
/// </summary>
public class Disciplina
{
    /// <summary>
    /// Identificador único da disciplina no banco de dados.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome da disciplina exibido nas telas acadêmicas.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da disciplina.
    ///
    /// Pode conter informações sobre objetivos, conteúdo programático
    /// ou proposta acadêmica.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Carga horária da disciplina.
    ///
    /// Representa a quantidade de horas previstas para a disciplina
    /// dentro do curso.
    /// </summary>
    public int CargaHoraria { get; set; }

    /// <summary>
    /// Chave estrangeira que vincula a disciplina a um curso.
    /// </summary>
    public int CursoId { get; set; }

    /// <summary>
    /// Propriedade de navegação para acessar o curso vinculado.
    ///
    /// O null! é utilizado porque o Entity Framework preenche essa propriedade
    /// quando a relação é carregada com Include ou pelo tracking do contexto.
    /// Na criação da disciplina, normalmente informamos apenas o CursoId.
    /// </summary>
    public Curso Curso { get; set; } = null!;

    /// <summary>
    /// Indica se a disciplina está ativa no sistema.
    ///
    /// O EduConnect utiliza soft delete para preservar histórico acadêmico.
    /// Em vez de remover a disciplina fisicamente do banco, o campo Ativo
    /// é alterado para false.
    /// </summary>
    public bool Ativo { get; set; } = true;
}