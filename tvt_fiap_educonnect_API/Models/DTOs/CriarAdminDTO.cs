namespace EduConnect_API.Models.DTOs
{
    public class CriarAdminDTO
    {
        public Guid UsuarioId { get; set; }
        public string? Departamento { get; set; }
        public string? Cargo { get; set; }
    }

}
