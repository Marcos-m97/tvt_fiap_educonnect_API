using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class AtividadeService : IAtividadeService
    {
        private readonly IAtividadeRepository _repo;
        private readonly ITurmaDisciplinaRepository _tdRepo;

        public AtividadeService(
            IAtividadeRepository repo,
            ITurmaDisciplinaRepository tdRepo)
        {
            _repo = repo;
            _tdRepo = tdRepo;
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

            atividade = await _repo.Criar(atividade);

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

        public async Task<IEnumerable<AtividadeDTO>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId)
        {
            var lista = await _repo.ListarPorTurmaDisciplina(turmaDisciplinaId);

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
    }
}
