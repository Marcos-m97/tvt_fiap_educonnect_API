namespace EduConnect_API.Models.DTOs
{
    /// <summary>
    /// DTO utilizado para criar ou atualizar os dados específicos de aluno.
    ///
    /// Esse objeto recebe os dados complementares do aluno e o ID do usuário
    /// que será vinculado ao perfil acadêmico.
    ///
    /// A criação do aluno depende de um usuário já existente com Tipo = 3,
    /// garantindo que apenas usuários com perfil de aluno recebam esse cadastro.
    /// </summary>
    public class CriarAlunoDTO
    {
        /// <summary>
        /// Identificador do usuário que será vinculado ao aluno.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// CPF do aluno.
        /// </summary>
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// Data de nascimento do aluno.
        /// </summary>
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// Endereço do aluno.
        /// </summary>
        public string? Endereco { get; set; }
    }
}