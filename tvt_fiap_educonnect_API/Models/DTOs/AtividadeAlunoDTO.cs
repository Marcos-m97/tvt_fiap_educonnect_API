namespace EduConnect_API.Models.DTOs
{
    public class AtividadeAlunoDTO
    {
        public int AtividadeId { get; set; }
        public string Titulo { get; set; } = string.Empty;

        public int DisciplinaId { get; set; }
        public string NomeDisciplina { get; set; } = string.Empty;

        public DateTime DataEntrega { get; set; }

        // Status para o front decidir o que mostrar
        public bool JaEntregue { get; set; }
        public decimal? Nota { get; set; }
    }
}
