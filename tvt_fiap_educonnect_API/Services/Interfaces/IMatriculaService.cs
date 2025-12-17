using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IMatriculaService
    {
        Task<MatriculaDTO> Criar(Guid alunoId, CriarMatriculaDTO dto);
        Task<MatriculaDTO?> ObterPorId(Guid id);
        Task<IEnumerable<MatriculaDTO>> Listar();
        Task<IEnumerable<MatriculaDTO>> ListarPorAluno(Guid alunoId);
        Task<IEnumerable<MatriculaDTO>> ListarPorTurma(Guid turmaId);
        Task<MatriculaDTO?> AtualizarStatus(Guid id, MatriculaStatus novoStatus);
        Task<bool> Deletar(Guid id);

        Task<MatriculaDTO?> UploadComprovantePagamento(Guid id, IFormFile arquivo);
        Task<MatriculaDTO?> UploadDocumentosPessoais(Guid id, IFormFile arquivo);
        Task<MatriculaDTO?> UploadDocumentosEscolaridade(Guid id, IFormFile arquivo);
        Task<IEnumerable<AlunoTurmaDTO>> ListarAlunosPorTurma(Guid turmaId);


        Task<byte[]?> BaixarComprovante(Guid id);
        Task<byte[]?> BaixarDocumentosPessoais(Guid id);
        Task<byte[]?> BaixarDocumentosEscolaridade(Guid id);
    }
}
