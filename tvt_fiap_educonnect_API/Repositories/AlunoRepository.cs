using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Aluno.
    ///
    /// No contexto do EduConnect, essa camada centraliza as consultas e alterações
    /// relacionadas ao perfil acadêmico do aluno, utilizando Entity Framework.
    ///
    /// Os métodos incluem a entidade Usuario com Include porque muitas respostas
    /// do aluno precisam exibir dados vindos do usuário vinculado, como nome e e-mail.
    /// </summary>
    public class AlunoRepository : IAlunoRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        /// </summary>
        public AlunoRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo aluno no banco de dados.
        ///
        /// A validação de regra de negócio, como verificar se o usuário é do tipo aluno,
        /// é feita na camada de Service. O Repository apenas persiste a entidade.
        /// </summary>
        public async Task<Aluno> Criar(Aluno aluno)
        {
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            return aluno;
        }

        /// <summary>
        /// Busca um aluno pelo ID do usuário vinculado.
        ///
        /// O Include em Usuario permite retornar também dados como nome e e-mail,
        /// necessários para montar o AlunoDTO.
        /// </summary>
        public async Task<Aluno?> ObterPorUsuarioId(int usuarioId)
        {
            return await _context.Alunos
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId);
        }

        /// <summary>
        /// Busca um aluno pelo ID da entidade Aluno.
        ///
        /// Também carrega o Usuario relacionado para permitir o mapeamento
        /// completo para DTO.
        /// </summary>
        public async Task<Aluno?> ObterPorId(int id)
        {
            return await _context.Alunos
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// Lista todos os alunos cadastrados.
        ///
        /// O Include em Usuario evita consultas adicionais ao acessar nome e e-mail
        /// durante o mapeamento para DTO.
        /// </summary>
        public async Task<IEnumerable<Aluno>> Listar()
        {
            return await _context.Alunos
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        /// <summary>
        /// Atualiza os dados de um aluno existente no banco.
        ///
        /// A entidade já chega alterada pela camada de Service.
        /// </summary>
        public async Task<Aluno> Atualizar(Aluno aluno)
        {
            _context.Alunos.Update(aluno);
            await _context.SaveChangesAsync();

            return aluno;
        }
    }
}