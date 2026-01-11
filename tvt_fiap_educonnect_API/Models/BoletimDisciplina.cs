namespace EduConnect_API.Models
{
    public class BoletimDisciplina
    {
        public int Id { get; set; }

        public int BoletimId { get; set; }
        public Boletim Boletim { get; set; } = null!;

        public string NomeDisciplina { get; set; } = string.Empty;

        public double Nota { get; set; }
        public double Media { get; set; }
        public string Situacao { get; set; } = string.Empty;
        public int TotalAtividades { get; set; }

        public List<BoletimAtividade> Atividades { get; set; } = new();
    }
}