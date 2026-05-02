using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    /// <summary>
    /// Interface que define as operações de negócio relacionadas a usuários.
    ///
    /// No EduConnect, o Service atua como intermediário entre o Controller
    /// e o Repository, concentrando regras como login, criptografia de senha,
    /// reset de senha, atualização de perfil e validações de foto.
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Realiza a autenticação do usuário a partir de e-mail e senha.
        ///
        /// Retorna o usuário quando as credenciais são válidas ou null
        /// quando o login falha.
        /// </summary>
        Task<Usuario?> Login(LoginDTO dto);

        /// <summary>
        /// Obtém os dados de um usuário pelo ID.
        /// </summary>
        Task<Usuario?> ObterPorId(int id);

        /// <summary>
        /// Cria um novo usuário no sistema.
        ///
        /// A camada de serviço é responsável por transformar o DTO em entidade
        /// e aplicar regras como geração do hash da senha.
        /// </summary>
        Task<Usuario> Criar(CriarUsuarioDTO dto);

        /// <summary>
        /// Atualiza os dados básicos de um usuário existente.
        /// </summary>
        Task<Usuario?> Atualizar(int id, AtualizarUsuarioDTO dto);

        /// <summary>
        /// Desativa logicamente um usuário.
        /// </summary>
        Task<bool> SoftDelete(int id);

        /// <summary>
        /// Reativa um usuário desativado.
        /// </summary>
        Task<bool> Reativar(int id);

        /// <summary>
        /// Inicia o fluxo de recuperação de senha.
        ///
        /// Quando o e-mail existe, o sistema gera um código temporário
        /// e envia as instruções por e-mail.
        /// </summary>
        Task SolicitarResetSenha(string email);

        /// <summary>
        /// Redefine a senha do usuário a partir de um código válido.
        /// </summary>
        Task<bool> ResetarSenha(string email, string codigo, string novaSenha);

        /// <summary>
        /// Lista usuários de forma paginada com busca opcional.
        /// </summary>
        Task<(IEnumerable<Usuario>, int)> ListarPaginado(
            int page,
            int pageSize,
            string? search
        );

        /// <summary>
        /// Atualiza a foto de perfil do usuário.
        ///
        /// Retorna a URL da imagem salva ou null caso o usuário não exista.
        /// </summary>
        Task<string?> AtualizarFotoPerfil(int id, IFormFile file);

        /// <summary>
        /// Obtém os bytes e o content type da foto de perfil do usuário.
        ///
        /// Esse método permite que o backend retorne a imagem como arquivo
        /// para o frontend.
        /// </summary>
        Task<(byte[] bytes, string contentType)?> ObterFotoPerfil(int id);
    }
}