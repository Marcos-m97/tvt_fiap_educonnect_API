namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa um código temporário de recuperação de senha.
    ///
    /// No EduConnect, essa entidade é usada no fluxo de "esqueci minha senha".
    /// Quando o usuário solicita a recuperação, o sistema gera um código,
    /// salva esse registro no banco e envia o código por e-mail.
    ///
    /// Depois, no reset de senha, o sistema valida se o código existe,
    /// se ainda não foi usado e se ainda está dentro do prazo de expiração.
    /// </summary>
    public class PasswordResetCode
    {
        /// <summary>
        /// Identificador único do código de recuperação no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// E-mail do usuário que solicitou a recuperação de senha.
        ///
        /// Esse campo é usado para localizar o usuário e validar se o código
        /// informado pertence ao e-mail correto.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Código temporário enviado ao usuário.
        ///
        /// No fluxo atual, esse código é usado junto com o e-mail para validar
        /// a autorização de redefinição da senha.
        /// </summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de expiração do código.
        ///
        /// Após esse prazo, o código deixa de ser válido para redefinição de senha.
        /// </summary>
        public DateTime ExpiraEm { get; set; }

        /// <summary>
        /// Indica se o código já foi utilizado.
        ///
        /// Após uma redefinição de senha bem-sucedida, esse campo é marcado
        /// como true para impedir reutilização do mesmo código.
        /// </summary>
        public bool Usado { get; set; } = false;
    }
}