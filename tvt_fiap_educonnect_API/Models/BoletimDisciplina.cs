namespace EduConnect_API.Models
{
    public class BoletimDisciplina
    {
        public Guid Id { get; set; }
        public Guid BoletimId { get; set; }

        public string NomeDisciplina { get; set; } = string.Empty;
        public double Nota { get; set; }
        public double Media { get; set; }
        public string Situacao { get; set; } = string.Empty;

        public Boletim? Boletim { get; set; }
    }
}
