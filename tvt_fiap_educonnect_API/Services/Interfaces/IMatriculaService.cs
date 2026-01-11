using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IMatriculaService
    {
        Task<MatriculaDTO> Criar(int alunoId, CriarMatriculaDTO dto);
        Task<MatriculaDTO?> ObterPorId(int id);
        Task<IEnumerable<MatriculaDTO>> Listar();
        Task<IEnumerable<MatriculaDTO>> ListarPorAluno(int alunoId);
        Task<IEnumerable<MatriculaDTO>> ListarPorTurma(int turmaId);
        Task<MatriculaDTO?> AtualizarStatus(int id, MatriculaStatus novoStatus);
        Task<bool> Deletar(int id);

        Task<MatriculaDTO?> UploadComprovantePagamento(int id, IFormFile arquivo);
        Task<MatriculaDTO?> UploadDocumentosPessoais(int id, IFormFile arquivo);
        Task<MatriculaDTO?> UploadDocumentosEscolaridade(int id, IFormFile arquivo);
        Task<IEnumerable<AlunoTurmaDTO>> ListarAlunosPorTurma(int turmaId);


        Task<byte[]?> BaixarComprovante(int id);
        Task<byte[]?> BaixarDocumentosPessoais(int id);
        Task<byte[]?> BaixarDocumentosEscolaridade(int id);
    }
}
