namespace EduConnect_API.Models.DTOs { 

    public class CriarEntregaDTO
    {
        public Guid AtividadeId { get; set; }
        public string ArquivoBase64 { get; set; } = string.Empty;
    }
}