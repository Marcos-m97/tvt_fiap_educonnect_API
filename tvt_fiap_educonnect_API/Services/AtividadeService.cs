using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class AtividadeService : IAtividadeService
    {
        private readonly IAtividadeRepository _atividadeRepo;
        private readonly ITurmaDisciplinaRepository _tdRepo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly IMatriculaRepository _matriculaRepo;

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


        public async Task<AtividadeDTO> Criar(CriarAtividadeDTO dto)
        {
            var td = await _tdRepo.ObterPorId(dto.TurmaDisciplinaId)
                ?? throw new Exception("TurmaDisciplina não encontrada.");

            var atividade = new Atividade
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
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
                DataEntrega = atividade.DataEntrega,
                Tipo = atividade.Tipo,
                TurmaDisciplinaId = atividade.TurmaDisciplinaId,
                TurmaNome = td.Turma.Nome,
                DisciplinaNome = td.Disciplina.Nome,
                ProfessorNome = td.Professor.Usuario.Nome
            };
        }

        public async Task<IEnumerable<AtividadeDTO>> ListarPorTurmaDisciplina(int turmaDisciplinaId)
        {
            var lista = await _atividadeRepo.ListarPorTurmaDisciplina(turmaDisciplinaId);

            return lista.Select(a => new AtividadeDTO
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Descricao = a.Descricao,
                DataEntrega = a.DataEntrega,
                Tipo = a.Tipo,
                TurmaDisciplinaId = a.TurmaDisciplinaId,
                TurmaNome = a.TurmaDisciplina.Turma.Nome,
                DisciplinaNome = a.TurmaDisciplina.Disciplina.Nome,
                ProfessorNome = a.TurmaDisciplina.Professor.Usuario.Nome
            });
        }
        public async Task<IEnumerable<AtividadeAlunoDTO>> ListarMinhasAtividades(int usuarioId)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new Exception("Aluno não encontrado.");

            // matrícula ativa do aluno
            var matricula = await _matriculaRepo.ObterAtivaPorAlunoId(aluno.Id)
                ?? throw new Exception("Aluno não possui matrícula ativa.");

            // buscar atividades da turma do aluno
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

    }
}
