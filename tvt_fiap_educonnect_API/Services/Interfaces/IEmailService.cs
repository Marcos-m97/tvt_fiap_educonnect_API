namespace EduConnect_API.Services.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmail(string para, string assunto, string corpo);
    }
}