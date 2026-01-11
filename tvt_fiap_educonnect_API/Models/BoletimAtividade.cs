namespace EduConnect_API.Models
{
    public class BoletimAtividade
    {
        public int Id { get; set; }

        public int BoletimDisciplinaId { get; set; }
        public BoletimDisciplina BoletimDisciplina { get; set; } = null!;

        public int AtividadeId { get; set; }
        public string Titulo { get; set; } = string.Empty;

        public double? Nota { get; set; }
        public bool Entregue { get; set; }
    }
}

