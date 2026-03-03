namespace EduConnect_API.Models.DTOs
{
    public class MatriculaDTO
    {
        public int Id { get; set; }

        public int AlunoId { get; set; }

        // 🔥 ESSENCIAL PARA FOTO DO USUÁRIO
        public int UsuarioId { get; set; }

        public string AlunoNome { get; set; } = string.Empty;

        public int TurmaId { get; set; }
        public string TurmaNome { get; set; } = string.Empty;

        public MatriculaStatus Status { get; set; }

        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}