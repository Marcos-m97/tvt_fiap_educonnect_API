using System.Net;
using System.Net.Mail;
using System.Text;
using EduConnect_API.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável pelo envio de e-mails da aplicação EduConnect.
    ///
    /// No sistema, esse serviço é utilizado em fluxos como:
    /// - recuperação de senha;
    /// - confirmação de cadastro;
    /// - criação de acesso administrativo;
    /// - criação de perfil de professor;
    /// - confirmação de matrícula efetivada.
    ///
    /// A centralização do envio de e-mails em um serviço evita duplicação
    /// de configuração SMTP em outras partes do sistema.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Recebe as configurações da aplicação por injeção de dependência.
        ///
        /// As configurações SMTP são lidas da seção EmailSettings do appsettings,
        /// incluindo host, porta, e-mail de origem e senha de aplicativo.
        /// </summary>
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // ============================================================
        // 1. ENVIAR E-MAIL
        // ============================================================

        /// <summary>
        /// Envia um e-mail para o destinatário informado.
        ///
        /// O método recebe destinatário, assunto, corpo da mensagem e uma flag
        /// indicando se o corpo deve ser interpretado como HTML.
        ///
        /// Esse método é genérico para permitir reutilização em diferentes fluxos
        /// da aplicação, tanto com mensagens simples em texto quanto com templates HTML.
        /// </summary>
        public async Task EnviarEmail(
            string para,
            string assunto,
            string corpo,
            bool isHtml = false
        )
        {
            var smtpHost = _config["EmailSettings:Smtp"];
            var smtpPort = int.Parse(_config["EmailSettings:Port"]);
            var emailOrigem = _config["EmailSettings:Email"];
            var senhaApp = _config["EmailSettings:AppPassword"];

            var mensagem = new MailMessage(emailOrigem, para)
            {
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = isHtml,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
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