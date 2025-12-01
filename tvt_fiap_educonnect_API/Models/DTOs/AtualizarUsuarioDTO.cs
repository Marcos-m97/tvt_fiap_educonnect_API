using System.ComponentModel.DataAnnotations;

namespace EduConnect_API.Models.DTOs
{
    public class AtualizarUsuarioDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo do usuário é obrigatório.")]
        [Range(0, 3, ErrorMessage = "Tipo inválido. Valores permitidos: 0, 1, 2, 3.")]
        public int Tipo { get; set; }
    }
}
