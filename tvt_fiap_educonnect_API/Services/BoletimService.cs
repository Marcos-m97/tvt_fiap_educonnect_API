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
            // Buscar disciplinas vinculadas à turma
            var turmaDisciplinas = await _context.TurmaDisciplinas
                .Where(td => td.TurmaId == dto.TurmaId)
                .Include(td => td.Disciplina)
                .ToListAsync();

            if (!turmaDisciplinas.Any())
                throw new Exception("Nenhuma disciplina vinculada a esta turma.");

            var boletim = new Boletim
            {
                Id = Guid.NewGuid(),
                AlunoId = dto.AlunoId,
                TurmaId = dto.TurmaId,
                GeradoEm = DateTime.Now
            };

            foreach (var td in turmaDisciplinas)
            {
                // Buscar todas as atividades da disciplina
                var atividades = await _context.Atividades
                    .Where(a => a.TurmaDisciplinaId == td.Id)
                    .ToListAsync();

                double somaNotas = 0;
                int totalAtividades = atividades.Count;
                int atividadesComEntrega = 0;

                foreach (var atv in atividades)
                {
                    // Buscar entrega do aluno
                    var entrega = await _context.EntregasAtividades
                        .Where(e => e.AtividadeId == atv.Id && e.AlunoId == dto.AlunoId)
                        .FirstOrDefaultAsync();

                    if (entrega != null)
                    {
                        somaNotas += (double)(entrega.Nota ?? 0);
                        atividadesComEntrega++;
                    }
                }

                double media = 0;
                string situacao = "Cursando";

                if (totalAtividades == 0)
                {
                    media = 0;
                    situacao = "Sem Avaliação";
                }
                else
                {
                    media = somaNotas / totalAtividades;

                    if (atividadesComEntrega == 0)
                        situacao = "Cursando";
                    else if (media >= 6)
                        situacao = "Aprovado";
                    else
                        situacao = "Reprovado";
                }

                boletim.Disciplinas.Add(new BoletimDisciplina
                {
                    Id = Guid.NewGuid(),
                    NomeDisciplina = td.Disciplina.Nome,
                    Nota = somaNotas,  // Somatório das notas
                    Media = media,
                    Situacao = situacao,
                    TotalAtividades = totalAtividades

                });
            }

            await _repo.Criar(boletim);

            return MapToDTO(boletim);
        }

        public async Task<BoletimDTO?> Obter(Guid boletimId)
        {
            var entity = await _repo.Obter(boletimId);
            return entity == null ? null : MapToDTO(entity);
        }

        public async Task<IEnumerable<BoletimDTO>> ListarPorAluno(Guid alunoId)
        {
            var lista = await _repo.ListarPorAluno(alunoId);
            return lista.Select(MapToDTO);
        }

        public async Task<byte[]> GerarPdf(Guid boletimId)
        {
            var boletim = await _repo.Obter(boletimId)
                ?? throw new Exception("Boletim não encontrado.");

            return BoletimPdfGenerator.GerarPdf(boletim);
        }

        private BoletimDTO MapToDTO(Boletim entity)
        {
            return new BoletimDTO
            {
                Id = entity.Id,
                AlunoId = entity.AlunoId,
                TurmaId = entity.TurmaId,
                GeradoEm = entity.GeradoEm,
                Disciplinas = entity.Disciplinas.Select(d => new BoletimDisciplinaDTO
                {
                    NomeDisciplina = d.NomeDisciplina,
                    Nota = d.Nota,
                    Media = d.Media,
                    Situacao = d.Situacao,
                    TotalAtividades = d.TotalAtividades
                }).ToList()
            };
        }
    }
}
