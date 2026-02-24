namespace EduConnect_API.Models.DTOs
{
    public class DisciplinaDTO
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public int CargaHoraria { get; set; }

        // Relacionamento com Curso
        public int CursoId { get; set; }
        public string CursoNome { get; set; } = string.Empty;

        // 🔥 SOFT DELETE
        public bool Ativo { get; set; }
    }
}