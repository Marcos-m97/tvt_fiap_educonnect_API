namespace EduConnect_API.Models.DTOs
{
    public class EntregaDTO
    {
        public Guid Id { get; set; }
        public Guid AtividadeId { get; set; }

        // 🔹 NOVOS CAMPOS
        public Guid AlunoId { get; set; }
        public Guid UsuarioId { get; set; }
        public string NomeAluno { get; set; } = string.Empty;

        public string TituloAtividade { get; set; } = string.Empty;
        public decimal? Nota { get; set; }
        public string? FeedbackProfessor { get; set; }
        public DateTime DataEnvio { get; set; }
        public string? Arquivo { get; set; }
    }
}
