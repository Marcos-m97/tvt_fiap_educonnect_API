using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class TurmaService : ITurmaService
    {
        private readonly ITurmaRepository _repo;
        private readonly ICursoRepository _cursoRepo;

        public TurmaService(ITurmaRepository repo, ICursoRepository cursoRepo)
        {
            _repo = repo;
            _cursoRepo = cursoRepo;
        }

        public async Task<TurmaDTO> Criar(CriarTurmaDTO dto)
        {
            var curso = await _cursoRepo.ObterPorId(dto.CursoId);
            if (curso == null)
                throw new Exception("Curso não encontrado.");

            var turma = new Turma
            {
                Nome = dto.Nome,
                Periodo = dto.Periodo,
                Semestre = dto.Semestre,
                CursoId = dto.CursoId
            };

            turma = await _repo.Criar(turma);

            return MapToDTO(turma);
        }

        public async Task<TurmaDTO?> ObterPorId(int id)
        {
            var turma = await _repo.ObterPorId(id);
            return turma == null ? null : MapToDTO(turma);
        }

        public async Task<IEnumerable<TurmaDTO>> Listar()
        {
            var list = await _repo.Listar();
            return list.Select(MapToDTO);
        }

        public async Task<IEnumerable<TurmaDTO>> ListarPorCurso(int cursoId)
        {
            var list = await _repo.ListarPorCurso(cursoId);
            return list.Select(MapToDTO);
        }

        public async Task<TurmaDTO?> Atualizar(int id, CriarTurmaDTO dto)
        {
            var turma = await _repo.ObterPorId(id);
            if (turma == null)
                return null;

            turma.Nome = dto.Nome;
            turma.Periodo = dto.Periodo;
            turma.Semestre = dto.Semestre;
            turma.CursoId = dto.CursoId;

            turma = await _repo.Atualizar(turma);

            return MapToDTO(turma);
        }

        public Task<bool> Deletar(int id)
        {
            return _repo.Deletar(id);
        }

        private TurmaDTO MapToDTO(Turma t)
        {
            return new TurmaDTO
            {
                Id = t.Id,
                Nome = t.Nome,
                Periodo = t.Periodo,
                Semestre = t.Semestre,
                CursoId = t.CursoId,
                CursoNome = t.Curso?.Nome ?? string.Empty
            };
        }
    }
}