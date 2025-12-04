using EduConnect_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ============================
        // Usuários
        // ============================
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Professor> Professores { get; set; }
        public DbSet<Admin> Admins { get; set; }

        // ============================
        // Acadêmico
        // ============================
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Disciplina> Disciplinas { get; set; }
        public DbSet<Turma> Turmas { get; set; }
        public DbSet<TurmaDisciplina> TurmaDisciplinas { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Atividade> Atividades { get; set; }
        public DbSet<EntregaAtividade> EntregasAtividades { get; set; }
        public DbSet<Evento> Eventos { get; set; }

        // ============================
        // Boletins (NOVO)
        // ============================
        public DbSet<Boletim> Boletins { get; set; }
        public DbSet<BoletimDisciplina> BoletinsDisciplinas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===============================================
            // RELACIONAMENTOS DO BOLETIM
            // (Necessário antes de remover cascades)
            // ===============================================
            modelBuilder.Entity<Boletim>()
                .HasMany(b => b.Disciplinas)
                .WithOne(d => d.Boletim)
                .HasForeignKey(d => d.BoletimId)
                .OnDelete(DeleteBehavior.Cascade);
            // ? CASCADE localmente aqui É SEGURO
            // pois o seu código global abaixo sobrescreve tudo para Restrict

            // ===============================================
            // IMPORTANTE:
            // Remover CASCADE DELETE GLOBALMENTE
            // ===============================================
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
