using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utils;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Services
{
    public class BoletimService : IBoletimService
    {
        private readonly AppDbContext _context;

        public BoletimService(AppDbContext context)
        {
            _context = context;
        }

        // ==============================================================
        // GERAR BOLETIM (SEMPRE INSERT – SEM UPDATE)
        // ==============================================================
        public async Task<BoletimDTO> Gerar(CreateBoletimDTO dto)
        {
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
                var atividades = await _context.Atividades
                    .Where(a => a.TurmaDisciplinaId == td.Id)
                    .ToListAsync();

                double somaNotas = 0;
                int totalAtividades = atividades.Count;
                int atividadesComEntrega = 0;

                var boletimDisciplina = new BoletimDisciplina
                {
                    Id = Guid.NewGuid(),
                    NomeDisciplina = td.Disciplina.Nome,
                    TotalAtividades = totalAtividades
                };

                foreach (var atv in atividades)
                {
                    var entrega = await _context.EntregasAtividades
                        .FirstOrDefaultAsync(e =>
                            e.AtividadeId == atv.Id &&
                            e.AlunoId == dto.AlunoId
                        );

                    if (entrega != null)
                    {
                        somaNotas += (double)(entrega.Nota ?? 0);
                        atividadesComEntrega++;
                    }

                    boletimDisciplina.Atividades.Add(new BoletimAtividade
                    {
                        Id = Guid.NewGuid(),
                        AtividadeId = atv.Id,
                        Titulo = atv.Titulo,
                        Nota = entrega?.Nota.HasValue == true
                            ? (double?)entrega.Nota.Value
                            : null,
                        Entregue = entrega != null
                    });
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

                boletimDisciplina.Nota = somaNotas;
                boletimDisciplina.Media = media;
                boletimDisciplina.Situacao = situacao;

                boletim.Disciplinas.Add(boletimDisciplina);
            }

            _context.Boletins.Add(boletim);
            await _context.SaveChangesAsync();

            return MapToDTO(boletim);
        }

        // ==============================================================
        // OBTER BOLETIM
        // ==============================================================
        public async Task<BoletimDTO?> Obter(Guid boletimId)
        {
            var boletim = await _context.Boletins
                .Include(b => b.Disciplinas)
                    .ThenInclude(d => d.Atividades)
                .FirstOrDefaultAsync(b => b.Id == boletimId);

            return boletim == null ? null : MapToDTO(boletim);
        }

        // ==============================================================
        // LISTAR POR ALUNO
        // ==============================================================
        public async Task<IEnumerable<BoletimDTO>> ListarPorAluno(Guid alunoId)
        {
            var lista = await _context.Boletins
                .Include(b => b.Disciplinas)
                    .ThenInclude(d => d.Atividades)
                .Where(b => b.AlunoId == alunoId)
                .OrderByDescending(b => b.GeradoEm)
                .ToListAsync();

            return lista.Select(MapToDTO);
        }

        // ==============================================================
        // GERAR PDF
        // ==============================================================
        public async Task<byte[]> GerarPdf(Guid boletimId)
        {
            var boletim = await Obter(boletimId)
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
        // MAP DTO
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
                    TotalAtividades = d.TotalAtividades,
                    Atividades = d.Atividades.Select(a => new BoletimAtividadeDTO
                    {
                        AtividadeId = a.AtividadeId,
                        Titulo = a.Titulo,
                        Nota = a.Nota,
                        Entregue = a.Entregue
                    }).ToList()
                }).ToList()
            };
        }
    }
}
