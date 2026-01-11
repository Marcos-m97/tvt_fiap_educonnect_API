namespace EduConnect_API.Models.DTOs
{
    public class EntregaAlunoDTO
    {
        public int EntregaId { get; set; }
        public int AtividadeId { get; set; }

        public string TituloAtividade { get; set; } = string.Empty;

        public int DisciplinaId { get; set; }
        public string NomeDisciplina { get; set; } = string.Empty;

        public DateTime DataEnvio { get; set; }
        public decimal? Nota { get; set; }
        public string? FeedbackProfessor { get; set; }
        public string? Arquivo { get; set; }
    }
}