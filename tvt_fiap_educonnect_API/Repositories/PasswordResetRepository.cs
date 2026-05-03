using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados relacionadas
    /// aos códigos de recuperação de senha.
    ///
    /// No EduConnect, esse repositório apoia o fluxo de "esqueci minha senha",
    /// armazenando códigos temporários, consultando códigos informados pelo usuário
    /// e atualizando o status de uso após a redefinição da senha.
    /// </summary>
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext expõe a tabela PasswordResetCodes, usada para armazenar
        /// os códigos temporários de recuperação de senha.
        /// </summary>
        public PasswordResetRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. SALVAR CÓDIGO DE RECUPERAÇÃO
        // ============================================================

        /// <summary>
        /// Salva um novo código de recuperação de senha no banco de dados.
        ///
        /// Esse método é chamado quando o usuário solicita redefinição de senha.
        /// O código salvo possui e-mail, código temporário, data de expiração
        /// e status de uso.
        /// </summary>
        public async Task Salvar(PasswordResetCode code)
        {
            _context.PasswordResetCodes.Add(code);
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // 2. OBTER CÓDIGO POR E-MAIL E CÓDIGO
        // ============================================================

        /// <summary>
        /// Busca um código de recuperação a partir do e-mail e do código informados.
        ///
        /// A ordenação por ExpiraEm decrescente prioriza o código mais recente
        /// caso existam múltiplas solicitações para o mesmo e-mail.
        ///
        /// A validação de expiração e uso do código é feita na camada de Service.
        /// </summary>
        public async Task<PasswordResetCode?> Obter(string email, string codigo)
        {
            return await _context.PasswordResetCodes
                .Where(x => x.Email == email && x.Codigo == codigo)
                .OrderByDescending(x => x.ExpiraEm)
                .FirstOrDefaultAsync();
        }

        // ============================================================
        // 3. ATUALIZAR CÓDIGO
        // ============================================================

        /// <summary>
        /// Atualiza um código de recuperação existente.
        ///
        /// Esse método é usado principalmente após a redefinição de senha,
        /// quando o código é marcado como usado para impedir reutilização.
        /// </summary>
        public async Task Atualizar(PasswordResetCode code)
        {
            _context.PasswordResetCodes.Update(code);
            await _context.SaveChangesAsync();
        }
    }
}