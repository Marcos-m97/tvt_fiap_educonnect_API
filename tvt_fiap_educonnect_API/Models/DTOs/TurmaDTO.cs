namespace EduConnect_API.Models.DTOs
{
    public class TurmaDTO
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Periodo { get; set; } = string.Empty;

        public string Semestre { get; set; } = string.Empty;

        // 🔗 Relacionamento com Curso
        public int CursoId { get; set; }
        public string CursoNome { get; set; } = string.Empty;

        // 🔥 SOFT DELETE
        public bool Ativo { get; set; }
    }
}