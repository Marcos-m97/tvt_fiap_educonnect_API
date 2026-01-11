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
        private readonly IEmailService _emailService;

        public AlunoService(
            IAlunoRepository repo,
            IUsuarioRepository usuarios,
            IEmailService emailService
        )
        {
            _repo = repo;
            _usuarios = usuarios;
            _emailService = emailService;
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

            // ==============================================================
            // ENVIO DE EMAIL – CADASTRO CONCLUÍDO (ALUNO)
            // ==============================================================
            var assunto = "Bem-vindo ao EduConnect – Cadastro concluído";

            var corpo = $@"
Olá, {usuario.Nome}!

Seu cadastro como aluno no EduConnect foi concluído com sucesso.

📌 Próximo passo:
Para ter acesso às disciplinas, atividades e boletim, é necessário concluir o processo de matrícula.

👉 Acesse o link abaixo para continuar:
https://educonnect.app/matricula

Após a aprovação da matrícula, você receberá um novo e-mail confirmando o acesso completo à plataforma.

🔐 Caso precise acessar sua conta:
https://educonnect.app/login

Se tiver qualquer dúvida, nossa equipe estará à disposição.

Atenciosamente,
Equipe EduConnect
";

            await _emailService.EnviarEmail(
                usuario.Email,
                assunto,
                corpo
            );

            return MapToDTO(aluno);
        }

        public async Task<AlunoDTO?> ObterPorUsuario(int usuarioId)
        {
            var aluno = await _repo.ObterPorUsuarioId(usuarioId);
            return aluno == null ? null : MapToDTO(aluno);
        }

        public async Task<IEnumerable<AlunoDTO>> Listar()
        {
            var lista = await _repo.Listar();
            return lista.Select(a => MapToDTO(a));
        }

        public async Task<AlunoDTO?> Atualizar(int id, CriarAlunoDTO dto)
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
