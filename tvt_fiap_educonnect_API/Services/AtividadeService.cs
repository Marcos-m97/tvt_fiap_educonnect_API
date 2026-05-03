using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas às atividades.
    ///
    /// No EduConnect, a atividade representa uma tarefa, prova, trabalho ou exercício
    /// criado dentro de uma TurmaDisciplina.
    ///
    /// Essa camada coordena a criação, consulta, atualização e listagem das atividades,
    /// além de montar a visão específica do aluno com informações de entrega e nota.
    /// </summary>
    public class AtividadeService : IAtividadeService
    {
        private readonly IAtividadeRepository _atividadeRepo;
        private readonly ITurmaDisciplinaRepository _tdRepo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly IMatriculaRepository _matriculaRepo;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IAtividadeRepository: acesso aos dados de atividades.
        /// ITurmaDisciplinaRepository: valida o contexto acadêmico da atividade.
        /// IAlunoRepository: localiza o aluno a partir do usuário autenticado.
        /// IMatriculaRepository: verifica a matrícula ativa do aluno.
        /// </summary>
        public AtividadeService(
            IAtividadeRepository atividadeRepo,
            ITurmaDisciplinaRepository tdRepo,
            IAlunoRepository alunoRepo,
            IMatriculaRepository matriculaRepo)
        {
            _atividadeRepo = atividadeRepo;
            _tdRepo = tdRepo;
            _alunoRepo = alunoRepo;
            _matriculaRepo = matriculaRepo;
        }

        // ============================================================
        // 1. CRIAR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Cria uma nova atividade vinculada a uma TurmaDisciplina.
        ///
        /// Antes de criar, o sistema valida se a TurmaDisciplina informada existe.
        /// Isso garante que a atividade seja criada dentro de um contexto acadêmico
        /// válido, ou seja, uma disciplina ofertada em uma turma.
        /// </summary>
        public async Task<AtividadeDTO> Criar(CriarAtividadeDTO dto)
        {
            var td = await _tdRepo.ObterPorId(dto.TurmaDisciplinaId)
                ?? throw new AppException("TurmaDisciplina não encontrada.", 404);

            var atividade = new Atividade
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                UrlMaterial = dto.UrlMaterial,
                DataEntrega = dto.DataEntrega,
                Tipo = dto.Tipo,
                TurmaDisciplinaId = dto.TurmaDisciplinaId
            };

            atividade = await _atividadeRepo.Criar(atividade);

            return new AtividadeDTO
            {
                Id = atividade.Id,
                Titulo = atividade.Titulo,
                Descricao = atividade.Descricao,
                UrlMaterial = atividade.UrlMaterial,
                DataEntrega = atividade.DataEntrega,
                Tipo = atividade.Tipo,
                TurmaDisciplinaId = atividade.TurmaDisciplinaId,
                TurmaNome = td.Turma.Nome,
                DisciplinaNome = td.Disciplina.Nome,
                ProfessorNome = td.Professor.Usuario.Nome
            };
        }

        // ============================================================
        // 2. LISTAR ATIVIDADES POR TURMA/DISCIPLINA
        // ============================================================

        /// <summary>
        /// Lista atividades vinculadas a uma TurmaDisciplina específica.
        ///
        /// Esse método é usado para exibir as atividades de uma disciplina
        /// dentro de uma turma, normalmente na visão do professor ou do aluno.
        /// </summary>
        public async Task<IEnumerable<AtividadeDTO>> ListarPorTurmaDisciplina(int turmaDisciplinaId)
        {
            var lista = await _atividadeRepo.ListarPorTurmaDisciplina(turmaDisciplinaId);

            return lista.Select(a => new AtividadeDTO
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Descricao = a.Descricao,
                UrlMaterial = a.UrlMaterial,
                DataEntrega = a.DataEntrega,
                Tipo = a.Tipo,
                TurmaDisciplinaId = a.TurmaDisciplinaId,
                TurmaNome = a.TurmaDisciplina.Turma.Nome,
                DisciplinaNome = a.TurmaDisciplina.Disciplina.Nome,
                ProfessorNome = a.TurmaDisciplina.Professor.Usuario.Nome
            });
        }

        // ============================================================
        // 3. LISTAR MINHAS ATIVIDADES
        // ============================================================

        /// <summary>
        /// Lista as atividades disponíveis para o aluno logado.
        ///
        /// O método localiza o aluno a partir do usuário autenticado,
        /// verifica se ele possui matrícula ativa e então busca as atividades
        /// da turma correspondente.
        ///
        /// Além dos dados da atividade, o retorno informa se o aluno já entregou
        /// e qual nota foi atribuída, quando existir uma entrega vinculada.
        /// </summary>
        public async Task<IEnumerable<AtividadeAlunoDTO>> ListarMinhasAtividades(int usuarioId)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new AppException("Aluno não encontrado.", 404);

            var matricula = await _matriculaRepo.ObterAtivaPorAlunoId(aluno.Id)
                ?? throw new AppException("Aluno não possui matrícula ativa.", 404);

            var atividades = await _atividadeRepo.ListarPorTurma(matricula.TurmaId);

            return atividades.Select(a =>
            {
                var entrega = a.Entregas
                    .FirstOrDefault(e => e.AlunoId == aluno.Id);

                return new AtividadeAlunoDTO
                {
                    AtividadeId = a.Id,
                    Titulo = a.Titulo,

                    DisciplinaId = a.TurmaDisciplina.DisciplinaId,
                    NomeDisciplina = a.TurmaDisciplina.Disciplina.Nome,

                    DataEntrega = a.DataEntrega,

                    JaEntregue = entrega != null,
                    Nota = entrega?.Nota
                };
            });
        }

        // ============================================================
        // 4. OBTER ATIVIDADE POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma atividade pelo ID.
        ///
        /// A consulta retorna também dados descritivos do contexto acadêmico,
        /// como turma, disciplina e professor.
        /// </summary>
        public async Task<AtividadeDTO> ObterPorId(int id)
        {
            var atividade = await _atividadeRepo.ObterPorId(id)
                ?? throw new AppException("Atividade não encontrada.", 404);

            return new AtividadeDTO
            {
                Id = atividade.Id,
                Titulo = atividade.Titulo,
                Descricao = atividade.Descricao,
                UrlMaterial = atividade.UrlMaterial,
                DataEntrega = atividade.DataEntrega,
                Tipo = atividade.Tipo,
                TurmaDisciplinaId = atividade.TurmaDisciplinaId,
                TurmaNome = atividade.TurmaDisciplina.Turma.Nome,
                DisciplinaNome = atividade.TurmaDisciplina.Disciplina.Nome,
                ProfessorNome = atividade.TurmaDisciplina.Professor.Usuario.Nome
            };
        }

        // ============================================================
        // 5. ATUALIZAR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma atividade existente.
        ///
        /// O método busca a atividade, altera os campos editáveis e delega
        /// a persistência ao Repository.
        /// </summary>
        public async Task<AtividadeDTO> Atualizar(int id, AtualizarAtividadeDTO dto)
        {
            var atividade = await _atividadeRepo.ObterPorId(id)
                ?? throw new AppException("Atividade não encontrada.", 404);

            atividade.Titulo = dto.Titulo;
            atividade.Descricao = dto.Descricao;
            atividade.UrlMaterial = dto.UrlMaterial;
            atividade.DataEntrega = dto.DataEntrega;
            atividade.Tipo = dto.Tipo;

            atividade = await _atividadeRepo.Atualizar(atividade);

            return new AtividadeDTO
            {
                Id = atividade.Id,
                Titulo = atividade.Titulo,
                Descricao = atividade.Descricao,
                UrlMaterial = atividade.UrlMaterial,
                DataEntrega = atividade.DataEntrega,
                Tipo = atividade.Tipo,
                TurmaDisciplinaId = atividade.TurmaDisciplinaId,
                TurmaNome = atividade.TurmaDisciplina.Turma.Nome,
                DisciplinaNome = atividade.TurmaDisciplina.Disciplina.Nome,
                ProfessorNome = atividade.TurmaDisciplina.Professor.Usuario.Nome
            };
        }
    }
}