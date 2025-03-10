using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AppUsuarios.Services
{
    public interface IEmailService
    {
        Task EnviarCorreo(string receptor, string asunto, string mensaje);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task EnviarCorreo(string receptor, string asunto, string mensaje)
        {
            var email = configuration.GetValue<string>("EmailSettings:Email");
            var password = configuration.GetValue<string>("EmailSettings:Password");
            var host = configuration.GetValue<string>("EmailSettings:Host");
            var port = configuration.GetValue<int>("EmailSettings:Port");

            var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email, password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(email),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true
            };

            message.To.Add(receptor);

            await smtpClient.SendMailAsync(message);
        }
    }
}
