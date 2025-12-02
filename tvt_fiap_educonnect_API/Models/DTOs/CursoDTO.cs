namespace EduConnect_API.Models.DTOs
{
        
    public class CursoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int CargaHoraria { get; set; }
    }


}
