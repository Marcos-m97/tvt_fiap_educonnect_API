public class AulaAlunoDTO
{
    public int AulaId { get; set; }
    public string Titulo { get; set; }
    public string? Descricao { get; set; }

    public int DisciplinaId { get; set; }
    public string NomeDisciplina { get; set; }

    public string? UrlVideo { get; set; }
    public bool TemMaterialApoio { get; set; }
}