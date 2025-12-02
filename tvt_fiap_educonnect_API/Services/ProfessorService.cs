using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class ProfessorService : IProfessorService
    {
        private readonly IProfessorRepository _repo;
        private readonly IUsuarioRepository _usuarios;

        public ProfessorService(IProfessorRepository repo, IUsuarioRepository usuarios)
        {
            _repo = repo;
            _usuarios = usuarios;
        }

        public async Task<ProfessorDTO> Criar(CriarProfessorDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(dto.UsuarioId);

            if (usuario == null || usuario.Tipo != 2)
                throw new Exception("Usuário não é um professor.");

            var prof = new Professor
            {
                UsuarioId = dto.UsuarioId,
                Especialidade = dto.Especialidade,
                Formacao = dto.Formacao,
                CurriculoLattes = dto.CurriculoLattes
            };

            prof = await _repo.Criar(prof);

            return MapToDTO(prof);
        }

        public async Task<ProfessorDTO?> ObterPorUsuario(Guid usuarioId)
        {
            var prof = await _repo.ObterPorUsuarioId(usuarioId);
            return prof == null ? null : MapToDTO(prof);
        }

        public async Task<IEnumerable<ProfessorDTO>> Listar()
        {
            var lista = await _repo.Listar();
            return lista.Select(p => MapToDTO(p));
        }

        public async Task<ProfessorDTO?> Atualizar(Guid id, CriarProfessorDTO dto)
        {
            var prof = await _repo.ObterPorId(id);

            if (prof == null)
                return null;

            prof.Especialidade = dto.Especialidade;
            prof.Formacao = dto.Formacao;
            prof.CurriculoLattes = dto.CurriculoLattes;

            prof = await _repo.Atualizar(prof);

            return MapToDTO(prof);
        }

        private ProfessorDTO MapToDTO(Professor p)
        {
            return new ProfessorDTO
            {
                Id = p.Id,
                UsuarioId = p.UsuarioId,
                Nome = p.Usuario.Nome,
                Email = p.Usuario.Email,
                Especialidade = p.Especialidade,
                Formacao = p.Formacao,
                CurriculoLattes = p.CurriculoLattes
            };
        }
    }
}
