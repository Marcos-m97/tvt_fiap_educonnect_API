namespace EduConnect_API.Models.DTOs
{
    public class AtividadeDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataEntrega { get; set; }
        public TipoAtividade Tipo { get; set; }

        public Guid TurmaDisciplinaId { get; set; }
        public string TurmaNome { get; set; } = string.Empty;
        public string DisciplinaNome { get; set; } = string.Empty;
        public string ProfessorNome { get; set; } = string.Empty;
    }
}