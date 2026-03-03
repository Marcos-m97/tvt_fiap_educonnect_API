namespace EduConnect_API.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public int Tipo { get; set; } // 0=superAdmin, 1=Admin, 2=Professor, 3=Aluno
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;
        public string? FotoPerfilUrl { get; set; } // NOVO

    }
}
