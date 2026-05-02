using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    /// <summary>
    /// Interface que define o contrato de acesso a dados da entidade Aluno.
    ///
    /// Essa abstração permite que a camada de serviço trabalhe com operações
    /// de aluno sem depender diretamente do Entity Framework ou do AppDbContext.
    /// </summary>
    public interface IAlunoRepository
    {
        /// <summary>
        /// Cria um novo registro de aluno no banco de dados.
        /// </summary>
        Task<Aluno> Criar(Aluno aluno);

        /// <summary>
        /// Obtém um aluno a partir do ID do usuário vinculado.
        ///
        /// Esse método é útil em fluxos onde o frontend conhece o usuário logado
        /// e precisa recuperar o perfil acadêmico correspondente.
        /// </summary>
        Task<Aluno?> ObterPorUsuarioId(int usuarioId);

        /// <summary>
        /// Obtém um aluno pelo seu ID próprio.
        /// </summary>
        Task<Aluno?> ObterPorId(int id);

        /// <summary>
        /// Lista todos os alunos cadastrados.
        /// </summary>
        Task<IEnumerable<Aluno>> Listar();

        /// <summary>
        /// Atualiza os dados de um aluno existente.
        /// </summary>
        Task<Aluno> Atualizar(Aluno aluno);
    }
}