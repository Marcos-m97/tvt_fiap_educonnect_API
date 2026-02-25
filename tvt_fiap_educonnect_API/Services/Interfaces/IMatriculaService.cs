using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IMatriculaService
    {
        Task<MatriculaDTO> Criar(int alunoId, CriarMatriculaDTO dto);
        Task<MatriculaDTO?> ObterPorId(int id);
        Task<IEnumerable<MatriculaDTO>> Listar();
        Task<PagedResultCommonDTO<MatriculaDTO>> ListarPaginado(
        int page,
        int pageSize,
        string? search,
        MatriculaStatus? status);
        Task<IEnumerable<MatriculaDTO>> ListarPorAluno(int alunoId);
        Task<IEnumerable<MatriculaDTO>> ListarPorTurma(int turmaId);
        Task<MatriculaDTO?> AtualizarStatus(int id, MatriculaStatus novoStatus);
        Task<bool> Deletar(int id);

        Task<MatriculaDTO?> UploadComprovantePagamento(int id, IFormFile arquivo);
        Task<MatriculaDTO?> UploadDocumentosPessoais(int id, IFormFile arquivo);
        Task<MatriculaDTO?> UploadDocumentosEscolaridade(int id, IFormFile arquivo);
        Task<object> ListarAlunosPorTurma(
            int turmaId,
            int page,
            int pageSize,
            string? search);
        Task<byte[]?> BaixarComprovante(int id);
        Task<byte[]?> BaixarDocumentosPessoais(int id);
        Task<byte[]?> BaixarDocumentosEscolaridade(int id);
        Task<MatriculaDTO?> ObterAtivaPorAlunoId(int alunoId);
    }
}
