namespace EduConnect_API.Models.DTOs
{
    public class BoletimDisciplinaDTO
    {
        public string NomeDisciplina { get; set; } = string.Empty;
        public double Nota { get; set; }
        public double Media { get; set; }
        public string Situacao { get; set; } = string.Empty;
        public int TotalAtividades { get; set; }
    }
}