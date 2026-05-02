using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas ao professor.
    ///
    /// No EduConnect, o professor representa o usuário responsável por ministrar
    /// disciplinas, gerenciar aulas, criar atividades e acompanhar entregas dos alunos.
    ///
    /// Essa camada valida se o usuário vinculado realmente possui perfil de professor,
    /// cria o registro docente, envia comunicação inicial por e-mail e transforma
    /// entidades em DTOs para retorno ao frontend.
    /// </summary>
    public class ProfessorService : IProfessorService
    {
        private readonly IProfessorRepository _repo;
        private readonly IUsuarioRepository _usuarios;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IProfessorRepository: acesso aos dados específicos do professor.
        /// IUsuarioRepository: consulta do usuário vinculado ao professor.
        /// IEmailService: envio de e-mails relacionados ao fluxo do professor.
        /// </summary>
        public ProfessorService(
            IProfessorRepository repo,
            IUsuarioRepository usuarios,
            IEmailService emailService)
        {
            _repo = repo;
            _usuarios = usuarios;
            _emailService = emailService;
        }

        // ============================================================
        // 1. CRIAR PROFESSOR
        // ============================================================

        /// <summary>
        /// Cria um perfil de professor vinculado a um usuário existente.
        ///
        /// Antes da criação, o sistema valida se o UsuarioId informado existe
        /// e se o usuário possui Tipo = 2, que representa o perfil de professor.
        ///
        /// Também valida se esse usuário já possui cadastro docente, evitando
        /// duplicidade de professor para o mesmo usuário.
        ///
        /// Após a criação, o sistema envia um e-mail informando que o acesso
        /// como professor foi criado.
        /// </summary>
        public async Task<ProfessorDTO> Criar(CriarProfessorDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(dto.UsuarioId);

            if (usuario == null || usuario.Tipo != 2)
                throw new AppException("Usuário não é um professor.", 400);

            var professorExistente = await _repo.ObterPorUsuarioId(dto.UsuarioId);

            if (professorExistente != null)
                throw new AppException("Este usuário já possui cadastro de professor.", 400);

            var prof = new Professor
            {
                UsuarioId = dto.UsuarioId,
                Especialidade = dto.Especialidade,
                Formacao = dto.Formacao,
                CurriculoLattes = dto.CurriculoLattes
            };

            prof = await _repo.Criar(prof);

            var assunto = "Seu acesso como professor no EduConnect foi criado";

            var corpo = $@"
Olá, {usuario.Nome}!

Seu perfil de professor no EduConnect foi criado com sucesso.

Para realizar o primeiro acesso, utilize:

E-mail: {usuario.Email}

🔐 Acesse a plataforma pelo link abaixo:
https://educonnect.app/login

Após acessar, utilize a opção ""Redefinir senha"" para criar uma nova senha pessoal.

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

        // ============================================================
        // 2. OBTER PROFESSOR POR USUÁRIO
        // ============================================================

        /// <summary>
        /// Obtém o perfil de professor a partir do ID do usuário.
        ///
        /// Esse método é útil para fluxos em que o frontend possui os dados
        /// do usuário autenticado e precisa localizar o cadastro docente
        /// correspondente na tabela de professores.
        /// </summary>
        public async Task<ProfessorDTO?> ObterPorUsuario(int usuarioId)
        {
            var prof = await _repo.ObterPorUsuarioId(usuarioId);

            return prof == null ? null : MapToDTO(prof);
        }

        // ============================================================
        // 3. LISTAR PROFESSORES
        // ============================================================

        /// <summary>
        /// Lista todos os professores cadastrados.
        ///
        /// As entidades retornadas pelo repositório são convertidas para DTO
        /// antes de serem enviadas ao frontend, evitando exposição direta
        /// das entidades do banco.
        /// </summary>
        public async Task<IEnumerable<ProfessorDTO>> Listar()
        {
            var lista = await _repo.Listar();

            return lista.Select(p => MapToDTO(p));
        }

        // ============================================================
        // 4. ATUALIZAR PROFESSOR
        // ============================================================

        /// <summary>
        /// Atualiza os dados acadêmicos do professor.
        ///
        /// Esse método altera informações complementares do perfil docente,
        /// como especialidade, formação e currículo Lattes.
        ///
        /// Caso o professor não exista, retorna null para que o Controller trate
        /// a resposta como não encontrado.
        /// </summary>
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

        // ============================================================
        // 5. MAPEAMENTO PARA DTO
        // ============================================================

        /// <summary>
        /// Converte a entidade Professor em ProfessorDTO.
        ///
        /// Esse mapeamento centraliza a montagem dos dados retornados ao frontend,
        /// combinando informações específicas do professor com dados básicos
        /// do usuário, como nome e e-mail.
        /// </summary>
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