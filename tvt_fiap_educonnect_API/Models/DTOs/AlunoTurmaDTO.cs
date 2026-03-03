using EduConnect_API.Models;

namespace EduConnect_API.Models.DTOs
{
    public class AlunoTurmaDTO
    {
        public int AlunoId { get; set; }

        public int UsuarioId { get; set; } // 🔥 NOVO CAMPO

        public string Nome { get; set; } = "";
        public string Email { get; set; } = "";
        public MatriculaStatus Status { get; set; }
    }
}