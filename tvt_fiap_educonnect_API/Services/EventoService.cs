using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas aos eventos.
    ///
    /// No EduConnect, eventos representam compromissos acadêmicos ou administrativos,
    /// como provas, atividades, aulas extras, reuniões ou avisos gerais.
    ///
    /// Essa camada coordena criação, consulta, listagem, atualização, exclusão
    /// e listagem dos eventos disponíveis para o aluno conforme sua matrícula ativa.
    /// </summary>
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _repo;
        private readonly ITurmaRepository _turmas;
        private readonly ITurmaDisciplinaRepository _tdRepo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly IMatriculaRepository _matriculaRepo;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IEventoRepository: acesso aos dados de eventos.
        /// ITurmaRepository: valida e consulta a turma vinculada ao evento.
        /// ITurmaDisciplinaRepository: valida o vínculo turma/disciplina.
        /// IAlunoRepository: localiza o aluno a partir do usuário autenticado.
        /// IMatriculaRepository: verifica a matrícula ativa do aluno.
        /// </summary>
        public EventoService(
            IEventoRepository repo,
            ITurmaRepository turmas,
            ITurmaDisciplinaRepository tdRepo,
            IAlunoRepository alunoRepo,
            IMatriculaRepository matriculaRepo)
        {
            _repo = repo;
            _turmas = turmas;
            _tdRepo = tdRepo;
            _alunoRepo = alunoRepo;
            _matriculaRepo = matriculaRepo;
        }

        // =========================================================
        // 1. CRIAR EVENTO
        // =========================================================

        /// <summary>
        /// Cria um novo evento acadêmico ou administrativo.
        ///
        /// O evento pode ser geral, vinculado a uma turma ou vinculado a uma
        /// TurmaDisciplina específica.
        ///
        /// Quando TurmaId ou TurmaDisciplinaId são informados, o Service valida
        /// se esses registros existem antes de salvar o evento.
        /// </summary>
        public async Task<EventoDTO> Criar(int criadorId, CriarEventoDTO dto)
        {
            if (dto.TurmaId != null)
            {
                var turma = await _turmas.ObterPorId(dto.TurmaId.Value);

                if (turma == null)
                    throw new AppException("Turma não encontrada.", 404);
            }

            if (dto.TurmaDisciplinaId != null)
            {
                var td = await _tdRepo.ObterPorId(dto.TurmaDisciplinaId.Value);

                if (td == null)
                    throw new AppException("TurmaDisciplina não encontrada.", 404);
            }

            var evento = new Evento
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Inicio = dto.Inicio,
                Fim = dto.Fim,
                Tipo = dto.Tipo,
                TurmaId = dto.TurmaId,
                TurmaDisciplinaId = dto.TurmaDisciplinaId,
                CriadoPorId = criadorId
            };

            evento = await _repo.Criar(evento);

            return MapToDTO(evento);
        }

        // =========================================================
        // 2. LISTAR MEUS EVENTOS
        // =========================================================

        /// <summary>
        /// Lista os eventos disponíveis para o aluno logado.
        ///
        /// O método localiza o aluno a partir do usuário autenticado,
        /// verifica sua matrícula ativa e busca os eventos relacionados à turma
        /// dessa matrícula.
        ///
        /// A listagem considera eventos vinculados diretamente à turma e eventos
        /// vinculados a disciplinas daquela turma.
        /// </summary>
        public async Task<IEnumerable<EventoDTO>> ListarMeusEventos(int usuarioId)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new AppException("Aluno não encontrado.", 404);

            var matricula = await _matriculaRepo.ObterAtivaPorAlunoId(aluno.Id)
                ?? throw new AppException("Aluno não possui matrícula ativa.", 404);

            var eventos = await _repo.ListarPorTurma(matricula.TurmaId);

            return eventos.Select(MapToDTO);
        }

        // =========================================================
        // 3. CONSULTAS
        // =========================================================

        /// <summary>
        /// Obtém um evento pelo ID.
        ///
        /// Retorna null quando o evento não é encontrado, permitindo que o Controller
        /// responda com NotFound.
        /// </summary>
        public async Task<EventoDTO?> Obter(int id)
        {
            var e = await _repo.Obter(id);

            return e == null ? null : MapToDTO(e);
        }

        /// <summary>
        /// Lista todos os eventos cadastrados.
        ///
        /// Usado em visões administrativas ou gerais de calendário.
        /// </summary>
        public async Task<IEnumerable<EventoDTO>> Listar()
        {
            return (await _repo.Listar()).Select(MapToDTO);
        }

        /// <summary>
        /// Lista eventos relacionados a uma turma específica.
        ///
        /// Esse método retorna tanto eventos vinculados diretamente à turma
        /// quanto eventos de disciplinas pertencentes a essa turma.
        /// </summary>
        public async Task<IEnumerable<EventoDTO>> ListarPorTurma(int turmaId)
        {
            return (await _repo.ListarPorTurma(turmaId)).Select(MapToDTO);
        }

        // =========================================================
        // 4. ATUALIZAR EVENTO
        // =========================================================

        /// <summary>
        /// Atualiza um evento existente, considerando regra de permissão.
        ///
        /// Administradores podem atualizar eventos de forma geral.
        /// Professores, quando identificados pela role 2, só podem editar eventos
        /// criados por eles mesmos.
        ///
        /// Também valida se a nova turma ou TurmaDisciplina informada existe.
        /// </summary>
        public async Task<EventoDTO?> Atualizar(
            int id,
            int usuarioId,
            string role,
            CriarEventoDTO dto)
        {
            var e = await _repo.Obter(id);

            if (e == null)
                return null;

            if (role == "2" && e.CriadoPorId != usuarioId)
                throw new AppException("Você não tem permissão para editar este evento.", 403);

            if (dto.TurmaId != null)
            {
                var turma = await _turmas.ObterPorId(dto.TurmaId.Value);

                if (turma == null)
                    throw new AppException("Turma não encontrada.", 404);
            }

            if (dto.TurmaDisciplinaId != null)
            {
                var td = await _tdRepo.ObterPorId(dto.TurmaDisciplinaId.Value);

                if (td == null)
                    throw new AppException("TurmaDisciplina não encontrada.", 404);
            }

            e.Titulo = dto.Titulo;
            e.Descricao = dto.Descricao;
            e.Inicio = dto.Inicio;
            e.Fim = dto.Fim;
            e.Tipo = dto.Tipo;
            e.TurmaId = dto.TurmaId;
            e.TurmaDisciplinaId = dto.TurmaDisciplinaId;

            e = await _repo.Atualizar(e);

            return MapToDTO(e);
        }

        // =========================================================
        // 5. DELETAR EVENTO
        // =========================================================

        /// <summary>
        /// Remove um evento, considerando regra de permissão.
        ///
        /// Administradores podem deletar eventos de forma geral.
        /// Professores, quando identificados pela role 2, só podem excluir eventos
        /// criados por eles mesmos.
        /// </summary>
        public async Task<bool> Deletar(int id, int usuarioId, string role)
        {
            var e = await _repo.Obter(id);

            if (e == null)
                return false;

            if (role == "2" && e.CriadoPorId != usuarioId)
                throw new AppException("Você não tem permissão para deletar este evento.", 403);

            return await _repo.Deletar(id);
        }

        // =========================================================
        // 6. MAPEAMENTO PARA DTO
        // =========================================================

        /// <summary>
        /// Converte a entidade Evento em EventoDTO.
        ///
        /// O DTO retorna os dados principais do evento e também nomes descritivos
        /// de turma e disciplina quando esses relacionamentos estiverem carregados.
        /// </summary>
        private EventoDTO MapToDTO(Evento e)
        {
            return new EventoDTO
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Descricao = e.Descricao,
                Inicio = e.Inicio,
                Fim = e.Fim,
                Tipo = e.Tipo,
                TurmaId = e.TurmaId,
                TurmaNome = e.Turma?.Nome ?? "",
                TurmaDisciplinaId = e.TurmaDisciplinaId,
                DisciplinaNome = e.TurmaDisciplina?.Disciplina?.Nome ?? ""
            };
        }
    }
}