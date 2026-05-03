using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas
    /// às entregas de atividades.
    ///
    /// No EduConnect, a entrega representa a submissão de uma atividade feita
    /// por um aluno. Essa camada coordena o envio de arquivo, correção pelo professor,
    /// listagem de entregas por atividade e consulta do histórico de entregas
    /// do próprio aluno.
    /// </summary>
    public class EntregaService : IEntregaService
    {
        private readonly IEntregaRepository _repo;
        private readonly IAtividadeRepository _atividadeRepo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly IArquivoStorageService _storage;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IEntregaRepository: acesso aos dados de entregas.
        /// IAtividadeRepository: valida e consulta a atividade relacionada.
        /// IAlunoRepository: localiza o aluno a partir do usuário autenticado.
        /// IArquivoStorageService: salva o arquivo enviado pelo aluno.
        /// </summary>
        public EntregaService(
            IEntregaRepository repo,
            IAtividadeRepository atividadeRepo,
            IAlunoRepository alunoRepo,
            IArquivoStorageService storage)
        {
            _repo = repo;
            _atividadeRepo = atividadeRepo;
            _alunoRepo = alunoRepo;
            _storage = storage;
        }

        // ============================================================
        // 1. CRIAR ENTREGA
        // ============================================================

        /// <summary>
        /// Cria uma nova entrega de atividade para o aluno logado.
        ///
        /// O método localiza o aluno a partir do ID do usuário autenticado,
        /// valida se a atividade existe, salva o arquivo enviado no storage
        /// e registra a entrega no banco de dados.
        /// </summary>
        public async Task<EntregaDTO> CriarEntrega(int usuarioId, int atividadeId, IFormFile arquivo)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new AppException("Aluno não encontrado.", 404);

            var atividade = await _atividadeRepo.ObterPorId(atividadeId)
                ?? throw new AppException("Atividade não encontrada.", 404);

            var caminho = await _storage.SalvarEntrega(atividadeId, aluno.Id, arquivo);

            var entrega = new EntregaAtividade
            {
                AlunoId = aluno.Id,
                AtividadeId = atividadeId,
                Arquivo = caminho
            };

            entrega = await _repo.Criar(entrega);

            return Map(
                entrega,
                atividade.Titulo,
                aluno.Usuario.Nome,
                aluno.Id,
                aluno.UsuarioId
            );
        }

        // ============================================================
        // 2. CORRIGIR ENTREGA
        // ============================================================

        /// <summary>
        /// Corrige uma entrega de atividade.
        ///
        /// Esse método é usado pelo professor para registrar nota e feedback.
        /// Após a alteração, a entrega é atualizada no banco e retornada como DTO.
        /// </summary>
        public async Task<EntregaDTO> Corrigir(int entregaId, decimal nota, string? feedback)
        {
            var entrega = await _repo.ObterPorId(entregaId)
                ?? throw new AppException("Entrega não encontrada.", 404);

            entrega.Nota = nota;
            entrega.FeedbackProfessor = feedback;

            entrega = await _repo.Atualizar(entrega);

            var atividade = await _atividadeRepo.ObterPorId(entrega.AtividadeId)
                ?? throw new AppException("Atividade não encontrada.", 404);

            var nomeAluno = entrega.Aluno.Usuario.Nome;

            return Map(
                entrega,
                atividade.Titulo,
                nomeAluno,
                entrega.AlunoId,
                entrega.Aluno.UsuarioId
            );
        }

        // ============================================================
        // 3. LISTAR ENTREGAS POR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Lista todas as entregas vinculadas a uma atividade.
        ///
        /// Esse método é utilizado principalmente na visão do professor,
        /// permitindo acompanhar quais alunos entregaram determinada atividade
        /// e quais entregas já foram corrigidas.
        /// </summary>
        public async Task<IEnumerable<EntregaDTO>> ListarPorAtividade(int atividadeId)
        {
            var lista = await _repo.ListarPorAtividade(atividadeId);

            return lista.Select(e => Map(
                e,
                e.Atividade.Titulo,
                e.Aluno.Usuario.Nome,
                e.AlunoId,
                e.Aluno.UsuarioId
            ));
        }

        // ============================================================
        // 4. LISTAR MINHAS ENTREGAS
        // ============================================================

        /// <summary>
        /// Lista o histórico de entregas do aluno logado.
        ///
        /// O método localiza o aluno a partir do usuário autenticado e permite
        /// aplicar filtros opcionais por disciplina e por atividade.
        ///
        /// O retorno inclui informações como atividade, disciplina, data de envio,
        /// nota, feedback do professor e arquivo enviado.
        /// </summary>
        public async Task<IEnumerable<EntregaAlunoDTO>> ListarMinhasEntregas(
            int usuarioId,
            int? disciplinaId,
            int? atividadeId)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new AppException("Aluno não encontrado.", 404);

            var entregas = await _repo.ListarPorAluno(aluno.Id);

            if (atividadeId.HasValue)
            {
                entregas = entregas
                    .Where(e => e.AtividadeId == atividadeId.Value)
                    .ToList();
            }

            if (disciplinaId.HasValue)
            {
                entregas = entregas
                    .Where(e =>
                        e.Atividade.TurmaDisciplina.DisciplinaId == disciplinaId.Value)
                    .ToList();
            }

            return entregas.Select(e => new EntregaAlunoDTO
            {
                EntregaId = e.Id,
                AtividadeId = e.AtividadeId,
                TituloAtividade = e.Atividade.Titulo,

                DisciplinaId = e.Atividade.TurmaDisciplina.DisciplinaId,
                NomeDisciplina = e.Atividade.TurmaDisciplina.Disciplina.Nome,

                DataEnvio = e.DataEnvio,
                Nota = e.Nota,
                FeedbackProfessor = e.FeedbackProfessor,
                Arquivo = e.Arquivo
            });
        }

        // ============================================================
        // 5. MAPEAMENTO PARA DTO
        // ============================================================

        /// <summary>
        /// Converte a entidade EntregaAtividade em EntregaDTO.
        ///
        /// Esse DTO é utilizado principalmente na visão do professor,
        /// trazendo dados da entrega, da atividade e do aluno em uma única resposta.
        /// </summary>
        private EntregaDTO Map(
            EntregaAtividade e,
            string titulo,
            string nomeAluno,
            int alunoId,
            int usuarioId)
        {
            return new EntregaDTO
            {
                Id = e.Id,
                AtividadeId = e.AtividadeId,

                AlunoId = alunoId,
                UsuarioId = usuarioId,
                NomeAluno = nomeAluno,

                TituloAtividade = titulo,
                Nota = e.Nota,
                FeedbackProfessor = e.FeedbackProfessor,
                DataEnvio = e.DataEnvio,
                Arquivo = e.Arquivo
            };
        }
    }
}