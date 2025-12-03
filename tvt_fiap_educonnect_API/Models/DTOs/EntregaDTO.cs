namespace EduConnect_API.Models.DTOs
{
    public class EntregaDTO
    {
        public Guid Id { get; set; }
        public Guid AtividadeId { get; set; }
        public string TituloAtividade { get; set; }
        public decimal? Nota { get; set; }
        public string? FeedbackProfessor { get; set; }
        public DateTime DataEnvio { get; set; }
        public string? Arquivo { get; set; }
    }

}