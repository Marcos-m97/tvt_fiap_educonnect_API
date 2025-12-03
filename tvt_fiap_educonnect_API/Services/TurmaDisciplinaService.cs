using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using tvt_fiap_educonnect_API.Models.DTOs;

namespace EduConnect_API.Services
{
    public class TurmaDisciplinaService : ITurmaDisciplinaService
    {
        private readonly ITurmaDisciplinaRepository _repo;
        private readonly ITurmaRepository _turmaRepo;
        private readonly IDisciplinaRepository _disciplinaRepo;
        private readonly IProfessorRepository _professorRepo;

        public TurmaDisciplinaService(
            ITurmaDisciplinaRepository repo,
            ITurmaRepository turmaRepo,
            IDisciplinaRepository disciplinaRepo,
            IProfessorRepository professorRepo)
        {
            _repo = repo;
            _turmaRepo = turmaRepo;
            _disciplinaRepo = disciplinaRepo;
            _professorRepo = professorRepo;
        }

        public async Task<TurmaDisciplinaDTO> Criar(CriarTurmaDisciplinaDTO dto)
        {
            // valida FK Turma
            var turma = await _turmaRepo.ObterPorId(dto.TurmaId)
                ?? throw new Exception("Turma não encontrada.");

            // valida FK Disciplina
            var disciplina = await _disciplinaRepo.ObterPorId(dto.DisciplinaId)
                ?? throw new Exception("Disciplina não encontrada.");

            // valida FK Professor
            var professor = await _professorRepo.ObterPorId(dto.ProfessorId)
                ?? throw new Exception("Professor não encontrado.");

            var entity = new TurmaDisciplina
            {
                TurmaId = dto.TurmaId,
                DisciplinaId = dto.DisciplinaId,
                ProfessorId = dto.ProfessorId
            };

            entity = await _repo.Criar(entity);

            return MapToDTO(entity, turma.Nome, disciplina.Nome, professor.Usuario.Nome);
        }

        public async Task<TurmaDisciplinaDTO?> ObterPorId(Guid id)
        {
            var entity = await _repo.ObterPorId(id);
            if (entity == null) return null;

            return MapToDTO(
                entity,
                entity.Turma.Nome,
                entity.Disciplina.Nome,
                entity.Professor.Usuario.Nome
            );
        }

        public async Task<IEnumerable<TurmaDisciplinaDTO>> Listar()
        {
            var list = await _repo.Listar();
            return list.Select(e =>
                MapToDTO(e, e.Turma.Nome, e.Disciplina.Nome, e.Professor.Usuario.Nome));
        }

        public async Task<IEnumerable<TurmaDisciplinaDTO>> ListarPorTurma(Guid turmaId)
        {
            var list = await _repo.ListarPorTurma(turmaId);
            return list.Select(e =>
                MapToDTO(e, e.Turma.Nome, e.Disciplina.Nome, e.Professor.Usuario.Nome));
        }

        public async Task<TurmaDisciplinaDTO?> Atualizar(Guid id, CriarTurmaDisciplinaDTO dto)
        {
            var entity = await _repo.ObterPorId(id);
            if (entity == null) return null;

            entity.TurmaId = dto.TurmaId;
            entity.DisciplinaId = dto.DisciplinaId;
            entity.ProfessorId = dto.ProfessorId;

            entity = await _repo.Atualizar(entity);

            return MapToDTO(
                entity,
                entity.Turma.Nome,
                entity.Disciplina.Nome,
                entity.Professor.Usuario.Nome
            );
        }

        public Task<bool> Deletar(Guid id)
        {
            return _repo.Deletar(id);
        }

        private TurmaDisciplinaDTO MapToDTO(TurmaDisciplina e, string turmaNome, string disciplinaNome, string professorNome)
        {
            return new TurmaDisciplinaDTO
            {
                Id = e.Id,
                TurmaId = e.TurmaId,
                TurmaNome = turmaNome,
                DisciplinaId = e.DisciplinaId,
                DisciplinaNome = disciplinaNome,
                ProfessorId = e.ProfessorId,
                ProfessorNome = professorNome
            };
        }
    }
}