using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using tvt_fiap_educonnect_API.Models.DTOs;

namespace EduConnect_API.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _repo;
        private readonly ITurmaRepository _turmas;
        private readonly ITurmaDisciplinaRepository _tdRepo;

        public EventoService(
            IEventoRepository repo,
            ITurmaRepository turmas,
            ITurmaDisciplinaRepository tdRepo)
        {
            _repo = repo;
            _turmas = turmas;
            _tdRepo = tdRepo;
        }

        public async Task<EventoDTO> Criar(Guid criadorId, CriarEventoDTO dto)
        {
            if (dto.TurmaId != null)
            {
                var turma = await _turmas.ObterPorId(dto.TurmaId.Value);
                if (turma == null) throw new Exception("Turma não encontrada.");
            }

            if (dto.TurmaDisciplinaId != null)
            {
                var td = await _tdRepo.ObterPorId(dto.TurmaDisciplinaId.Value);
                if (td == null) throw new Exception("TurmaDisciplina não encontrada.");
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

        public async Task<EventoDTO?> Obter(Guid id)
        {
            var e = await _repo.Obter(id);
            return e == null ? null : MapToDTO(e);
        }

        public async Task<IEnumerable<EventoDTO>> Listar()
        {
            return (await _repo.Listar()).Select(MapToDTO);
        }

        public async Task<IEnumerable<EventoDTO>> ListarPorTurma(Guid turmaId)
        {
            return (await _repo.ListarPorTurma(turmaId)).Select(MapToDTO);
        }

        public async Task<EventoDTO?> Atualizar(Guid id, CriarEventoDTO dto)
        {
            var e = await _repo.Obter(id);
            if (e == null) return null;

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

        public Task<bool> Deletar(Guid id)
        {
            return _repo.Deletar(id);
        }

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

