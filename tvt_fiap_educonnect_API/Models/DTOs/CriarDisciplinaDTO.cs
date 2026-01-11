namespace EduConnect_API.Models.DTOs
{
    public class CriarDisciplinaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int CargaHoraria { get; set; }
        public int CursoId { get; set; } // disciplina sempre precisa de um curso
    }
}