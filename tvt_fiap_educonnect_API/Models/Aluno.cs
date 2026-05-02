using System;
using System.Collections.Generic;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa o perfil acadêmico de um aluno no EduConnect.
    ///
    /// O Aluno é uma especialização do Usuario. Enquanto a entidade Usuario
    /// armazena os dados comuns de autenticação e identificação, a entidade Aluno
    /// armazena informações específicas do contexto acadêmico, como CPF,
    /// data de nascimento, endereço, matrículas e entregas de atividades.
    ///
    /// Essa separação permite que o sistema trate dados comuns de login
    /// de forma centralizada em Usuario, mantendo dados específicos do aluno
    /// em uma tabela própria.
    /// </summary>
    public class Aluno
    {
        /// <summary>
        /// Identificador único do aluno no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chave estrangeira que vincula o aluno a um usuário do sistema.
        ///
        /// No EduConnect, todo aluno também é um usuário, pois precisa realizar login
        /// e acessar funcionalidades protegidas da plataforma.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados do usuário vinculado.
        ///
        /// Por meio dessa relação, é possível obter informações como nome, e-mail,
        /// tipo de usuário e foto de perfil.
        /// </summary>
        public Usuario Usuario { get; set; } = null!;

        /// <summary>
        /// CPF do aluno.
        ///
        /// Representa um dado pessoal complementar usado no cadastro acadêmico.
        /// </summary>
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// Data de nascimento do aluno.
        ///
        /// Campo opcional, pois pode ser preenchido ou atualizado posteriormente.
        /// </summary>
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// Endereço do aluno.
        ///
        /// Campo opcional utilizado para complementar o cadastro acadêmico.
        /// </summary>
        public string? Endereco { get; set; }

        /// <summary>
        /// Lista de matrículas vinculadas ao aluno.
        ///
        /// Essa relação permite acompanhar em quais turmas/cursos o aluno está
        /// matriculado e qual é o status da matrícula.
        /// </summary>
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

        /// <summary>
        /// Lista de entregas de atividades realizadas pelo aluno.
        ///
        /// Essa relação conecta o aluno às atividades enviadas, permitindo
        /// consulta de notas, datas de envio e histórico acadêmico.
        /// </summary>
        public ICollection<EntregaAtividade> Entregas { get; set; } = new List<EntregaAtividade>();
    }
}