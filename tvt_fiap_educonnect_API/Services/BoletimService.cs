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

        // ==============================================================
        // GERAR BOLETIM
        // ==============================================================
        public async Task<BoletimDTO> Gerar(CreateBoletimDTO dto)
        {
            // Buscar disciplinas da turma
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
                // Buscar atividades daquela disciplina
                var atividades = await _context.Atividades
                    .Where(a => a.TurmaDisciplinaId == td.Id)
                    .ToListAsync();

                double somaNotas = 0;
                int totalAtividades = atividades.Count;
                int atividadesComEntrega = 0;

                foreach (var atv in atividades)
                {
                    var entrega = await _context.EntregasAtividades
                        .FirstOrDefaultAsync(e => e.AtividadeId == atv.Id && e.AlunoId == dto.AlunoId);

                    if (entrega != null)
                    {
                        somaNotas += (double)(entrega.Nota ?? 0);
                        atividadesComEntrega++;
                    }
                }

                double media = 0;
                string situacao;

                if (totalAtividades == 0)
                {
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
                    Nota = somaNotas,
                    Media = media,
                    Situacao = situacao,
                    TotalAtividades = totalAtividades
                });
            }

            await _repo.Criar(boletim);

            return MapToDTO(boletim);
        }

        // ==============================================================
        // OBTER UM BOLETIM
        // ==============================================================
        public async Task<BoletimDTO?> Obter(Guid boletimId)
        {
            var boletim = await _repo.Obter(boletimId);
            return boletim == null ? null : MapToDTO(boletim);
        }

        // ==============================================================
        // LISTAR POR ALUNO
        // ==============================================================
        public async Task<IEnumerable<BoletimDTO>> ListarPorAluno(Guid alunoId)
        {
            var lista = await _repo.ListarPorAluno(alunoId);
            return lista.Select(MapToDTO);
        }

        // ==============================================================
        // GERAR PDF COM NOME DO ALUNO E NOME DA TURMA
        // ==============================================================
        public async Task<byte[]> GerarPdf(Guid boletimId)
        {
            var boletim = await _repo.Obter(boletimId)
                ?? throw new Exception("Boletim não encontrado.");

            var aluno = await _context.Alunos
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == boletim.AlunoId)
                ?? throw new Exception("Aluno não encontrado.");

            var turma = await _context.Turmas
                .FirstOrDefaultAsync(t => t.Id == boletim.TurmaId)
                ?? throw new Exception("Turma não encontrada.");

            return BoletimPdfGenerator.GerarPdf(
                boletim,
                aluno.Usuario.Nome,
                turma.Nome
            );
        }

        // ==============================================================
        // MAPEAR PARA DTO
        // ==============================================================
        private BoletimDTO MapToDTO(Boletim b)
        {
            return new BoletimDTO
            {
                Id = b.Id,
                AlunoId = b.AlunoId,
                TurmaId = b.TurmaId,
                GeradoEm = b.GeradoEm,
                Disciplinas = b.Disciplinas.Select(d => new BoletimDisciplinaDTO
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
