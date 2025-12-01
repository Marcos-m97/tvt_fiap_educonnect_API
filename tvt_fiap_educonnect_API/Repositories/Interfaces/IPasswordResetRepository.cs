using EduConnect_API.Models;

public interface IPasswordResetRepository
{
    Task Salvar(PasswordResetCode code);
    Task<PasswordResetCode?> Obter(string email, string codigo);
    Task Atualizar(PasswordResetCode code);
}