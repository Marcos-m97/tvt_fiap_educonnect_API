using EduConnect_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Data
{
    /// <summary>
    /// Contexto principal do Entity Framework Core para o EduConnect.
    ///
    /// O AppDbContext representa a ponte entre a aplicação e o banco de dados.
    /// Ele expõe as tabelas do sistema por meio de DbSet e concentra configurações
    /// específicas do modelo, como relacionamentos, comportamento de exclusão
    /// e precisão de campos numéricos.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Recebe as opções de configuração do DbContext por injeção de dependência.
        ///
        /// Essas opções normalmente incluem a string de conexão e o provedor
        /// de banco de dados configurado no Program.cs.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ============================================================
        // 1. USUÁRIOS E PERFIS
        // ============================================================

        /// <summary>
        /// Tabela de usuários base do sistema.
        ///
        /// Armazena dados comuns de autenticação e autorização, como nome,
        /// e-mail, senha hash, tipo de perfil, status ativo e foto de perfil.
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// Tabela de códigos temporários para recuperação de senha.
        /// </summary>
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

        /// <summary>
        /// Tabela de perfis de aluno.
        ///
        /// Funciona como extensão da entidade Usuario para usuários do tipo Aluno.
        /// </summary>
        public DbSet<Aluno> Alunos { get; set; }

        /// <summary>
        /// Tabela de perfis de professor.
        ///
        /// Funciona como extensão da entidade Usuario para usuários do tipo Professor.
        /// </summary>
        public DbSet<Professor> Professores { get; set; }

        /// <summary>
        /// Tabela de perfis administrativos.
        ///
        /// Funciona como extensão da entidade Usuario para usuários administrativos.
        /// </summary>
        public DbSet<Admin> Admins { get; set; }

        // ============================================================
        // 2. ESTRUTURA ACADÊMICA
        // ============================================================

        /// <summary>
        /// Tabela de cursos disponíveis na plataforma.
        /// </summary>
        public DbSet<Curso> Cursos { get; set; }

        /// <summary>
        /// Tabela de disciplinas vinculadas aos cursos.
        /// </summary>
        public DbSet<Disciplina> Disciplinas { get; set; }

        /// <summary>
        /// Tabela de turmas vinculadas aos cursos.
        /// </summary>
        public DbSet<Turma> Turmas { get; set; }

        /// <summary>
        /// Tabela de associação entre turma, disciplina e professor.
        ///
        /// Define quais disciplinas serão ofertadas em cada turma
        /// e qual professor será responsável por cada uma.
        /// </summary>
        public DbSet<TurmaDisciplina> TurmaDisciplinas { get; set; }

        /// <summary>
        /// Tabela de matrículas dos alunos em turmas.
        /// </summary>
        public DbSet<Matricula> Matriculas { get; set; }

        /// <summary>
        /// Tabela de atividades acadêmicas criadas para uma turma/disciplina.
        /// </summary>
        public DbSet<Atividade> Atividades { get; set; }

        /// <summary>
        /// Tabela de entregas de atividades realizadas pelos alunos.
        /// </summary>
        public DbSet<EntregaAtividade> EntregasAtividades { get; set; }

        /// <summary>
        /// Tabela de eventos acadêmicos ou administrativos do calendário.
        /// </summary>
        public DbSet<Evento> Eventos { get; set; }

        /// <summary>
        /// Tabela de aulas cadastradas em uma turma/disciplina.
        /// </summary>
        public DbSet<Aula> Aulas { get; set; }

        // ============================================================
        // 3. BOLETINS
        // ============================================================

        /// <summary>
        /// Tabela de boletins gerais dos alunos.
        ///
        /// Representa o boletim consolidado de um aluno em uma turma.
        /// </summary>
        public DbSet<Boletim> Boletins { get; set; }

        /// <summary>
        /// Tabela de resultados por disciplina dentro de um boletim.
        /// </summary>
        public DbSet<BoletimDisciplina> BoletinsDisciplinas { get; set; }

        /// <summary>
        /// Tabela de atividades detalhadas dentro de uma disciplina do boletim.
        /// </summary>
        public DbSet<BoletimAtividade> BoletimAtividades { get; set; }

        // ============================================================
        // 4. CONFIGURAÇÕES DO MODELO
        // ============================================================

        /// <summary>
        /// Configura regras adicionais do modelo do Entity Framework.
        ///
        /// Aqui são definidos comportamentos que não ficam explícitos apenas
        /// nas entidades, como precisão de campos decimais e comportamento
        /// de exclusão dos relacionamentos.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===============================================
            // PRECISÃO DECIMAL - NOTA DA ENTREGA
            // ===============================================
            // Define explicitamente a precisão da coluna Nota.
            //
            // Sem essa configuração, o Entity Framework gera warning informando
            // que valores decimais podem ser truncados silenciosamente no SQL Server.
            //
            // HasPrecision(5, 2) permite valores com até 5 dígitos no total,
            // sendo 2 casas decimais. Exemplos: 10.00, 8.50, 100.00.
            modelBuilder.Entity<EntregaAtividade>()
                .Property(e => e.Nota)
                .HasPrecision(5, 2);

            // ===============================================
            // RELACIONAMENTOS DO BOLETIM
            // ===============================================
            // Define o relacionamento entre Boletim e BoletimDisciplina.
            //
            // Um boletim possui várias disciplinas.
            // Cada disciplina pertence a um único boletim.
            modelBuilder.Entity<Boletim>()
                .HasMany(b => b.Disciplinas)
                .WithOne(d => d.Boletim)
                .HasForeignKey(d => d.BoletimId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===============================================
            // IMPORTANTE:
            // REMOVER CASCADE DELETE GLOBALMENTE
            // ===============================================
            // Altera o comportamento de exclusão dos relacionamentos para Restrict.
            //
            // Essa estratégia evita exclusões em cascata acidentais em entidades
            // acadêmicas relacionadas, como usuário, aluno, turma, matrícula,
            // disciplinas, atividades e entregas.
            //
            // Em vez de apagar automaticamente registros filhos, o sistema exige
            // tratamento explícito pela regra de negócio.
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}