using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas ao perfil administrativo.
    ///
    /// No EduConnect, o Admin é uma extensão da entidade Usuario e representa
    /// usuários com permissão para executar ações de gestão acadêmica, como
    /// criação de cursos, turmas, disciplinas, gerenciamento de usuários
    /// e acompanhamento de matrículas.
    ///
    /// Essa camada valida se o usuário vinculado realmente possui perfil administrativo,
    /// cria o registro de Admin, envia comunicação inicial por e-mail e transforma
    /// entidades em DTOs para retorno ao frontend.
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;
        private readonly IUsuarioRepository _usuarios;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IAdminRepository: acesso aos dados específicos do administrador.
        /// IUsuarioRepository: consulta do usuário vinculado ao perfil administrativo.
        /// IEmailService: envio de e-mails relacionados ao acesso administrativo.
        /// </summary>
        public AdminService(
            IAdminRepository repo,
            IUsuarioRepository usuarios,
            IEmailService emailService)
        {
            _repo = repo;
            _usuarios = usuarios;
            _emailService = emailService;
        }

        // ============================================================
        // 1. CRIAR ADMIN
        // ============================================================

        /// <summary>
        /// Cria um perfil administrativo vinculado a um usuário existente.
        ///
        /// Antes da criação, o sistema valida se o UsuarioId informado existe
        /// e se o usuário possui Tipo = 1, que representa o perfil Admin.
        ///
        /// Após criar o registro administrativo, o sistema envia um e-mail
        /// informando que o acesso administrativo foi criado.
        /// </summary>
        public async Task<AdminDTO> Criar(CriarAdminDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(dto.UsuarioId);

            if (usuario == null || usuario.Tipo != 1)
                throw new AppException("Usuário não é um administrador.", 400);

            var admin = new Admin
            {
                UsuarioId = dto.UsuarioId,
                Departamento = dto.Departamento,
                Cargo = dto.Cargo
            };

            admin = await _repo.Criar(admin);

            var assunto = "Seu acesso administrativo ao EduConnect foi criado";

            var corpo = $@"
Olá, {usuario.Nome}!

Seu acesso como administrador no EduConnect foi criado com sucesso.

Para realizar o primeiro acesso, utilize:

E-mail: {usuario.Email}

Com esse perfil, você poderá:
- Gerenciar usuários e perfis
- Aprovar matrículas
- Criar e manter cursos, turmas e disciplinas
- Acompanhar relatórios e boletins acadêmicos

🔐 Acesse o painel administrativo pelo link abaixo:
https://educonnect.app/admin

Após acessar, utilize a opção ""Redefinir senha"" para criar uma nova senha pessoal.

Em caso de dúvidas, nossa equipe está à disposição.

Atenciosamente,
Equipe EduConnect
";

            await _emailService.EnviarEmail(
                usuario.Email,
                assunto,
                corpo
            );

            return MapToDTO(admin);
        }

        // ============================================================
        // 2. OBTER ADMIN POR USUÁRIO
        // ============================================================

        /// <summary>
        /// Obtém o perfil administrativo a partir do ID do usuário.
        ///
        /// Esse método é útil quando o frontend possui o usuário autenticado
        /// e precisa localizar o cadastro administrativo correspondente.
        /// </summary>
        public async Task<AdminDTO?> ObterPorUsuario(int usuarioId)
        {
            var admin = await _repo.ObterPorUsuarioId(usuarioId);

            return admin == null ? null : MapToDTO(admin);
        }

        // ============================================================
        // 3. LISTAR ADMINS
        // ============================================================

        /// <summary>
        /// Lista todos os administradores cadastrados.
        ///
        /// As entidades retornadas pelo Repository são convertidas para DTO
        /// antes de serem enviadas ao frontend, evitando exposição direta
        /// das entidades do banco.
        /// </summary>
        public async Task<IEnumerable<AdminDTO>> Listar()
        {
            var admins = await _repo.Listar();

            return admins.Select(a => MapToDTO(a));
        }

        // ============================================================
        // 4. ATUALIZAR ADMIN
        // ============================================================

        /// <summary>
        /// Atualiza os dados específicos do perfil administrativo.
        ///
        /// Esse método altera informações complementares, como departamento
        /// e cargo. Os dados comuns do usuário, como nome e e-mail, permanecem
        /// na entidade Usuario.
        ///
        /// Caso o Admin não exista, retorna null para que o Controller trate
        /// a resposta como não encontrado.
        /// </summary>
        public async Task<AdminDTO?> Atualizar(int id, CriarAdminDTO dto)
        {
            var admin = await _repo.ObterPorId(id);

            if (admin == null)
                return null;

            admin.Departamento = dto.Departamento;
            admin.Cargo = dto.Cargo;

            admin = await _repo.Atualizar(admin);

            return MapToDTO(admin);
        }

        // ============================================================
        // 5. MAPEAMENTO PARA DTO
        // ============================================================

        /// <summary>
        /// Converte a entidade Admin em AdminDTO.
        ///
        /// Esse mapeamento combina informações específicas do Admin com dados
        /// básicos do Usuario vinculado, como nome e e-mail.
        /// </summary>
        private AdminDTO MapToDTO(Admin admin)
        {
            return new AdminDTO
            {
                Id = admin.Id,
                UsuarioId = admin.UsuarioId,
                Nome = admin.Usuario.Nome,
                Email = admin.Usuario.Email,
                Departamento = admin.Departamento,
                Cargo = admin.Cargo
            };
        }
    }
}