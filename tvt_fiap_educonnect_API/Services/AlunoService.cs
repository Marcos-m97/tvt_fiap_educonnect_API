using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _repo;
        private readonly IUsuarioRepository _usuarios;

        public AlunoService(IAlunoRepository repo, IUsuarioRepository usuarios)
        {
            _repo = repo;
            _usuarios = usuarios;
        }

        public async Task<AlunoDTO> Criar(CriarAlunoDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(dto.UsuarioId);

            if (usuario == null || usuario.Tipo != 3)
                throw new Exception("Usuário não é um aluno.");

            var aluno = new Aluno
            {
                UsuarioId = dto.UsuarioId,
                CPF = dto.CPF,
                DataNascimento = dto.DataNascimento,
                Endereco = dto.Endereco
            };

            aluno = await _repo.Criar(aluno);

            return MapToDTO(aluno);
        }

        public async Task<AlunoDTO?> ObterPorUsuario(Guid usuarioId)
        {
            var aluno = await _repo.ObterPorUsuarioId(usuarioId);
            return aluno == null ? null : MapToDTO(aluno);
        }

        public async Task<IEnumerable<AlunoDTO>> Listar()
        {
            var lista = await _repo.Listar();
            return lista.Select(a => MapToDTO(a));
        }

        public async Task<AlunoDTO?> Atualizar(Guid id, CriarAlunoDTO dto)
        {
            var aluno = await _repo.ObterPorId(id);

            if (aluno == null)
                return null;

            aluno.CPF = dto.CPF;
            aluno.DataNascimento = dto.DataNascimento;
            aluno.Endereco = dto.Endereco;

            aluno = await _repo.Atualizar(aluno);

            return MapToDTO(aluno);
        }

        private AlunoDTO MapToDTO(Aluno a)
        {
            return new AlunoDTO
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                Nome = a.Usuario.Nome,
                Email = a.Usuario.Email,
                CPF = a.CPF,
                DataNascimento = a.DataNascimento,
                Endereco = a.Endereco
            };
        }
    }
}