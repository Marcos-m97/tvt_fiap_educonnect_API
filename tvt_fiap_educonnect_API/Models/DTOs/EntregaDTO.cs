namespace EduConnect_API.Models.DTOs
{
    public class EntregaDTO
    {
        public int Id { get; set; }
        public int AtividadeId { get; set; }

        // 🔹 NOVOS CAMPOS
        public int AlunoId { get; set; }
        public int UsuarioId { get; set; }
        public string NomeAluno { get; set; } = string.Empty;

        public string TituloAtividade { get; set; } = string.Empty;
        public decimal? Nota { get; set; }
        public string? FeedbackProfessor { get; set; }
        public DateTime DataEnvio { get; set; }
        public string? Arquivo { get; set; }
    }
}
