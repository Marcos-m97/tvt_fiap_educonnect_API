namespace EduConnect_API.Models.DTOs
{
    public class BoletimAtividadeDTO
    {
        public Guid AtividadeId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public double? Nota { get; set; }
        public bool Entregue { get; set; }
    }
}