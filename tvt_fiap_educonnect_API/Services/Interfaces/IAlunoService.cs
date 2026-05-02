using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    /// <summary>
    /// Interface que define as operações de negócio relacionadas ao aluno.
    ///
    /// No EduConnect, o Service de aluno é responsável por criar o perfil acadêmico,
    /// consultar dados do aluno, listar alunos e atualizar informações pessoais.
    /// </summary>
    public interface IAlunoService
    {
        /// <summary>
        /// Cria um perfil de aluno vinculado a um usuário existente.
        /// </summary>
        Task<AlunoDTO> Criar(CriarAlunoDTO dto);

        /// <summary>
        /// Obtém o aluno a partir do ID do usuário.
        /// </summary>
        Task<AlunoDTO?> ObterPorUsuario(int usuarioId);

        /// <summary>
        /// Lista todos os alunos cadastrados.
        /// </summary>
        Task<IEnumerable<AlunoDTO>> Listar();

        /// <summary>
        /// Atualiza os dados pessoais de um aluno.
        /// </summary>
        Task<AlunoDTO?> Atualizar(int id, CriarAlunoDTO dto);
    }
}