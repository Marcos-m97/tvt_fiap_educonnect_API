namespace tvt_fiap_educonnect_API.Models.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
    }

}
