using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Admin.
    ///
    /// No EduConnect, o Admin representa uma extensão da entidade Usuario,
    /// armazenando informações específicas do perfil administrativo,
    /// como departamento e cargo.
    ///
    /// Essa camada centraliza criação, consulta, listagem e atualização
    /// dos perfis administrativos, utilizando Entity Framework.
    /// </summary>
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext expõe a tabela de administradores e o relacionamento
        /// com a entidade Usuario.
        /// </summary>
        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR ADMIN
        // ============================================================

        /// <summary>
        /// Cria um novo perfil administrativo no banco de dados.
        ///
        /// A entidade já chega montada pela camada de Service, que deve validar
        /// se o usuário vinculado existe e possui perfil administrativo.
        /// </summary>
        public async Task<Admin> Criar(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return admin;
        }

        // ============================================================
        // 2. OBTER ADMIN POR USUÁRIO
        // ============================================================

        /// <summary>
        /// Obtém o perfil administrativo a partir do ID do usuário vinculado.
        ///
        /// Esse método é útil quando o sistema possui o usuário autenticado
        /// e precisa localizar o cadastro administrativo correspondente.
        ///
        /// O Include em Usuario permite retornar dados como nome, e-mail e tipo.
        /// </summary>
        public async Task<Admin?> ObterPorUsuarioId(int usuarioId)
        {
            return await _context.Admins
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId);
        }

        // ============================================================
        // 3. OBTER ADMIN POR ID
        // ============================================================

        /// <summary>
        /// Obtém um perfil administrativo pelo ID da entidade Admin.
        ///
        /// Também carrega o Usuario relacionado para permitir montar DTOs
        /// com informações completas para o frontend.
        /// </summary>
        public async Task<Admin?> ObterPorId(int id)
        {
            return await _context.Admins
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // ============================================================
        // 4. LISTAR ADMINS
        // ============================================================

        /// <summary>
        /// Lista todos os administradores cadastrados.
        ///
        /// A consulta carrega também a entidade Usuario, pois dados como nome
        /// e e-mail pertencem ao usuário vinculado ao perfil administrativo.
        /// </summary>
        public async Task<IEnumerable<Admin>> Listar()
        {
            return await _context.Admins
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        // ============================================================
        // 5. ATUALIZAR ADMIN
        // ============================================================

        /// <summary>
        /// Atualiza os dados de um perfil administrativo existente.
        ///
        /// A entidade já chega modificada pela camada de Service.
        /// </summary>
        public async Task<Admin> Atualizar(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();

            return admin;
        }
    }
}