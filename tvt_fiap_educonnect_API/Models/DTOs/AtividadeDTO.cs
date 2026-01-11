namespace EduConnect_API.Models.DTOs
{
    public class AtividadeDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataEntrega { get; set; }
        public TipoAtividade Tipo { get; set; }

        public int TurmaDisciplinaId { get; set; }
        public string TurmaNome { get; set; } = string.Empty;
        public string DisciplinaNome { get; set; } = string.Empty;
        public string ProfessorNome { get; set; } = string.Empty;
    }
}