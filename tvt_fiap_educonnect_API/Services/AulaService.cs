using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class AulaService : IAulaService
    {
        private readonly IAulaRepository _repo;
        private readonly ITurmaDisciplinaRepository _turmaDisciplinaRepo;
        private readonly IUsuarioRepository _usuarios;
        private readonly IArquivoStorageService _storage;

        public AulaService(
            IAulaRepository repo,
            ITurmaDisciplinaRepository turmaDisciplinaRepo,
            IUsuarioRepository usuarios,
            IArquivoStorageService storage)
        {
            _repo = repo;
            _turmaDisciplinaRepo = turmaDisciplinaRepo;
            _usuarios = usuarios;
            _storage = storage;
        }

        // =========================================================
        // CRIAR AULA (Professor ou Admin)
        // =========================================================
        public async Task<AulaDTO> Criar(Guid usuarioId, CriarAulaDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(usuarioId)
                ?? throw new Exception("Usuário não encontrado.");

            if (usuario.Tipo != 0 && usuario.Tipo != 1 && usuario.Tipo != 2)
                throw new Exception("Usuário não autorizado a criar aulas.");

            var turmaDisciplina = await _turmaDisciplinaRepo.ObterPorId(dto.TurmaDisciplinaId)
                ?? throw new Exception("TurmaDisciplina não encontrada.");

            var aula = new Aula
            {
                Id = Guid.NewGuid(),
                TurmaDisciplinaId = dto.TurmaDisciplinaId,
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                UrlVideo = dto.UrlVideo,
                Observacoes = dto.Observacoes,
                CriadoPor = usuario.Tipo == 1 ? "Administrador" : "Professor"
            };

            aula = await _repo.Criar(aula);

            return MapToDTO(aula);
        }

        // =========================================================
        // UPLOAD MATERIAL DE APOIO (PDF)
        // =========================================================
        public async Task<AulaDTO?> UploadMaterialApoio(Guid aulaId, IFormFile arquivo)
        {
            var aula = await _repo.ObterPorId(aulaId);
            if (aula == null) return null;

            var caminho = $"uploads/aulas/{aulaId}/material_apoio.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            aula.MaterialApoio = caminhoSalvo;
            aula = await _repo.Atualizar(aula);

            return MapToDTO(aula);
        }

        // =========================================================
        // BAIXAR MATERIAL DE APOIO
        // =========================================================
        public async Task<byte[]?> BaixarMaterialApoio(Guid aulaId)
        {
            var aula = await _repo.ObterPorId(aulaId);
            if (aula?.MaterialApoio == null) return null;

            return await _storage.BaixarAsync(aula.MaterialApoio);
        }

        // =========================================================
        // CONSULTAS
        // =========================================================
        public async Task<AulaDTO?> ObterPorId(Guid id)
        {
            var aula = await _repo.ObterPorId(id);
            return aula == null ? null : MapToDTO(aula);
        }

        public async Task<IEnumerable<AulaDTO>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId)
        {
            var lista = await _repo.ListarPorTurmaDisciplina(turmaDisciplinaId);
            return lista.Select(MapToDTO);
        }

        public async Task<IEnumerable<AulaDTO>> Listar()
        {
            var lista = await _repo.Listar();
            return lista.Select(MapToDTO);
        }

        // =========================================================
        // DELETAR
        // =========================================================
        public Task<bool> Deletar(Guid id) => _repo.Deletar(id);

        // =========================================================
        // MAP
        // =========================================================
        private AulaDTO MapToDTO(Aula a)
        {
            return new AulaDTO
            {
                Id = a.Id,
                TurmaDisciplinaId = a.TurmaDisciplinaId,
                Titulo = a.Titulo,
                Descricao = a.Descricao,
                UrlVideo = a.UrlVideo,
                MaterialApoio = a.MaterialApoio,
                Observacoes = a.Observacoes,
                CriadoEm = a.CriadoEm,
                CriadoPor = a.CriadoPor
            };
        }
    }
}