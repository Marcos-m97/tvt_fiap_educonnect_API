using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa o perfil docente de um usuário no EduConnect.
    ///
    /// O Professor é uma especialização da entidade Usuario. Enquanto Usuario
    /// concentra os dados comuns de autenticação e identificação, Professor
    /// armazena informações específicas do perfil acadêmico docente, como
    /// especialidade, formação e currículo Lattes.
    ///
    /// Essa separação permite manter os dados comuns de acesso centralizados
    /// em Usuario e os dados específicos do professor em uma tabela própria.
    /// </summary>
    public class Professor
    {
        /// <summary>
        /// Identificador único do professor no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chave estrangeira que vincula o professor a um usuário do sistema.
        ///
        /// Todo professor também é um usuário, pois precisa autenticar na plataforma
        /// e acessar funcionalidades protegidas.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados do usuário vinculado.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include ou pelo tracking do contexto.
        /// Na criação do professor, normalmente informamos apenas o UsuarioId.
        /// </summary>
        public Usuario Usuario { get; set; } = null!;

        /// <summary>
        /// Área principal de atuação ou especialidade do professor.
        /// </summary>
        public string? Especialidade { get; set; }

        /// <summary>
        /// Formação acadêmica do professor.
        ///
        /// Campo opcional utilizado para complementar o cadastro docente.
        /// </summary>
        public string? Formacao { get; set; }

        /// <summary>
        /// Link ou referência para o currículo Lattes do professor.
        ///
        /// Campo opcional, útil para registro acadêmico e apresentação do docente.
        /// </summary>
        public string? CurriculoLattes { get; set; }

        /// <summary>
        /// Relação entre professor, turma e disciplina.
        ///
        /// Essa coleção indica quais disciplinas o professor ministra
        /// em determinadas turmas dentro do EduConnect.
        /// </summary>
        public ICollection<TurmaDisciplina> TurmasDisciplinas { get; set; } = new List<TurmaDisciplina>();
    }
}