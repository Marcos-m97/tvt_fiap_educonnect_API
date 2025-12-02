using EduConnect_API.Models;
namespace EduConnect_API.Repositories.Interfaces { 

    public interface IPasswordResetRepository
    {
        Task Salvar(PasswordResetCode code);
        Task<PasswordResetCode?> Obter(string email, string codigo);
        Task Atualizar(PasswordResetCode code);
    } 
}