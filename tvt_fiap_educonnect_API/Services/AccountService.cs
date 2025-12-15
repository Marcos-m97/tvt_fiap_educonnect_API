using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAlunoRepository _alunoRepo;
        private readonly IProfessorRepository _professorRepo;
        private readonly IMatriculaRepository _matriculaRepo;
        private readonly ITurmaDisciplinaRepository _tdRepo;

        public AccountService(
            IAlunoRepository alunoRepo,
            IProfessorRepository professorRepo,
            IMatriculaRepository matriculaRepo,
            ITurmaDisciplinaRepository tdRepo)
        {
            _alunoRepo = alunoRepo;
            _professorRepo = professorRepo;
            _matriculaRepo = matriculaRepo;
            _tdRepo = tdRepo;
        }

        // =========================================================
        // CONTEXTO DO USUÁRIO LOGADO
        // =========================================================
        public async Task<MeContextoDTO> ObterContexto(Guid usuarioId, string tipoUsuario)
        {
            if (tipoUsuario == "3") // aluno
            {
                var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                    ?? throw new Exception("Aluno não encontrado.");

                return await MontarContextoAluno(aluno.Id);
            }

            if (tipoUsuario == "2") // professor
            {
                var professor = await _professorRepo.ObterPorUsuarioId(usuarioId)
                    ?? throw new Exception("Professor não encontrado.");

                return await MontarContextoProfessor(professor.Id);
            }

            throw new Exception("Tipo de usuário não suportado para contexto.");
        }

        // =========================================================
        // CONTEXTO ADMINISTRATIVO
        // =========================================================
        public async Task<MeContextoDTO> ObterContextoAluno(Guid alunoId)
        {
            return await MontarContextoAluno(alunoId);
        }

        public async Task<MeContextoDTO> ObterContextoProfessor(Guid professorId)
        {
            return await MontarContextoProfessor(professorId);
        }

        // =========================================================
        // MÉTODOS PRIVADOS (REUTILIZÁVEIS)
        // =========================================================
        private async Task<MeContextoDTO> MontarContextoAluno(Guid alunoId)
        {
            var aluno = await _alunoRepo.ObterPorId(alunoId)
                ?? throw new Exception("Aluno não encontrado.");

            var matricula = await _matriculaRepo.ObterAtivaPorAlunoId(aluno.Id)
                ?? throw new Exception("Aluno não possui matrícula ativa.");

            var disciplinas = await _tdRepo.ListarPorTurma(matricula.TurmaId);

            return new MeContextoDTO
            {
                TipoUsuario = "Aluno",
                AlunoId = aluno.Id,
                TurmaId = matricula.TurmaId,
                TurmaNome = matricula.Turma.Nome,
                CursoNome = matricula.Turma.Curso.Nome,
                Disciplinas = disciplinas.Select(d => new DisciplinaResumoDTO
                {
                    DisciplinaId = d.DisciplinaId,
                    Nome = d.Disciplina.Nome
                }).ToList()
            };
        }

        private async Task<MeContextoDTO> MontarContextoProfessor(Guid professorId)
        {
            var professor = await _professorRepo.ObterPorId(professorId)
                ?? throw new Exception("Professor não encontrado.");

            var vinculos = await _tdRepo.ListarPorProfessor(professor.Id);

            return new MeContextoDTO
            {
                TipoUsuario = "Professor",
                ProfessorId = professor.Id,
                TurmasDisciplinas = vinculos.Select(v => new TurmaDisciplinaResumoDTO
                {
                    TurmaDisciplinaId = v.Id,

                    TurmaId = v.TurmaId,
                    TurmaNome = v.Turma.Nome,

                    DisciplinaId = v.DisciplinaId,
                    DisciplinaNome = v.Disciplina.Nome
                }).ToList()
            };
        }
    }
}
