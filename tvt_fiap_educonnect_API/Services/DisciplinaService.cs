using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class DisciplinaService : IDisciplinaService
    {
        private readonly IDisciplinaRepository _repo;
        private readonly ICursoRepository _cursoRepo;

        public DisciplinaService(IDisciplinaRepository repo, ICursoRepository cursoRepo)
        {
            _repo = repo;
            _cursoRepo = cursoRepo;
        }

        public async Task<DisciplinaDTO> Criar(CriarDisciplinaDTO dto)
        {
            var curso = await _cursoRepo.ObterPorId(dto.CursoId);
            if (curso == null)
                throw new Exception("Curso não encontrado.");

            var disciplina = new Disciplina
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                CargaHoraria = dto.CargaHoraria,
                CursoId = dto.CursoId,
                Ativo = true
            };

            disciplina = await _repo.Criar(disciplina);

            return MapToDTO(disciplina);
        }

        public async Task<IEnumerable<DisciplinaDTO>> Listar()
        {
            var list = await _repo.Listar();
            return list.Select(MapToDTO);
        }

        public async Task<IEnumerable<DisciplinaDTO>> ListarPorCurso(int cursoId)
        {
            var list = await _repo.ListarPorCurso(cursoId);
            return list.Select(MapToDTO);
        }

        public async Task<DisciplinaDTO?> ObterPorId(int id)
        {
            var disciplina = await _repo.ObterPorId(id);
            return disciplina == null ? null : MapToDTO(disciplina);
        }

        public async Task<DisciplinaDTO?> Atualizar(int id, CriarDisciplinaDTO dto)
        {
            var disciplina = await _repo.ObterPorId(id);
            if (disciplina == null)
                return null;

            disciplina.Nome = dto.Nome;
            disciplina.Descricao = dto.Descricao;
            disciplina.CargaHoraria = dto.CargaHoraria;
            disciplina.CursoId = dto.CursoId;

            disciplina = await _repo.Atualizar(disciplina);

            return MapToDTO(disciplina);
        }

        // 🔥 SOFT DELETE
        public async Task<bool> Deletar(int id)
        {
            return await _repo.Deletar(id);
        }

        // 🔥 REATIVAR
        public async Task<bool> Reativar(int id)
        {
            var disciplina = await _repo.ObterPorId(id);
            if (disciplina == null)
                return false;

            disciplina.Ativo = true;

            await _repo.Atualizar(disciplina);

            return true;
        }

        private DisciplinaDTO MapToDTO(Disciplina d)
        {
            return new DisciplinaDTO
            {
                Id = d.Id,
                Nome = d.Nome,
                Descricao = d.Descricao,
                CargaHoraria = d.CargaHoraria,
                CursoId = d.CursoId,
                CursoNome = d.Curso?.Nome ?? string.Empty,
                Ativo = d.Ativo
            };
        }
    }
}