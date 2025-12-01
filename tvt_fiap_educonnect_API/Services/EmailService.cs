using System.Net;
using System.Net.Mail;
using EduConnect_API.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EduConnect_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarEmail(string para, string assunto, string corpo)
        {
            var smtpHost = _config["EmailSettings:Smtp"];
            var smtpPort = int.Parse(_config["EmailSettings:Port"]);
            var emailOrigem = _config["EmailSettings:Email"];
            var senhaApp = _config["EmailSettings:AppPassword"];

            var mensagem = new MailMessage(emailOrigem, para)
            {
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = false // coloque true se quiser formatar com HTML
            };

            var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(emailOrigem, senhaApp),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mensagem);
        }
    }
}
