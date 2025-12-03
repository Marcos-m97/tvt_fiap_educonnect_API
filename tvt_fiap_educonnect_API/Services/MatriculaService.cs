using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _repo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly ITurmaRepository _turmaRepo;
        private readonly IArquivoStorageService _storage;

        public MatriculaService(
            IMatriculaRepository repo,
            IAlunoRepository alunoRepo,
            ITurmaRepository turmaRepo,
            IArquivoStorageService storage)
        {
            _repo = repo;
            _alunoRepo = alunoRepo;
            _turmaRepo = turmaRepo;
            _storage = storage;
        }

        public async Task<MatriculaDTO> Criar(Guid usuarioId, CriarMatriculaDTO dto)
        {
            // aluno agora é buscado pelo UsuarioId (via token)
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new Exception("Aluno não encontrado.");

            var turma = await _turmaRepo.ObterPorId(dto.TurmaId)
                ?? throw new Exception("Turma não encontrada.");

            var matricula = new Matricula
            {
                AlunoId = aluno.Id,    // ainda usa o aluno.Id como FK
                TurmaId = dto.TurmaId,
                Status = MatriculaStatus.Inscricao
            };

            matricula = await _repo.Criar(matricula);

            return MapToDTO(matricula, aluno.Usuario.Nome, turma.Nome);
        }


        public async Task<MatriculaDTO?> UploadComprovantePagamento(Guid id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            var caminho = $"uploads/matriculas/{id}/comprovante.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.ComprovantePagamento = caminhoSalvo;
            m.Status = MatriculaStatus.Pagamento;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public async Task<MatriculaDTO?> UploadDocumentosPessoais(Guid id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            var caminho = $"uploads/matriculas/{id}/documentos_pessoais.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.DocumentosPessoais = caminhoSalvo;
            m.Status = MatriculaStatus.Documentos;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public async Task<MatriculaDTO?> UploadDocumentosEscolaridade(Guid id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            var caminho = $"uploads/matriculas/{id}/documentos_escolaridade.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.DocumentosEscolaridade = caminhoSalvo;
            m.Status = MatriculaStatus.Documentos;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public async Task<byte[]?> BaixarComprovante(Guid id)
        {
            var m = await _repo.ObterPorId(id);
            if (m?.ComprovantePagamento == null) return null;

            return await _storage.BaixarAsync(m.ComprovantePagamento);
        }

        public async Task<byte[]?> BaixarDocumentosPessoais(Guid id)
        {
            var m = await _repo.ObterPorId(id);
            if (m?.DocumentosPessoais == null) return null;

            return await _storage.BaixarAsync(m.DocumentosPessoais);
        }

        public async Task<byte[]?> BaixarDocumentosEscolaridade(Guid id)
        {
            var m = await _repo.ObterPorId(id);
            if (m?.DocumentosEscolaridade == null) return null;

            return await _storage.BaixarAsync(m.DocumentosEscolaridade);
        }

        public async Task<MatriculaDTO?> ObterPorId(Guid id)
        {
            var m = await _repo.ObterPorId(id);
            return m == null ? null : MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public async Task<IEnumerable<MatriculaDTO>> Listar()
        {
            var lista = await _repo.Listar();
            return lista.Select(m => MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        public async Task<IEnumerable<MatriculaDTO>> ListarPorAluno(Guid alunoId)
        {
            var lista = await _repo.ListarPorAluno(alunoId);
            return lista.Select(m => MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        public async Task<IEnumerable<MatriculaDTO>> ListarPorTurma(Guid turmaId)
        {
            var lista = await _repo.ListarPorTurma(turmaId);
            return lista.Select(m => MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        public async Task<MatriculaDTO?> AtualizarStatus(Guid id, MatriculaStatus novoStatus)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            m.Status = novoStatus;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public Task<bool> Deletar(Guid id) => _repo.Deletar(id);

        private MatriculaDTO MapToDTO(Matricula m, string alunoNome, string turmaNome)
        {
            return new MatriculaDTO
            {
                Id = m.Id,
                AlunoId = m.AlunoId,
                AlunoNome = alunoNome,
                TurmaId = m.TurmaId,
                TurmaNome = turmaNome,
                Status = m.Status,
                CriadoEm = m.CriadoEm,
                AtualizadoEm = m.AtualizadoEm
            };
        }
    }
}
