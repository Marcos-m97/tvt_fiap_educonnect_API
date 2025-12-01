namespace EduConnect_API.Models.DTOs
{
    public class CriarUsuarioDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Tipo do usuário que será criado:
        // 0 = SuperAdmin
        // 1 = Admin
        // 2 = Professor
        // 3 = Aluno
        public int Tipo { get; set; }

        public string Senha { get; set; } = string.Empty;
    }
}
