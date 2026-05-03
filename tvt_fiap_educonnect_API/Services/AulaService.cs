using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas às aulas.
    ///
    /// No EduConnect, uma aula é um conteúdo acadêmico vinculado a uma TurmaDisciplina,
    /// ou seja, a uma disciplina ofertada em uma turma específica.
    ///
    /// Essa camada coordena criação, atualização, listagem, upload de material de apoio,
    /// upload de vídeo aula, download de material e acesso às aulas do aluno conforme
    /// sua matrícula ativa.
    /// </summary>
    public class AulaService : IAulaService
    {
        private readonly IAulaRepository _repo;
        private readonly ITurmaDisciplinaRepository _turmaDisciplinaRepo;
        private readonly IUsuarioRepository _usuarios;
        private readonly IAlunoRepository _alunoRepo;
        private readonly IMatriculaRepository _matriculaRepo;
        private readonly IArquivoStorageService _storage;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IAulaRepository: acesso aos dados de aula.
        /// ITurmaDisciplinaRepository: valida o contexto acadêmico da aula.
        /// IUsuarioRepository: identifica o usuário que cria ou atualiza a aula.
        /// IAlunoRepository: localiza o perfil acadêmico do aluno logado.
        /// IMatriculaRepository: verifica se o aluno possui matrícula ativa.
        /// IArquivoStorageService: salva e recupera arquivos vinculados à aula.
        /// </summary>
        public AulaService(
            IAulaRepository repo,
            ITurmaDisciplinaRepository turmaDisciplinaRepo,
            IUsuarioRepository usuarios,
            IAlunoRepository alunoRepo,
            IMatriculaRepository matriculaRepo,
            IArquivoStorageService storage)
        {
            _repo = repo;
            _turmaDisciplinaRepo = turmaDisciplinaRepo;
            _usuarios = usuarios;
            _alunoRepo = alunoRepo;
            _matriculaRepo = matriculaRepo;
            _storage = storage;
        }

        // =========================================================
        // 1. CRIAR AULA
        // =========================================================

        /// <summary>
        /// Cria uma nova aula vinculada a uma TurmaDisciplina.
        ///
        /// O método valida se o usuário existe e se possui permissão para criar aulas.
        /// No fluxo atual, SuperAdmin, Admin e Professor podem criar aulas.
        ///
        /// Também valida se a TurmaDisciplina informada existe, garantindo que a aula
        /// seja criada dentro de um contexto acadêmico válido.
        /// </summary>
        public async Task<AulaDTO> Criar(int usuarioId, CriarAulaDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(usuarioId)
                ?? throw new AppException("Usuário não encontrado.", 404);

            if (usuario.Tipo != 0 && usuario.Tipo != 1 && usuario.Tipo != 2)
                throw new AppException("Usuário não autorizado a criar aulas.", 403);

            var turmaDisciplina = await _turmaDisciplinaRepo.ObterPorId(dto.TurmaDisciplinaId)
                ?? throw new AppException("TurmaDisciplina não encontrada.", 404);

            var aula = new Aula
            {
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
        // 2. LISTAR MINHAS AULAS
        // =========================================================

        /// <summary>
        /// Lista as aulas disponíveis para o aluno logado.
        ///
        /// O método localiza o perfil de aluno a partir do usuário autenticado
        /// e verifica se existe uma matrícula efetivada/ativa.
        ///
        /// A partir da turma da matrícula ativa, busca todas as aulas relacionadas
        /// às disciplinas daquela turma.
        /// </summary>
        public async Task<IEnumerable<AulaDTO>> ListarMinhasAulas(int usuarioId)
        {
            var aluno = await _alunoRepo.ObterPorUsuarioId(usuarioId)
                ?? throw new AppException("Aluno não encontrado.", 404);

            var matricula = await _matriculaRepo.ObterAtivaPorAlunoId(aluno.Id)
                ?? throw new AppException("Aluno não possui matrícula ativa.", 404);

            var aulas = await _repo.ListarPorTurma(matricula.TurmaId);

            return aulas.Select(MapToDTO);
        }

        // =========================================================
        // 3. UPLOAD MATERIAL DE APOIO
        // =========================================================

        /// <summary>
        /// Realiza o upload do material de apoio da aula.
        ///
        /// O arquivo é salvo pelo serviço de storage e o caminho retornado
        /// é gravado no campo MaterialApoio da aula.
        /// </summary>
        public async Task<AulaDTO?> UploadMaterialApoio(int aulaId, IFormFile arquivo)
        {
            var aula = await _repo.ObterPorId(aulaId);

            if (aula == null)
                return null;

            var caminho = $"uploads/aulas/{aulaId}/material_apoio.pdf";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            aula.MaterialApoio = caminhoSalvo;

            aula = await _repo.Atualizar(aula);

            return MapToDTO(aula);
        }

        // =========================================================
        // 4. UPLOAD VIDEO AULA
        // =========================================================

        /// <summary>
        /// Realiza o upload do vídeo principal da aula.
        ///
        /// O fluxo atual aceita apenas arquivos MP4.
        /// Após o salvamento, o caminho do vídeo é gravado no campo VideoAula.
        /// </summary>
        public async Task<AulaDTO?> UploadVideoAula(int aulaId, IFormFile arquivo)
        {
            var aula = await _repo.ObterPorId(aulaId);

            if (aula == null)
                return null;

            if (!arquivo.FileName.EndsWith(".mp4"))
                throw new AppException("Apenas arquivos MP4 são permitidos.", 400);

            var nomeArquivo = $"aula_{aulaId}_{Guid.NewGuid()}.mp4";
            var caminho = $"uploads/videos/{nomeArquivo}";
            var caminhoSalvo = await _storage.SalvarAsync(arquivo, caminho);

            aula.VideoAula = caminhoSalvo;

            aula = await _repo.Atualizar(aula);

            return MapToDTO(aula);
        }

        // =========================================================
        // 5. DOWNLOAD MATERIAL DE APOIO
        // =========================================================

        /// <summary>
        /// Baixa o material de apoio vinculado à aula.
        ///
        /// Retorna null quando a aula não existe ou quando não há material
        /// de apoio cadastrado.
        /// </summary>
        public async Task<byte[]?> BaixarMaterialApoio(int aulaId)
        {
            var aula = await _repo.ObterPorId(aulaId);

            if (aula?.MaterialApoio == null)
                return null;

            return await _storage.BaixarAsync(aula.MaterialApoio);
        }

        // =========================================================
        // 6. CONSULTAS
        // =========================================================

        /// <summary>
        /// Obtém uma aula pelo ID.
        /// </summary>
        public async Task<AulaDTO?> ObterPorId(int id)
        {
            var aula = await _repo.ObterPorId(id);

            return aula == null ? null : MapToDTO(aula);
        }

        /// <summary>
        /// Lista aulas vinculadas a uma TurmaDisciplina específica.
        ///
        /// Usado para exibir as aulas de determinada disciplina dentro de uma turma.
        /// </summary>
        public async Task<IEnumerable<AulaDTO>> ListarPorTurmaDisciplina(int turmaDisciplinaId)
        {
            var lista = await _repo.ListarPorTurmaDisciplina(turmaDisciplinaId);

            return lista.Select(MapToDTO);
        }

        /// <summary>
        /// Lista todas as aulas cadastradas.
        ///
        /// Pode ser usado em contextos administrativos ou listagens gerais.
        /// </summary>
        public async Task<IEnumerable<AulaDTO>> Listar()
        {
            var lista = await _repo.Listar();

            return lista.Select(MapToDTO);
        }

        // =========================================================
        // 7. DELETAR AULA
        // =========================================================

        /// <summary>
        /// Remove uma aula.
        ///
        /// A operação é delegada ao Repository.
        /// </summary>
        public Task<bool> Deletar(int id)
        {
            return _repo.Deletar(id);
        }

        // =========================================================
        // 8. ATUALIZAR AULA
        // =========================================================

        /// <summary>
        /// Atualiza os dados de uma aula existente.
        ///
        /// O método valida se o usuário existe e se possui permissão para atualizar aulas.
        /// No fluxo atual, SuperAdmin, Admin e Professor podem atualizar aulas.
        ///
        /// Depois, atualiza os campos editáveis da aula e persiste as alterações.
        /// </summary>
        public async Task<AulaDTO?> Atualizar(int aulaId, int usuarioId, AtualizarAulaDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(usuarioId)
                ?? throw new AppException("Usuário não encontrado.", 404);

            if (usuario.Tipo != 0 && usuario.Tipo != 1 && usuario.Tipo != 2)
                throw new AppException("Usuário não autorizado a atualizar aulas.", 403);

            var aula = await _repo.ObterPorId(aulaId);

            if (aula == null)
                return null;

            aula.Titulo = dto.Titulo;
            aula.Descricao = dto.Descricao;
            aula.UrlVideo = dto.UrlVideo;
            aula.Observacoes = dto.Observacoes;

            aula = await _repo.Atualizar(aula);

            return MapToDTO(aula);
        }

        // =========================================================
        // 9. MAPEAMENTO PARA DTO
        // =========================================================

        /// <summary>
        /// Converte a entidade Aula em AulaDTO.
        ///
        /// Esse mapeamento centraliza os dados que serão retornados ao frontend,
        /// evitando expor diretamente a entidade do banco.
        /// </summary>
        private AulaDTO MapToDTO(Aula a)
        {
            return new AulaDTO
            {
                Id = a.Id,
                TurmaDisciplinaId = a.TurmaDisciplinaId,
                Titulo = a.Titulo,
                Descricao = a.Descricao,
                UrlVideo = a.UrlVideo,
                VideoAula = a.VideoAula,
                MaterialApoio = a.MaterialApoio,
                Observacoes = a.Observacoes,
                CriadoEm = a.CriadoEm,
                CriadoPor = a.CriadoPor
            };
        }
    }
}