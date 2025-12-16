namespace EduConnect_API.Models.DTOs
{
    public class BoletimDisciplinaDTO
    {
        public string NomeDisciplina { get; set; } = string.Empty;

        public int TotalAtividades { get; set; }
        public double Nota { get; set; }     // soma das notas
        public double Media { get; set; }
        public string Situacao { get; set; } = string.Empty;

        public List<BoletimAtividadeDTO> Atividades { get; set; } = new();
    }
}
