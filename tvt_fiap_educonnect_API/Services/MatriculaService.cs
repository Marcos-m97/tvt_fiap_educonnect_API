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
        // CRIAR MATRÍCULA (INSCRIÇÃO)
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
        // UPLOAD COMPROVANTE DE PAGAMENTO
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

        // ==============================================================
        // UPLOAD DOCUMENTOS PESSOAIS
        // ==============================================================
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

        // ==============================================================
        // UPLOAD DOCUMENTOS DE ESCOLARIDADE
        // ==============================================================
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
        // ATUALIZAR STATUS (EMAIL AUTOMÁTICO NA EFETIVAÇÃO)
        // ==============================================================
        public async Task<MatriculaDTO?> AtualizarStatus(int id, MatriculaStatus novoStatus)
        {
            var m = await _repo.ObterPorId(id);
            if (m == null) return null;

            var statusAnterior = m.Status;

            m.Status = novoStatus;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            // 🔔 EMAIL SOMENTE NA TRANSIÇÃO PARA EFETIVADA
            if (statusAnterior != MatriculaStatus.Efetivada &&
                novoStatus == MatriculaStatus.Efetivada)
            {
                var email = m.Aluno.Usuario.Email;
                var nomeAluno = m.Aluno.Usuario.Nome;
                var nomeTurma = m.Turma.Nome;

                var assunto = "🎉 Matrícula efetivada com sucesso - EduConnect";

                var corpo = $@"
Olá {nomeAluno},

Temos uma ótima notícia! 🎓

Sua matrícula na turma **{nomeTurma}** foi **efetivada com sucesso**.

A partir de agora, você já pode:
- Acessar o portal do aluno
- Visualizar suas disciplinas
- Acompanhar atividades e boletins

👉 Acesse o portal:
https://educonnect.com/login

Se precisar de ajuda, nossa equipe administrativa está à disposição.

Bem-vindo(a) à EduConnect!
";

                await _emailService.EnviarEmail(email, assunto, corpo);
            }

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        // ==============================================================
        // EXCLUIR MATRÍCULA
        // ==============================================================
        public Task<bool> Deletar(int id) => _repo.Deletar(id);

        // ==============================================================
        // MAPEAMENTO DTO
        // ==============================================================
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
        // ==============================================================
        // LISTAR ALUNOS DA TURMA (PROFESSOR / ADM)
        // ==============================================================
        public async Task<IEnumerable<AlunoTurmaDTO>> ListarAlunosPorTurma(int turmaId)
        {
            var matriculas = await _repo.ListarPorTurma(turmaId);

            return matriculas
                .Where(m => m.Status == MatriculaStatus.Efetivada)
                .Select(m => new AlunoTurmaDTO
                {
                    AlunoId = m.AlunoId,
                    Nome = m.Aluno.Usuario.Nome,
                    Email = m.Aluno.Usuario.Email,
                    Status = m.Status
                });
        }

    }
}
