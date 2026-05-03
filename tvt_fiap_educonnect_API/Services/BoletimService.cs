using EduConnect_API.Data;
using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utils;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas ao boletim.
    ///
    /// No EduConnect, o boletim consolida o desempenho acadêmico de um aluno
    /// em uma turma, agrupando os resultados por disciplina e detalhando
    /// as atividades consideradas no cálculo.
    ///
    /// Essa camada gera boletins persistidos, monta prévias sem salvar,
    /// consulta boletins existentes e gera o PDF do boletim.
    /// </summary>
    public class BoletimService : IBoletimService
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// Neste serviço, o AppDbContext é usado diretamente porque o cálculo
        /// do boletim depende de consultas em várias tabelas relacionadas:
        /// TurmaDisciplina, Atividades, EntregasAtividades, Alunos e Turmas.
        /// </summary>
        public BoletimService(AppDbContext context)
        {
            _context = context;
        }

        // ==============================================================
        // 1. GERAR BOLETIM
        // ==============================================================

        /// <summary>
        /// Gera e salva um novo boletim para um aluno em uma turma.
        ///
        /// O método busca todas as disciplinas vinculadas à turma, percorre
        /// as atividades de cada disciplina e verifica se o aluno possui entrega
        /// registrada para cada atividade.
        ///
        /// A partir disso, calcula:
        /// - soma das notas;
        /// - média da disciplina;
        /// - total de atividades;
        /// - situação acadêmica;
        /// - detalhe de cada atividade.
        ///
        /// No fluxo atual, a geração sempre cria um novo registro de boletim,
        /// sem atualizar boletins anteriores.
        /// </summary>
        public async Task<BoletimDTO> Gerar(CreateBoletimDTO dto)
        {
            var turmaDisciplinas = await _context.TurmaDisciplinas
                .Where(td => td.TurmaId == dto.TurmaId)
                .Include(td => td.Disciplina)
                .ToListAsync();

            if (!turmaDisciplinas.Any())
                throw new AppException("Nenhuma disciplina vinculada a esta turma.", 404);

            var boletim = new Boletim
            {
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
        // 2. OBTER BOLETIM
        // ==============================================================

        /// <summary>
        /// Obtém um boletim pelo ID.
        ///
        /// A consulta carrega as disciplinas e as atividades vinculadas a cada
        /// disciplina, permitindo retornar ao frontend a estrutura completa
        /// do boletim.
        /// </summary>
        public async Task<BoletimDTO?> Obter(int boletimId)
        {
            var boletim = await _context.Boletins
                .Include(b => b.Disciplinas)
                    .ThenInclude(d => d.Atividades)
                .FirstOrDefaultAsync(b => b.Id == boletimId);

            return boletim == null ? null : MapToDTO(boletim);
        }

        // ==============================================================
        // 3. LISTAR POR ALUNO
        // ==============================================================

        /// <summary>
        /// Lista todos os boletins de um aluno.
        ///
        /// A ordenação exibe primeiro os boletins gerados mais recentemente.
        /// Esse método pode ser usado na visão do aluno ou em telas administrativas
        /// de acompanhamento acadêmico.
        /// </summary>
        public async Task<IEnumerable<BoletimDTO>> ListarPorAluno(int alunoId)
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
        // 4. GERAR PDF
        // ==============================================================

        /// <summary>
        /// Gera o PDF de um boletim existente.
        ///
        /// O método busca o boletim, o aluno e a turma para enviar ao gerador
        /// de PDF as informações necessárias, como nome do aluno e nome da turma.
        /// </summary>
        public async Task<byte[]> GerarPdf(int boletimId)
        {
            var boletim = await Obter(boletimId)
                ?? throw new AppException("Boletim não encontrado.", 404);

            var aluno = await _context.Alunos
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == boletim.AlunoId)
                ?? throw new AppException("Aluno não encontrado.", 404);

            var turma = await _context.Turmas
                .FirstOrDefaultAsync(t => t.Id == boletim.TurmaId)
                ?? throw new AppException("Turma não encontrada.", 404);

            return BoletimPdfGenerator.GerarPdf(
                boletim,
                aluno.Usuario.Nome,
                turma.Nome
            );
        }

        // ==============================================================
        // 5. PREVIEW DO BOLETIM
        // ==============================================================

        /// <summary>
        /// Monta uma prévia do boletim sem salvar no banco.
        ///
        /// A lógica de cálculo é semelhante ao método Gerar, mas o boletim
        /// criado fica apenas em memória e é retornado como DTO.
        ///
        /// Esse fluxo é útil para o frontend exibir uma simulação do boletim
        /// antes de persistir oficialmente o registro.
        /// </summary>
        public async Task<BoletimDTO> Preview(int alunoId, int turmaId)
        {
            var turmaDisciplinas = await _context.TurmaDisciplinas
                .Where(td => td.TurmaId == turmaId)
                .Include(td => td.Disciplina)
                .ToListAsync();

            if (!turmaDisciplinas.Any())
                throw new AppException("Nenhuma disciplina vinculada a esta turma.", 404);

            var boletim = new Boletim
            {
                AlunoId = alunoId,
                TurmaId = turmaId,
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
                    NomeDisciplina = td.Disciplina.Nome,
                    TotalAtividades = totalAtividades
                };

                foreach (var atv in atividades)
                {
                    var entrega = await _context.EntregasAtividades
                        .FirstOrDefaultAsync(e =>
                            e.AtividadeId == atv.Id &&
                            e.AlunoId == alunoId
                        );

                    if (entrega != null)
                    {
                        somaNotas += (double)(entrega.Nota ?? 0);
                        atividadesComEntrega++;
                    }

                    boletimDisciplina.Atividades.Add(new BoletimAtividade
                    {
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

            return MapToDTO(boletim);
        }

        // ==============================================================
        // 6. MAPEAMENTO PARA DTO
        // ==============================================================

        /// <summary>
        /// Converte a entidade Boletim em BoletimDTO.
        ///
        /// O DTO mantém a estrutura hierárquica do boletim:
        /// boletim geral, disciplinas e atividades de cada disciplina.
        ///
        /// Isso facilita o consumo pelo frontend, que consegue renderizar
        /// o boletim agrupado sem precisar montar a estrutura manualmente.
        /// </summary>
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