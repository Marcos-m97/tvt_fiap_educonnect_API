using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Services
{
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _repo;

        public CursoService(ICursoRepository repo)
        {
            _repo = repo;
        }

        public async Task<CursoDTO> Criar(CriarCursoDTO dto)
        {
            var curso = new Curso
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                CargaHoraria = dto.CargaHoraria,
                Ativo = true
            };

            curso = await _repo.Criar(curso);

            return MapToDTO(curso);
        }

        public async Task<PagedResultDTO<CursoDTO>> Listar(int page, int pageSize, string? search)
        {
            var query = _repo.Query();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nome.Contains(search));
            }

            var total = await query.CountAsync();

            var cursos = await query
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDTO<CursoDTO>
            {
                Data = cursos.Select(MapToDTO),
                Total = total
            };
        }

        public async Task<CursoDTO?> ObterPorId(int id)
        {
            var curso = await _repo.ObterPorId(id);
            return curso == null ? null : MapToDTO(curso);
        }

        public async Task<CursoDTO?> Atualizar(int id, CriarCursoDTO dto)
        {
            var curso = await _repo.ObterPorId(id);
            if (curso == null)
                return null;

            curso.Nome = dto.Nome;
            curso.Descricao = dto.Descricao;
            curso.CargaHoraria = dto.CargaHoraria;

            var atualizado = await _repo.Atualizar(curso);
            return MapToDTO(atualizado);
        }

        public Task<bool> Deletar(int id)
        {
            return _repo.Deletar(id);
        }

        public Task<bool> Reativar(int id)
        {
            return _repo.Reativar(id);
        }

        private CursoDTO MapToDTO(Curso c)
        {
            return new CursoDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                CargaHoraria = c.CargaHoraria,
                Ativo = c.Ativo
            };
        }
    }
}