using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _repo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly ITurmaRepository _turmaRepo;
        private readonly IArquivoStorageService _storage;
        private readonly IEmailService _emailService;

        public MatriculaService(
            IMatriculaRepository repo,
            IAlunoRepository alunoRepo,
            ITurmaRepository turmaRepo,
            IArquivoStorageService storage,
            IEmailService emailService)
        {
            _repo = repo;
            _alunoRepo = alunoRepo;
            _turmaRepo = turmaRepo;
            _storage = storage;
            _emailService = emailService;
        }

        // ==============================================================
        // CRIAR MATRÍCULA
        // ==============================================================
        public async Task<MatriculaDTO> Criar(int usuarioId, CriarMatriculaDTO dto)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new Exception("Aluno não encontrado.");

            var turma = await _turmaRepo.ObterPorId(dto.TurmaId)
                ?? throw new Exception("Turma não encontrada.");

            var matricula = new Matricula
            {
                AlunoId = aluno.Id,
                TurmaId = dto.TurmaId,
                Status = MatriculaStatus.Inscricao
            };

            matricula = await _repo.Criar(matricula);

            return MapToDTO(matricula, aluno.Usuario.Nome, turma.Nome);
        }

        // ==============================================================
        // UPLOAD COMPROVANTE
        // ==============================================================
        public async Task<MatriculaDTO?> UploadComprovantePagamento(int id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            var caminho = $"uploads/matriculas/{id}/comprovante_pagamento.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.ComprovantePagamento = caminhoSalvo;
            m.Status = MatriculaStatus.Pagamento;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public async Task<MatriculaDTO?> UploadDocumentosPessoais(int id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            var caminho = $"uploads/matriculas/{id}/documentos_pessoais.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.DocumentosPessoais = caminhoSalvo;
            m.Status = MatriculaStatus.Documentos;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public async Task<MatriculaDTO?> UploadDocumentosEscolaridade(int id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            var caminho = $"uploads/matriculas/{id}/documentos_escolaridade.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.DocumentosEscolaridade = caminhoSalvo;
            m.Status = MatriculaStatus.Documentos;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        // ==============================================================
        // DOWNLOADS
        // ==============================================================
        public async Task<byte[]?> BaixarComprovante(int id)
        {
            var m = await _repo.ObterPorId(id);
            if (m?.ComprovantePagamento == null) return null;
            return await _storage.BaixarAsync(m.ComprovantePagamento);
        }

        public async Task<byte[]?> BaixarDocumentosPessoais(int id)
        {
            var m = await _repo.ObterPorId(id);
            if (m?.DocumentosPessoais == null) return null;
            return await _storage.BaixarAsync(m.DocumentosPessoais);
        }

        public async Task<byte[]?> BaixarDocumentosEscolaridade(int id)
        {
            var m = await _repo.ObterPorId(id);
            if (m?.DocumentosEscolaridade == null) return null;
            return await _storage.BaixarAsync(m.DocumentosEscolaridade);
        }

        // ==============================================================
        // CONSULTAS
        // ==============================================================
        public async Task<MatriculaDTO?> ObterPorId(int id)
        {
            var m = await _repo.ObterPorId(id);
            return m == null ? null : MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public async Task<IEnumerable<MatriculaDTO>> Listar()
        {
            var lista = await _repo.Listar();
            return lista.Select(m => MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        public async Task<IEnumerable<MatriculaDTO>> ListarPorAluno(int alunoId)
        {
            var lista = await _repo.ListarPorAluno(alunoId);
            return lista.Select(m => MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        public async Task<IEnumerable<MatriculaDTO>> ListarPorTurma(int turmaId)
        {
            var lista = await _repo.ListarPorTurma(turmaId);
            return lista.Select(m => MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        // ==============================================================
        // ATUALIZAR STATUS
        // ==============================================================
        public async Task<MatriculaDTO?> AtualizarStatus(int id, MatriculaStatus novoStatus)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            var statusAnterior = m.Status;

            m.Status = novoStatus;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            if (statusAnterior != MatriculaStatus.Efetivada &&
                novoStatus == MatriculaStatus.Efetivada)
            {
                var email = m.Aluno.Usuario.Email;
                var nomeAluno = m.Aluno.Usuario.Nome;
                var nomeTurma = m.Turma.Nome;

                var assunto = "🎉 Matrícula efetivada com sucesso - EduConnect";

                var corpo = $@"
Olá {nomeAluno},

Sua matrícula na turma {nomeTurma} foi efetivada com sucesso.

Acesse o portal:
https://educonnect.com/login

Bem-vindo(a) à EduConnect!
";

                await _emailService.EnviarEmail(email, assunto, corpo);
            }

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public Task<bool> Deletar(int id) => _repo.Deletar(id);

        // ==============================================================
        // 🔥 MAP CORRIGIDO COM USUARIOID
        // ==============================================================
        private MatriculaDTO MapToDTO(Matricula m, string alunoNome, string turmaNome)
        {
            return new MatriculaDTO
            {
                Id = m.Id,
                AlunoId = m.AlunoId,
                UsuarioId = m.Aluno.UsuarioId, // 🔥 ESSENCIAL PARA FOTO
                AlunoNome = alunoNome,
                TurmaId = m.TurmaId,
                TurmaNome = turmaNome,
                Status = m.Status,
                CriadoEm = m.CriadoEm,
                AtualizadoEm = m.AtualizadoEm
            };
        }

        // ==============================================================
        // LISTAR ALUNOS DA TURMA
        // ==============================================================
        public async Task<object> ListarAlunosPorTurma(
            int turmaId,
            int page,
            int pageSize,
            string? search)
        {
            var (items, totalCount) =
                await _repo.ListarAlunosPorTurmaPaginado(
                    turmaId, page, pageSize, search);

            var result = items.Select(m => new AlunoTurmaDTO
            {
                AlunoId = m.AlunoId,
                UsuarioId = m.Aluno.UsuarioId,
                Nome = m.Aluno.Usuario.Nome,
                Email = m.Aluno.Usuario.Email,
                Status = m.Status
            });

            return new
            {
                items = result,
                totalCount,
                page,
                pageSize
            };
        }

        public async Task<MatriculaDTO?> ObterAtivaPorAlunoId(int alunoId)
        {
            var m = await _repo.ObterAtivaPorAlunoId(alunoId);
            if (m == null) return null;

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        public async Task<PagedResultCommonDTO<MatriculaDTO>> ListarPaginado(
            int page,
            int pageSize,
            string? search,
            MatriculaStatus? status)
        {
            var (items, totalCount) =
                await _repo.ListarPaginado(page, pageSize, search, status);

            var data = items.Select(m =>
                MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));

            return new PagedResultCommonDTO<MatriculaDTO>
            {
                Data = data,
                Total = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}