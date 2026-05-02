using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas aos usuários.
    ///
    /// No contexto do EduConnect, essa camada faz a ponte entre o Controller
    /// e os Repositories, mantendo fora do Controller regras como:
    /// autenticação, criptografia de senha, criação de usuário, soft delete,
    /// recuperação de senha e atualização de foto de perfil.
    ///
    /// A separação em Service ajuda a manter o Controller focado apenas em receber
    /// requisições HTTP e devolver respostas para o frontend.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Recebe as dependências necessárias por injeção de dependência.
        ///
        /// IUsuarioRepository: acesso aos dados de usuário.
        /// IPasswordResetRepository: persistência dos códigos de recuperação de senha.
        /// IEmailService: envio de e-mails relacionados ao usuário.
        /// </summary>
        public UsuarioService(
            IUsuarioRepository repo,
            IPasswordResetRepository passwordResetRepository,
            IEmailService emailService)
        {
            _repo = repo;
            _passwordResetRepository = passwordResetRepository;
            _emailService = emailService;
        }

        // ============================================================
        // 1. LOGIN
        // ============================================================

        /// <summary>
        /// Realiza o login do usuário validando e-mail, senha e status ativo.
        ///
        /// O sistema localiza o usuário pelo e-mail, verifica se ele está ativo
        /// e compara a senha informada com o hash salvo no banco usando BCrypt.
        ///
        /// Como o EduConnect utiliza soft delete, usuários desativados continuam
        /// cadastrados no banco, porém não devem conseguir acessar a plataforma.
        /// </summary>
        public async Task<Usuario?> Login(LoginDTO dto)
        {
            var user = await _repo.ObterPorEmail(dto.Email);

            if (user == null || !user.Ativo)
                return null;

            bool senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, user.SenhaHash);

            if (!senhaValida)
                return null;

            return user;
        }

        // ============================================================
        // 2. OBTER POR ID (/ME)
        // ============================================================

        /// <summary>
        /// Obtém um usuário pelo identificador único.
        ///
        /// Esse método é utilizado em fluxos como consulta de perfil,
        /// edição de usuário, validações internas e recuperação de dados
        /// do usuário autenticado.
        /// </summary>
        public async Task<Usuario?> ObterPorId(int id)
        {
            return await _repo.ObterPorId(id);
        }

        // ============================================================
        // 3. CRIAR USUÁRIO
        // ============================================================

        /// <summary>
        /// Cria um novo usuário no sistema.
        ///
        /// Antes de salvar, valida se já existe outro usuário com o mesmo e-mail,
        /// pois o e-mail é utilizado como credencial de login.
        ///
        /// A senha recebida no DTO é convertida para hash com BCrypt antes
        /// de ser persistida, evitando armazenamento de senha em texto puro.
        /// </summary>
        public async Task<Usuario> Criar(CriarUsuarioDTO dto)
        {
            var usuarioExistente = await _repo.ObterPorEmail(dto.Email);

            if (usuarioExistente != null)
                throw new AppException("Já existe um usuário cadastrado com este e-mail.", 400);

            var novo = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Tipo = dto.Tipo,
                CriadoEm = DateTime.UtcNow,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
            };

            return await _repo.Criar(novo);
        }

        // ============================================================
        // 4. LISTAR PAGINADO
        // ============================================================

        /// <summary>
        /// Lista usuários com paginação e busca opcional.
        ///
        /// O retorno contém os usuários da página solicitada e o total de registros,
        /// permitindo que o frontend monte componentes de paginação corretamente.
        /// </summary>
        public async Task<(IEnumerable<Usuario>, int)> ListarPaginado(
            int page,
            int pageSize,
            string? search)
        {
            return await _repo.ListarPaginado(page, pageSize, search);
        }

        // ============================================================
        // 5. ATUALIZAR
        // ============================================================

        /// <summary>
        /// Atualiza os dados básicos de um usuário existente.
        ///
        /// Antes de atualizar, o sistema valida se o usuário existe e está ativo.
        /// Isso impede alterações em usuários desativados logicamente.
        /// </summary>
        public async Task<Usuario?> Atualizar(int id, AtualizarUsuarioDTO dto)
        {
            var usuario = await _repo.ObterPorId(id);

            if (usuario == null || !usuario.Ativo)
                return null;

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.Tipo = dto.Tipo;

            return await _repo.Atualizar(usuario);
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente um usuário.
        ///
        /// Em vez de remover o registro do banco, o campo Ativo é alterado
        /// para false. Isso preserva o histórico e permite reativação futura.
        /// </summary>
        public async Task<bool> SoftDelete(int id)
        {
            return await _repo.SoftDelete(id);
        }

        // ============================================================
        // 7. REATIVAR
        // ============================================================

        /// <summary>
        /// Reativa um usuário previamente desativado.
        ///
        /// Esse fluxo permite que usuários removidos logicamente voltem a acessar
        /// o sistema sem necessidade de recriação do cadastro.
        /// </summary>
        public async Task<bool> Reativar(int id)
        {
            return await _repo.Reativar(id);
        }

        // ============================================================
        // 8. SOLICITAR RESET DE SENHA
        // ============================================================

        /// <summary>
        /// Inicia o fluxo de recuperação de senha.
        ///
        /// Caso o e-mail exista, o sistema gera um código temporário,
        /// salva esse código no banco e envia um e-mail com as instruções
        /// para redefinição de senha.
        ///
        /// Se o e-mail não existir, o método apenas encerra sem erro.
        /// Essa abordagem evita expor se determinado e-mail está ou não cadastrado.
        /// </summary>
        public async Task SolicitarResetSenha(string email)
        {
            var usuario = await _repo.ObterPorEmail(email);

            if (usuario == null)
                return;

            var codigo = new Random().Next(100000, 999999).ToString();

            var reset = new PasswordResetCode
            {
                Email = email,
                Codigo = codigo,
                ExpiraEm = DateTime.Now.AddMinutes(10),
                Usado = false
            };

            await _passwordResetRepository.Salvar(reset);

            var resetLinkComCodigo = $"http://localhost:5173/reset-password?email={email}&codigo={codigo}";
            var resetLinkManual = "http://localhost:5173/reset-password";

            var bodyHtml = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2>Redefinição de Senha</h2>

                    <p>Olá,</p>

                    <p>Recebemos uma solicitação para redefinir sua senha.</p>

                    <p>
                        <strong>Código de verificação:</strong><br/>
                        <span style='font-size: 20px; letter-spacing: 2px;'>
                            {codigo}
                        </span>
                    </p>

                    <p>
                        Você pode redefinir sua senha clicando no botão abaixo:
                    </p>

                    <p>
                        <a href='{resetLinkComCodigo}'
                           style='background-color:#4f46e5;
                                  color:white;
                                  padding:10px 16px;
                                  text-decoration:none;
                                  border-radius:6px;
                                  display:inline-block;'>
                            Redefinir Senha
                        </a>
                    </p>

                    <p style='margin-top:12px; font-size: 13px;'>
                        Caso o botão acima não funcione, acesse o link abaixo e informe o código manualmente:
                    </p>

                    <p>
                        <a href='{resetLinkManual}'
                           style='color:#4f46e5;'>
                            {resetLinkManual}
                        </a>
                    </p>

                    <p style='margin-top:20px; font-size: 12px; color: #666;'>
                        Este código expira em 10 minutos.<br/>
                        Se você não solicitou essa alteração, ignore este e-mail.
                    </p>
                </div>
                ";

            await _emailService.EnviarEmail(
                email,
                "Redefinição de Senha - EduConnect",
                bodyHtml,
                isHtml: true
            );
        }

        // ============================================================
        // 9. RESETAR SENHA
        // ============================================================

        /// <summary>
        /// Redefine a senha do usuário a partir de um código válido.
        ///
        /// O sistema valida se o código existe, se ainda não foi utilizado
        /// e se ainda está dentro do prazo de expiração.
        ///
        /// Após a alteração, a nova senha é salva como hash e o código de reset
        /// é marcado como utilizado, impedindo reutilização.
        /// </summary>
        public async Task<bool> ResetarSenha(string email, string codigo, string novaSenha)
        {
            var reset = await _passwordResetRepository.Obter(email, codigo);

            if (reset == null || reset.Usado || reset.ExpiraEm < DateTime.Now)
                return false;

            var usuario = await _repo.ObterPorEmail(email);

            if (usuario == null)
                return false;

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            await _repo.Atualizar(usuario);

            reset.Usado = true;
            await _passwordResetRepository.Atualizar(reset);

            var bodyHtml = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2>Senha Alterada com Sucesso</h2>

                    <p>Olá,</p>

                    <p>
                        Informamos que sua senha foi alterada com sucesso.
                    </p>

                    <p>
                        Se você realizou essa alteração, nenhuma ação adicional é necessária.
                    </p>

                    <p>
                        Caso você <strong>não reconheça essa alteração</strong>,
                        recomendamos redefinir sua senha imediatamente.
                    </p>

                    <p style='margin-top:20px; font-size: 12px; color: #666;'>
                        EduConnect - Sistema Acadêmico
                    </p>
                </div>
                ";

            await _emailService.EnviarEmail(
                email,
                "Senha alterada com sucesso - EduConnect",
                bodyHtml,
                isHtml: true
            );

            return true;
        }

        // ============================================================
        // 10. ATUALIZAR FOTO DE PERFIL
        // ============================================================

        /// <summary>
        /// Atualiza a foto de perfil do usuário.
        ///
        /// O método valida se o usuário existe e está ativo, verifica se o arquivo
        /// possui uma extensão permitida e se respeita o limite de tamanho.
        ///
        /// Após a validação, a imagem é salva na pasta wwwroot/uploads/perfis
        /// e a URL relativa é registrada no cadastro do usuário.
        /// </summary>
        public async Task<string?> AtualizarFotoPerfil(int id, IFormFile file)
        {
            var usuario = await _repo.ObterPorId(id);

            if (usuario == null || !usuario.Ativo)
                return null;

            var extensao = Path.GetExtension(file.FileName).ToLower();
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png" };

            if (!extensoesPermitidas.Contains(extensao))
                throw new AppException("Apenas JPG ou PNG são permitidos.", 400);

            if (file.Length > 2 * 1024 * 1024)
                throw new AppException("Arquivo deve ter no máximo 2MB.", 400);

            var nomeArquivo = $"perfil_{id}_{Guid.NewGuid()}{extensao}";

            var caminhoPasta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "perfis"
            );

            if (!Directory.Exists(caminhoPasta))
                Directory.CreateDirectory(caminhoPasta);

            var caminhoCompleto = Path.Combine(caminhoPasta, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/perfis/{nomeArquivo}";

            usuario.FotoPerfilUrl = url;

            await _repo.Atualizar(usuario);

            return url;
        }

        // ============================================================
        // 11. OBTER FOTO DE PERFIL
        // ============================================================

        /// <summary>
        /// Obtém a foto de perfil de um usuário como arquivo.
        ///
        /// O método busca a URL salva no cadastro do usuário, monta o caminho físico
        /// dentro da pasta wwwroot e retorna os bytes da imagem junto com o content type.
        ///
        /// Isso permite que o Controller retorne a imagem usando File(),
        /// facilitando o consumo pelo frontend.
        /// </summary>
        public async Task<(byte[] bytes, string contentType)?> ObterFotoPerfil(int id)
        {
            var usuario = await _repo.ObterPorId(id);

            if (usuario == null || string.IsNullOrEmpty(usuario.FotoPerfilUrl))
                return null;

            var caminhoCompleto = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                usuario.FotoPerfilUrl.TrimStart('/')
            );

            if (!System.IO.File.Exists(caminhoCompleto))
                return null;

            var bytes = await System.IO.File.ReadAllBytesAsync(caminhoCompleto);

            var extensao = Path.GetExtension(caminhoCompleto).ToLower();

            var contentType = extensao switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            return (bytes, contentType);
        }
    }
}