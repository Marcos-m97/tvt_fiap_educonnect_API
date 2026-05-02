using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas ao aluno.
    ///
    /// No EduConnect, o aluno representa o perfil acadêmico do usuário que acessa
    /// disciplinas, aulas, atividades, boletim e matrícula.
    ///
    /// Essa camada valida se o usuário vinculado realmente possui perfil de aluno,
    /// cria o registro acadêmico, envia comunicação inicial por e-mail e transforma
    /// as entidades em DTOs para retorno ao frontend.
    /// </summary>
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _repo;
        private readonly IUsuarioRepository _usuarios;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IAlunoRepository: acesso aos dados específicos do aluno.
        /// IUsuarioRepository: consulta do usuário vinculado ao aluno.
        /// IEmailService: envio de e-mails relacionados ao fluxo do aluno.
        /// </summary>
        public AlunoService(
            IAlunoRepository repo,
            IUsuarioRepository usuarios,
            IEmailService emailService)
        {
            _repo = repo;
            _usuarios = usuarios;
            _emailService = emailService;
        }

        // ============================================================
        // 1. CRIAR ALUNO
        // ============================================================

        /// <summary>
        /// Cria um perfil de aluno vinculado a um usuário existente.
        ///
        /// Antes da criação, o sistema valida se o UsuarioId informado existe
        /// e se o usuário possui Tipo = 3, que representa o perfil de aluno.
        ///
        /// Após criar o registro, o sistema envia um e-mail informando que o cadastro
        /// como aluno foi concluído e orientando o próximo passo: realizar a matrícula.
        /// </summary>
        public async Task<AlunoDTO> Criar(CriarAlunoDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(dto.UsuarioId);

            if (usuario == null || usuario.Tipo != 3)
                throw new AppException("Usuário não é um aluno.", 400);

            var aluno = new Aluno
            {
                UsuarioId = dto.UsuarioId,
                CPF = dto.CPF,
                DataNascimento = dto.DataNascimento,
                Endereco = dto.Endereco
            };

            aluno = await _repo.Criar(aluno);

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

        // ============================================================
        // 2. OBTER ALUNO POR USUÁRIO
        // ============================================================

        /// <summary>
        /// Obtém o perfil de aluno a partir do ID do usuário.
        ///
        /// Esse método é útil para fluxos em que o frontend possui os dados
        /// do usuário autenticado e precisa localizar o cadastro acadêmico
        /// correspondente na tabela de alunos.
        /// </summary>
        public async Task<AlunoDTO?> ObterPorUsuario(int usuarioId)
        {
            var aluno = await _repo.ObterPorUsuarioId(usuarioId);

            return aluno == null ? null : MapToDTO(aluno);
        }

        // ============================================================
        // 3. LISTAR ALUNOS
        // ============================================================

        /// <summary>
        /// Lista todos os alunos cadastrados.
        ///
        /// As entidades retornadas pelo repositório são convertidas para DTO
        /// antes de serem enviadas ao frontend, evitando exposição direta
        /// das entidades do banco.
        /// </summary>
        public async Task<IEnumerable<AlunoDTO>> Listar()
        {
            var lista = await _repo.Listar();

            return lista.Select(a => MapToDTO(a));
        }

        // ============================================================
        // 4. ATUALIZAR ALUNO
        // ============================================================

        /// <summary>
        /// Atualiza os dados pessoais do aluno.
        ///
        /// Esse método altera informações complementares do perfil acadêmico,
        /// como CPF, data de nascimento e endereço.
        ///
        /// Caso o aluno não exista, retorna null para que o Controller trate
        /// a resposta como não encontrado.
        /// </summary>
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

        // ============================================================
        // 5. MAPEAMENTO PARA DTO
        // ============================================================

        /// <summary>
        /// Converte a entidade Aluno em AlunoDTO.
        ///
        /// Esse mapeamento centraliza a montagem dos dados retornados ao frontend,
        /// combinando informações específicas do aluno com dados básicos do usuário,
        /// como nome e e-mail.
        /// </summary>
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