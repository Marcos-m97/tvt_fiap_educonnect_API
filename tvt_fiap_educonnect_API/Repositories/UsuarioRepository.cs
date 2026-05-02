using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Usuario.
    ///
    /// No contexto do EduConnect, essa camada centraliza as consultas e alterações
    /// relacionadas aos usuários, utilizando o AppDbContext e o Entity Framework.
    ///
    /// Essa separação evita que Controllers e Services tenham consultas SQL/EF
    /// espalhadas pelo código, facilitando manutenção e evolução do projeto.
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext representa a conexão com o banco e expõe as tabelas
        /// utilizadas pela aplicação.
        /// </summary>
        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém um usuário a partir do e-mail informado.
        ///
        /// Esse método é utilizado principalmente no login, onde o sistema precisa
        /// localizar o usuário antes de validar a senha.
        /// </summary>
        public async Task<Usuario?> ObterPorEmail(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Obtém um usuário pelo ID.
        ///
        /// Utilizado em fluxos como consulta de perfil, edição de usuário,
        /// atualização de foto e validações internas do sistema.
        /// </summary>
        public async Task<Usuario?> ObterPorId(int id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Persiste um novo usuário no banco de dados.
        ///
        /// A entidade já deve chegar com os dados de negócio preparados pela camada
        /// de Service, como senha criptografada e tipo de perfil.
        /// </summary>
        public async Task<Usuario> Criar(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        /// <summary>
        /// Lista os usuários com paginação e busca opcional.
        ///
        /// A busca considera nome, e-mail ou ID do usuário. A ordenação prioriza
        /// usuários ativos e depois organiza por nome, facilitando a visualização
        /// no painel administrativo.
        ///
        /// O retorno inclui:
        /// - usuários da página atual;
        /// - total de registros encontrados.
        /// </summary>
        public async Task<(IEnumerable<Usuario>, int)> ListarPaginado(
            int page,
            int pageSize,
            string? search)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Nome.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.Id.ToString().Contains(search));
            }

            query = query
                .OrderByDescending(u => u.Ativo)
                .ThenBy(u => u.Nome);

            var total = await query.CountAsync();

            var usuarios = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (usuarios, total);
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        ///
        /// O método recebe a entidade já alterada pela camada de Service
        /// e apenas realiza a persistência no banco.
        /// </summary>
        public async Task<Usuario> Atualizar(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        /// <summary>
        /// Realiza a exclusão lógica de um usuário.
        ///
        /// Em vez de remover fisicamente o registro do banco, o sistema altera
        /// o campo Ativo para false. Essa abordagem preserva o histórico e permite
        /// reativação futura.
        /// </summary>
        public async Task<bool> SoftDelete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return false;

            usuario.Ativo = false;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Reativa um usuário previamente desativado.
        ///
        /// Esse método altera o campo Ativo para true, permitindo que o usuário
        /// volte a ser utilizado no sistema.
        /// </summary>
        public async Task<bool> Reativar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return false;

            usuario.Ativo = true;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}