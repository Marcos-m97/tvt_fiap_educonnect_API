using EduConnect_API.Exceptions;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por montar o contexto acadêmico do usuário autenticado.
    ///
    /// No EduConnect, além dos dados básicos do usuário, o frontend precisa saber
    /// qual é o contexto de acesso daquele perfil.
    ///
    /// Exemplos:
    /// - Aluno: matrícula ativa, turma, curso e disciplinas disponíveis.
    /// - Professor: turmas e disciplinas em que atua.
    ///
    /// Esse serviço centraliza essa montagem para evitar que o frontend precise
    /// chamar vários endpoints separadamente para compor a tela inicial.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAlunoRepository _alunoRepo;
        private readonly IProfessorRepository _professorRepo;
        private readonly IMatriculaRepository _matriculaRepo;
        private readonly ITurmaDisciplinaRepository _tdRepo;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IAlunoRepository: localiza o perfil acadêmico do aluno.
        /// IProfessorRepository: localiza o perfil docente do professor.
        /// IMatriculaRepository: identifica a matrícula ativa do aluno.
        /// ITurmaDisciplinaRepository: lista disciplinas da turma ou vínculos do professor.
        /// </summary>
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
        // 1. CONTEXTO DO USUÁRIO LOGADO
        // =========================================================

        /// <summary>
        /// Obtém o contexto do usuário autenticado com base no tipo de perfil.
        ///
        /// O tipo vem da role do token JWT:
        /// 2 = Professor
        /// 3 = Aluno
        ///
        /// Para alunos, o serviço monta um contexto com matrícula ativa,
        /// turma, curso e disciplinas.
        ///
        /// Para professores, o serviço monta um contexto com as turmas
        /// e disciplinas vinculadas ao professor.
        /// </summary>
        public async Task<MeContextoDTO> ObterContexto(int usuarioId, string tipoUsuario)
        {
            if (tipoUsuario == "3")
            {
                var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                    ?? throw new AppException("Aluno não encontrado.", 404);

                return await MontarContextoAluno(aluno.Id);
            }

            if (tipoUsuario == "2")
            {
                var professor = await _professorRepo.ObterPorUsuarioId(usuarioId)
                    ?? throw new AppException("Professor não encontrado.", 404);

                return await MontarContextoProfessor(professor.Id);
            }

            throw new AppException("Tipo de usuário não suportado para contexto.", 400);
        }

        // =========================================================
        // 2. CONTEXTO ADMINISTRATIVO DE ALUNO
        // =========================================================

        /// <summary>
        /// Obtém o contexto acadêmico de um aluno pelo ID do aluno.
        ///
        /// Esse método é usado em fluxos administrativos ou de professor,
        /// quando é necessário consultar o contexto de um aluno específico.
        /// </summary>
        public async Task<MeContextoDTO> ObterContextoAluno(int alunoId)
        {
            return await MontarContextoAluno(alunoId);
        }

        // =========================================================
        // 3. CONTEXTO ADMINISTRATIVO DE PROFESSOR
        // =========================================================

        /// <summary>
        /// Obtém o contexto acadêmico de um professor pelo ID do professor.
        ///
        /// Esse método é usado em fluxos administrativos para consultar
        /// as turmas e disciplinas vinculadas a determinado professor.
        /// </summary>
        public async Task<MeContextoDTO> ObterContextoProfessor(int professorId)
        {
            return await MontarContextoProfessor(professorId);
        }

        // =========================================================
        // 4. MONTAR CONTEXTO DO ALUNO
        // =========================================================

        /// <summary>
        /// Monta o contexto acadêmico de um aluno.
        ///
        /// O método busca o aluno, localiza sua matrícula ativa e, a partir
        /// da turma da matrícula, lista as disciplinas disponíveis.
        ///
        /// O retorno inclui dados como:
        /// - ID do aluno;
        /// - turma ativa;
        /// - curso;
        /// - disciplinas vinculadas à turma.
        /// </summary>
        private async Task<MeContextoDTO> MontarContextoAluno(int alunoId)
        {
            var aluno = await _alunoRepo.ObterPorId(alunoId)
                ?? throw new AppException("Aluno não encontrado.", 404);

            var matricula = await _matriculaRepo.ObterAtivaPorAlunoId(aluno.Id)
                ?? throw new AppException("Aluno não possui matrícula ativa.", 404);

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

        // =========================================================
        // 5. MONTAR CONTEXTO DO PROFESSOR
        // =========================================================

        /// <summary>
        /// Monta o contexto acadêmico de um professor.
        ///
        /// O método busca o professor e lista seus vínculos em TurmaDisciplina.
        /// Esses vínculos indicam quais disciplinas o professor ministra
        /// e em quais turmas.
        ///
        /// O retorno é usado pelo frontend para montar a área do professor.
        /// </summary>
        private async Task<MeContextoDTO> MontarContextoProfessor(int professorId)
        {
            var professor = await _professorRepo.ObterPorId(professorId)
                ?? throw new AppException("Professor não encontrado.", 404);

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