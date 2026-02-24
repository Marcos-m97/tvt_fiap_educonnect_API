using EduConnect_API.Models;

public class Disciplina
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    public int CargaHoraria { get; set; }

    public int CursoId { get; set; }
    public Curso Curso { get; set; }

    public bool Ativo { get; set; } = true;
}