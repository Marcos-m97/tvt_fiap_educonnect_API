namespace EduConnect_API.Models.DTOs
{
    public class EnviarDocumentosDTO
    {
        public string DocumentosPessoaisBase64 { get; set; } = string.Empty;
        public string DocumentosEscolaridadeBase64 { get; set; } = string.Empty;
    }
}
