namespace EduConnect_API.Models
{
    public class BoletimAtividade
    {
        public Guid Id { get; set; }

        public Guid BoletimDisciplinaId { get; set; }
        public BoletimDisciplina BoletimDisciplina { get; set; } = null!;

        public Guid AtividadeId { get; set; }
        public string Titulo { get; set; } = string.Empty;

        public double? Nota { get; set; }
        public bool Entregue { get; set; }
    }
}

