using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    /// <summary>
    /// Interface que define o contrato de acesso a dados da entidade Usuario.
    ///
    /// No EduConnect, essa interface permite separar a regra de negócio da forma
    /// como os dados são consultados ou persistidos no banco.
    /// Com isso, o Service depende de uma abstração e não diretamente do Entity Framework.
    /// </summary>
    public interface IUsuarioRepository
    {
        /// <summary>
        /// Busca um usuário pelo e-mail.
        ///
        /// Utilizado principalmente no fluxo de login e recuperação de senha.
        /// </summary>
        Task<Usuario?> ObterPorEmail(string email);

        /// <summary>
        /// Busca um usuário pelo identificador único.
        ///
        /// Utilizado em operações como edição de perfil, consulta de dados
        /// do usuário logado e atualização de foto.
        /// </summary>
        Task<Usuario?> ObterPorId(int id);

        /// <summary>
        /// Cria um novo usuário no banco de dados.
        /// </summary>
        Task<Usuario> Criar(Usuario usuario);

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        Task<Usuario> Atualizar(Usuario usuario);

        /// <summary>
        /// Desativa logicamente um usuário.
        ///
        /// O registro permanece no banco, mas deixa de ser considerado ativo.
        /// </summary>
        Task<bool> SoftDelete(int id);

        /// <summary>
        /// Reativa um usuário previamente desativado.
        /// </summary>
        Task<bool> Reativar(int id);

        /// <summary>
        /// Lista usuários com paginação e filtro opcional de busca.
        ///
        /// Retorna a lista da página atual e o total de registros encontrados,
        /// permitindo que o frontend monte a paginação corretamente.
        /// </summary>
        Task<(IEnumerable<Usuario>, int)> ListarPaginado(
            int page,
            int pageSize,
            string? search
        );
    }
}