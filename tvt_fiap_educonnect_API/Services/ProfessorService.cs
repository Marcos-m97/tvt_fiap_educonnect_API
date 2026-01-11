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
        private readonly IEmailService _emailService;

        public ProfessorService(
            IProfessorRepository repo,
            IUsuarioRepository usuarios,
            IEmailService emailService
        )
        {
            _repo = repo;
            _usuarios = usuarios;
            _emailService = emailService;
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

            // ==============================================================
            // ENVIO DE EMAIL – PERFIL DE PROFESSOR CRIADO
            // ==============================================================
            var assunto = "Seu acesso como professor no EduConnect foi criado";

            var corpo = $@"
Olá, {usuario.Nome}!

Seu perfil de professor no EduConnect foi criado com sucesso.

A partir de agora, você já pode acessar a plataforma para:
- Visualizar suas turmas e disciplinas
- Publicar atividades
- Avaliar entregas dos alunos
- Acompanhar boletins e desempenho

🔐 Acesse a plataforma pelo link abaixo:
https://educonnect.app/login

Caso seja seu primeiro acesso, utilize o e-mail cadastrado e a senha definida no momento do cadastro.

Se tiver qualquer dúvida, nossa equipe administrativa está à disposição.

Atenciosamente,
Equipe EduConnect
";

            await _emailService.EnviarEmail(
                usuario.Email,
                assunto,
                corpo
            );

            return MapToDTO(prof);
        }

        public async Task<ProfessorDTO?> ObterPorUsuario(int usuarioId)
        {
            var prof = await _repo.ObterPorUsuarioId(usuarioId);
            return prof == null ? null : MapToDTO(prof);
        }

        public async Task<IEnumerable<ProfessorDTO>> Listar()
        {
            var lista = await _repo.Listar();
            return lista.Select(p => MapToDTO(p));
        }

        public async Task<ProfessorDTO?> Atualizar(int id, CriarProfessorDTO dto)
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
