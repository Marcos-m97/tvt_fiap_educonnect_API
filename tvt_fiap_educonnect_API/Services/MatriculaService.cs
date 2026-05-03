using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas às matrículas.
    ///
    /// No EduConnect, a matrícula representa o vínculo entre um aluno e uma turma.
    /// Ela também controla o fluxo de entrada do aluno na plataforma, passando por
    /// etapas como inscrição, pagamento, envio de documentos e efetivação.
    ///
    /// Essa camada coordena criação de matrícula, uploads e downloads de arquivos,
    /// atualização de status, envio de e-mail de confirmação e consultas para
    /// administração, professores e alunos.
    /// </summary>
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _repo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly ITurmaRepository _turmaRepo;
        private readonly IArquivoStorageService _storage;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IMatriculaRepository: acesso aos dados de matrícula.
        /// IAlunoRepository: usado para localizar o aluno a partir do usuário logado.
        /// ITurmaRepository: usado para validar e consultar a turma selecionada.
        /// IArquivoStorageService: responsável por salvar e baixar arquivos da matrícula.
        /// IEmailService: usado para enviar comunicações automáticas ao aluno.
        /// </summary>
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
        // 1. CRIAR MATRÍCULA
        // ==============================================================

        /// <summary>
        /// Cria uma nova matrícula para o aluno logado.
        ///
        /// O Controller envia o ID do usuário autenticado, e o Service busca
        /// o perfil acadêmico de aluno correspondente.
        ///
        /// A matrícula é criada vinculando o aluno à turma informada e inicia
        /// com o status Inscricao, representando o primeiro passo do fluxo.
        /// </summary>
        public async Task<MatriculaDTO> Criar(int usuarioId, CriarMatriculaDTO dto)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new AppException("Aluno não encontrado.", 404);

            var turma = await _turmaRepo.ObterPorId(dto.TurmaId)
                ?? throw new AppException("Turma não encontrada.", 404);

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
        // 2. UPLOAD DO COMPROVANTE DE PAGAMENTO
        // ==============================================================

        /// <summary>
        /// Salva o comprovante de pagamento enviado pelo aluno.
        ///
        /// Após salvar o arquivo, a matrícula avança para o status Pagamento
        /// e registra a data de atualização.
        /// </summary>
        public async Task<MatriculaDTO?> UploadComprovantePagamento(int id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);

            if (m == null)
                return null;

            var caminho = $"uploads/matriculas/{id}/comprovante_pagamento.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.ComprovantePagamento = caminhoSalvo;
            m.Status = MatriculaStatus.Pagamento;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        // ==============================================================
        // 3. UPLOAD DOS DOCUMENTOS PESSOAIS
        // ==============================================================

        /// <summary>
        /// Salva o arquivo de documentos pessoais enviado pelo aluno.
        ///
        /// Essa etapa faz parte da fase documental da matrícula.
        /// Após o upload, o status é atualizado para Documentos.
        /// </summary>
        public async Task<MatriculaDTO?> UploadDocumentosPessoais(int id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);

            if (m == null)
                return null;

            var caminho = $"uploads/matriculas/{id}/documentos_pessoais.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.DocumentosPessoais = caminhoSalvo;
            m.Status = MatriculaStatus.Documentos;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        // ==============================================================
        // 4. UPLOAD DOS DOCUMENTOS DE ESCOLARIDADE
        // ==============================================================

        /// <summary>
        /// Salva o arquivo de documentos de escolaridade enviado pelo aluno.
        ///
        /// Assim como os documentos pessoais, esse arquivo compõe a etapa
        /// documental da matrícula.
        /// </summary>
        public async Task<MatriculaDTO?> UploadDocumentosEscolaridade(int id, IFormFile arquivo)
        {
            var m = await _repo.ObterPorId(id);

            if (m == null)
                return null;

            var caminho = $"uploads/matriculas/{id}/documentos_escolaridade.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            m.DocumentosEscolaridade = caminhoSalvo;
            m.Status = MatriculaStatus.Documentos;
            m.AtualizadoEm = DateTime.UtcNow;

            await _repo.Atualizar(m);

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        // ==============================================================
        // 5. DOWNLOADS DOS ARQUIVOS DA MATRÍCULA
        // ==============================================================

        /// <summary>
        /// Baixa o comprovante de pagamento vinculado à matrícula.
        ///
        /// Retorna null quando a matrícula não existe ou quando o comprovante
        /// ainda não foi enviado.
        /// </summary>
        public async Task<byte[]?> BaixarComprovante(int id)
        {
            var m = await _repo.ObterPorId(id);

            if (m?.ComprovantePagamento == null)
                return null;

            return await _storage.BaixarAsync(m.ComprovantePagamento);
        }

        /// <summary>
        /// Baixa os documentos pessoais vinculados à matrícula.
        /// </summary>
        public async Task<byte[]?> BaixarDocumentosPessoais(int id)
        {
            var m = await _repo.ObterPorId(id);

            if (m?.DocumentosPessoais == null)
                return null;

            return await _storage.BaixarAsync(m.DocumentosPessoais);
        }

        /// <summary>
        /// Baixa os documentos de escolaridade vinculados à matrícula.
        /// </summary>
        public async Task<byte[]?> BaixarDocumentosEscolaridade(int id)
        {
            var m = await _repo.ObterPorId(id);

            if (m?.DocumentosEscolaridade == null)
                return null;

            return await _storage.BaixarAsync(m.DocumentosEscolaridade);
        }

        // ==============================================================
        // 6. CONSULTAS
        // ==============================================================

        /// <summary>
        /// Obtém uma matrícula pelo ID.
        /// </summary>
        public async Task<MatriculaDTO?> ObterPorId(int id)
        {
            var m = await _repo.ObterPorId(id);

            return m == null
                ? null
                : MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        /// <summary>
        /// Lista todas as matrículas.
        ///
        /// Esse método pode ser usado em fluxos administrativos ou internos.
        /// </summary>
        public async Task<IEnumerable<MatriculaDTO>> Listar()
        {
            var lista = await _repo.Listar();

            return lista.Select(m =>
                MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        /// <summary>
        /// Lista as matrículas de um aluno específico.
        ///
        /// Usado para a visão do aluno, permitindo acompanhar suas solicitações
        /// e o status atual de cada matrícula.
        /// </summary>
        public async Task<IEnumerable<MatriculaDTO>> ListarPorAluno(int alunoId)
        {
            var lista = await _repo.ListarPorAluno(alunoId);

            return lista.Select(m =>
                MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        /// <summary>
        /// Lista as matrículas vinculadas a uma turma específica.
        ///
        /// Usado por perfis administrativos e professores para acompanhar alunos
        /// associados a determinada turma.
        /// </summary>
        public async Task<IEnumerable<MatriculaDTO>> ListarPorTurma(int turmaId)
        {
            var lista = await _repo.ListarPorTurma(turmaId);

            return lista.Select(m =>
                MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome));
        }

        // ==============================================================
        // 7. ATUALIZAR STATUS
        // ==============================================================

        /// <summary>
        /// Atualiza o status de uma matrícula.
        ///
        /// Esse método é usado principalmente pela administração para avançar
        /// ou ajustar a etapa da matrícula.
        ///
        /// Quando a matrícula muda para Efetivada pela primeira vez, o sistema
        /// envia automaticamente um e-mail ao aluno informando que o acesso
        /// à plataforma foi liberado.
        /// </summary>
        public async Task<MatriculaDTO?> AtualizarStatus(int id, MatriculaStatus novoStatus)
        {
            var m = await _repo.ObterPorId(id);

            if (m == null)
                return null;

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

        // ==============================================================
        // 8. DELETAR MATRÍCULA
        // ==============================================================

        /// <summary>
        /// Remove uma matrícula.
        ///
        /// A operação é delegada ao Repository.
        /// </summary>
        public Task<bool> Deletar(int id)
        {
            return _repo.Deletar(id);
        }

        // ==============================================================
        // 9. LISTAR ALUNOS DA TURMA
        // ==============================================================

        /// <summary>
        /// Lista alunos efetivados em uma turma com paginação e busca.
        ///
        /// Esse método é utilizado principalmente na visão administrativa
        /// ou do professor, permitindo consultar os alunos pertencentes a uma turma.
        /// </summary>
        public async Task<object> ListarAlunosPorTurma(
            int turmaId,
            int page,
            int pageSize,
            string? search)
        {
            var (items, totalCount) =
                await _repo.ListarAlunosPorTurmaPaginado(
                    turmaId,
                    page,
                    pageSize,
                    search
                );

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

        // ==============================================================
        // 10. OBTER MATRÍCULA ATIVA DO ALUNO
        // ==============================================================

        /// <summary>
        /// Obtém a matrícula ativa/efetivada de um aluno.
        ///
        /// Esse método é usado para identificar se o aluno já possui uma matrícula
        /// efetivada e, a partir disso, liberar sua jornada acadêmica no portal.
        /// </summary>
        public async Task<MatriculaDTO?> ObterAtivaPorAlunoId(int alunoId)
        {
            var m = await _repo.ObterAtivaPorAlunoId(alunoId);

            if (m == null)
                return null;

            return MapToDTO(m, m.Aluno.Usuario.Nome, m.Turma.Nome);
        }

        // ==============================================================
        // 11. LISTAR MATRÍCULAS PAGINADO
        // ==============================================================

        /// <summary>
        /// Lista matrículas de forma paginada, com filtro opcional por nome do aluno
        /// e status da matrícula.
        ///
        /// Esse método é usado no painel administrativo para acompanhar e gerenciar
        /// solicitações de matrícula sem carregar todos os registros de uma vez.
        /// </summary>
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

        // ==============================================================
        // 12. MAPEAMENTO PARA DTO
        // ==============================================================

        /// <summary>
        /// Converte a entidade Matricula em MatriculaDTO.
        ///
        /// O DTO retorna dados técnicos e descritivos para o frontend, incluindo
        /// o UsuarioId do aluno. Esse campo é importante para funcionalidades
        /// como exibição da foto de perfil do aluno.
        /// </summary>
        private MatriculaDTO MapToDTO(Matricula m, string alunoNome, string turmaNome)
        {
            return new MatriculaDTO
            {
                Id = m.Id,
                AlunoId = m.AlunoId,
                UsuarioId = m.Aluno.UsuarioId,
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