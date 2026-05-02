namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa um usuário do sistema EduConnect.
    ///
    /// No contexto da aplicação, o usuário é a base de autenticação e autorização.
    /// A partir do campo Tipo, o sistema identifica se o usuário atua como
    /// super administrador, administrador, professor ou aluno.
    ///
    /// Essa entidade também armazena informações comuns a todos os perfis,
    /// como nome, e-mail, senha criptografada, status de ativação e foto de perfil.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único do usuário no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do usuário exibido no portal.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// E-mail utilizado para login e comunicação com o usuário.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha armazenada em formato criptografado/hash.
        ///
        /// Por segurança, a aplicação nunca salva a senha em texto puro.
        /// A validação da senha é feita comparando a senha informada no login
        /// com este hash armazenado.
        /// </summary>
        public string SenhaHash { get; set; } = string.Empty;

        /// <summary>
        /// Define o perfil de acesso do usuário no sistema.
        ///
        /// Valores utilizados:
        /// 0 = SuperAdmin
        /// 1 = Admin
        /// 2 = Professor
        /// 3 = Aluno
        ///
        /// Esse campo é utilizado na geração do token JWT e nas regras de autorização
        /// dos endpoints protegidos.
        /// </summary>
        public int Tipo { get; set; }

        /// <summary>
        /// Data de criação do usuário.
        ///
        /// O valor padrão é definido em UTC para manter consistência no backend,
        /// independentemente do fuso horário do cliente.
        /// </summary>
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indica se o usuário está ativo no sistema.
        ///
        /// Em vez de excluir fisicamente o registro do banco, o sistema utiliza
        /// soft delete, alterando esse campo para false.
        /// Isso preserva o histórico e evita perda de informações relacionadas.
        /// </summary>
        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Caminho/URL da foto de perfil do usuário.
        ///
        /// Esse campo é opcional, pois o usuário pode existir sem foto cadastrada.
        /// </summary>
        public string? FotoPerfilUrl { get; set; }
    }
}
