using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class EntregaService : IEntregaService
    {
        private readonly IEntregaRepository _repo;
        private readonly IAtividadeRepository _atividadeRepo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly IArquivoStorageService _storage;

        public EntregaService(
            IEntregaRepository repo,
            IAtividadeRepository atividadeRepo,
            IAlunoRepository alunoRepo,
            IArquivoStorageService storage)
        {
            _repo = repo;
            _atividadeRepo = atividadeRepo;
            _alunoRepo = alunoRepo;
            _storage = storage;
        }

        // ============================================================
        // CRIAR ENTREGA
        // ============================================================
        public async Task<EntregaDTO> CriarEntrega(Guid usuarioId, Guid atividadeId, IFormFile arquivo)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new Exception("Aluno não encontrado.");

            var atividade = await _atividadeRepo.ObterPorId(atividadeId)
                ?? throw new Exception("Atividade não encontrada.");

            // Salvar arquivo no storage
            var caminho = await _storage.SalvarEntrega(atividadeId, aluno.Id, arquivo);

            var entrega = new EntregaAtividade
            {
                AlunoId = aluno.Id,
                AtividadeId = atividadeId,
                Arquivo = caminho
            };

            entrega = await _repo.Criar(entrega);

            return Map(entrega, atividade.Titulo, aluno.Usuario.Nome, aluno.Id, aluno.UsuarioId);
        }

        // ============================================================
        // CORRIGIR ENTREGA (professor)
        // ============================================================
        public async Task<EntregaDTO> Corrigir(Guid entregaId, decimal nota, string? feedback)
        {
            var entrega = await _repo.ObterPorId(entregaId)
                ?? throw new Exception("Entrega não encontrada.");

            entrega.Nota = nota;
            entrega.FeedbackProfessor = feedback;

            entrega = await _repo.Atualizar(entrega);

            var atividade = await _atividadeRepo.ObterPorId(entrega.AtividadeId)!;
            var nomeAluno = entrega.Aluno.Usuario.Nome;

            return Map(
                entrega,
                atividade.Titulo,
                nomeAluno,
                entrega.AlunoId,
                entrega.Aluno.UsuarioId
            );
        }

        // ============================================================
        // LISTAR ENTREGAS POR ATIVIDADE
        // ============================================================
        public async Task<IEnumerable<EntregaDTO>> ListarPorAtividade(Guid atividadeId)
        {
            var lista = await _repo.ListarPorAtividade(atividadeId);

            return lista.Select(e => Map(
                e,
                e.Atividade.Titulo,
                e.Aluno.Usuario.Nome,
                e.AlunoId,
                e.Aluno.UsuarioId
            ));
        }

        // ============================================================
        // MAPEAR PARA DTO (AGORA COMPLETO)
        // ============================================================
        private EntregaDTO Map(
            EntregaAtividade e,
            string titulo,
            string nomeAluno,
            Guid alunoId,
            Guid usuarioId)
        {
            return new EntregaDTO
            {
                Id = e.Id,
                AtividadeId = e.AtividadeId,

                // 🔹 CAMPOS DO ALUNO
                AlunoId = alunoId,
                UsuarioId = usuarioId,
                NomeAluno = nomeAluno,

                TituloAtividade = titulo,
                Nota = e.Nota,
                FeedbackProfessor = e.FeedbackProfessor,
                DataEnvio = e.DataEnvio,
                Arquivo = e.Arquivo
            };
        }
    }
}
