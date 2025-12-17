public class AulaAlunoDTO
{
    public Guid AulaId { get; set; }
    public string Titulo { get; set; }
    public string? Descricao { get; set; }

    public Guid DisciplinaId { get; set; }
    public string NomeDisciplina { get; set; }

    public string? UrlVideo { get; set; }
    public bool TemMaterialApoio { get; set; }
}