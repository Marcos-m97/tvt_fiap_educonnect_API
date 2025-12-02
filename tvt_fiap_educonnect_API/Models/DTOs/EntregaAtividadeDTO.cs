namespace EduConnect_API.Models.DTOs
{
    public class EntregaAtividadeDTO
    {
        public Guid Id { get; set; }
        public Guid AtividadeId { get; set; }
        public Guid AlunoId { get; set; }
        public string AlunoNome { get; set; } = string.Empty;

        public DateTime DataEnvio { get; set; }
        public decimal? Nota { get; set; }
        public string? FeedbackProfessor { get; set; }
    }
}