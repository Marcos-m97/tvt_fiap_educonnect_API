using System;

namespace EduConnect_API.Models.DTOs
{
    /// <summary>
    /// DTO utilizado para retornar dados de aluno ao frontend.
    ///
    /// No EduConnect, esse DTO combina informações da entidade Aluno
    /// com informações básicas da entidade Usuario, como nome e e-mail.
    ///
    /// Essa abordagem evita expor a entidade completa do banco e permite
    /// entregar ao frontend apenas os dados necessários para exibição.
    /// </summary>
    public class AlunoDTO
    {
        /// <summary>
        /// Identificador do aluno.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do usuário vinculado ao aluno.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nome do aluno, obtido a partir da entidade Usuario.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// E-mail do aluno, obtido a partir da entidade Usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// CPF cadastrado para o aluno.
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