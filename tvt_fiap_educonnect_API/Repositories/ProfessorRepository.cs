using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Professor.
    ///
    /// No EduConnect, essa camada centraliza as consultas e alterações relacionadas
    /// ao perfil docente, utilizando o AppDbContext e o Entity Framework.
    ///
    /// Os métodos utilizam Include em Usuario porque muitas respostas do professor
    /// precisam exibir dados vindos do usuário vinculado, como nome e e-mail.
    /// </summary>
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        /// </summary>
        public ProfessorRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo professor no banco de dados.
        ///
        /// A validação de regra de negócio, como verificar se o usuário possui
        /// Tipo = 2, é feita na camada de Service. O Repository apenas persiste
        /// a entidade.
        /// </summary>
        public async Task<Professor> Criar(Professor professor)
        {
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();

            return professor;
        }

        /// <summary>
        /// Busca um professor a partir do ID do usuário vinculado.
        ///
        /// Esse método é útil quando o frontend possui o usuário autenticado
        /// e precisa localizar o registro correspondente na tabela de professores.
        /// </summary>
        public async Task<Professor?> ObterPorUsuarioId(int usuarioId)
        {
            return await _context.Professores
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }

        /// <summary>
        /// Busca um professor pelo ID da entidade Professor.
        ///
        /// O Include em Usuario permite montar corretamente o ProfessorDTO
        /// com dados como nome e e-mail.
        /// </summary>
        public async Task<Professor?> ObterPorId(int id)
        {
            return await _context.Professores
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Lista todos os professores cadastrados.
        ///
        /// O Include em Usuario evita consultas adicionais ao acessar dados
        /// do usuário durante o mapeamento para DTO.
        /// </summary>
        public async Task<IEnumerable<Professor>> Listar()
        {
            return await _context.Professores
                .Include(p => p.Usuario)
                .ToListAsync();
        }

        /// <summary>
        /// Atualiza os dados de um professor existente.
        ///
        /// A entidade já chega alterada pela camada de Service.
        /// </summary>
        public async Task<Professor> Atualizar(Professor professor)
        {
            _context.Professores.Update(professor);
            await _context.SaveChangesAsync();

            return professor;
        }
    }
}