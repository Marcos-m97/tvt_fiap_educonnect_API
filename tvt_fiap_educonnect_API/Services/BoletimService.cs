using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utils;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Services
{
    public class BoletimService : IBoletimService
    {
        private readonly IBoletimRepository _repo;
        private readonly AppDbContext _context;

        public BoletimService(IBoletimRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<BoletimDTO> Gerar(CreateBoletimDTO dto)
        {
            // 1) Buscar disciplinas REAIS usando TurmaDisciplina
            var turmaDisciplinas = await _context.TurmaDisciplinas
                .Where(td => td.TurmaId == dto.TurmaId)
                .Include(td => td.Disciplina)
                .ToListAsync();

            if (!turmaDisciplinas.Any())
                throw new Exception("Nenhuma disciplina vinculada a esta turma.");

            // 2) Criar boletim
            var boletim = new Boletim
            {
                Id = Guid.NewGuid(),
                AlunoId = dto.AlunoId,
                TurmaId = dto.TurmaId,
                GeradoEm = DateTime.Now
            };

            // 3) Criar BoletimDisciplina para cada disciplina vinculada
            foreach (var td in turmaDisciplinas)
            {
                boletim.Disciplinas.Add(new BoletimDisciplina
                {
                    Id = Guid.NewGuid(),
                    NomeDisciplina = td.Disciplina.Nome,
                    Nota = 0,
                    Media = 0,
                    Situacao = "Cursando"
                });
            }

            // 4) Persistir no banco
            await _repo.Criar(boletim);

            return MapToDTO(boletim);
        }

        public async Task<BoletimDTO?> Obter(Guid boletimId)
        {
            var boletim = await _repo.Obter(boletimId);
            return boletim == null ? null : MapToDTO(boletim);
        }

        public async Task<IEnumerable<BoletimDTO>> ListarPorAluno(Guid alunoId)
        {
            var boletins = await _repo.ListarPorAluno(alunoId);
            return boletins.Select(b => MapToDTO(b));
        }

        public async Task<byte[]> GerarPdf(Guid boletimId)
        {
            var boletim = await _repo.Obter(boletimId);

            if (boletim == null)
                throw new Exception("Boletim não encontrado.");

            return BoletimPdfGenerator.GerarPdf(boletim);
        }

        // ===========================================
        // MAPEAR PARA DTO
        // ===========================================
        private BoletimDTO MapToDTO(Boletim boletim)
        {
            return new BoletimDTO
            {
                Id = boletim.Id,
                AlunoId = boletim.AlunoId,
                TurmaId = boletim.TurmaId,
                GeradoEm = boletim.GeradoEm,
                Disciplinas = boletim.Disciplinas.Select(d => new BoletimDisciplinaDTO
                {
                    NomeDisciplina = d.NomeDisciplina,
                    Nota = d.Nota,
                    Media = d.Media,
                    Situacao = d.Situacao
                }).ToList()
            };
        }
    }
}
