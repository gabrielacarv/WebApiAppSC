using MailKit.Net.Smtp;
using MimeKit;

namespace WebApiAppSS.Service.Email
{
    public class EmailService
    {
        private const string SmtpServer = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const string GmailUsername = "sortecacau@gmail.com";
        private const string GmailPassword = "sfsm vbgx wgse qrjc";

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sorte Cacau", GmailUsername));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Recuperação de Senha";

            string resetLink = $"{resetToken}";
            message.Body = new TextPart("plain")
            {
                Text = $"Você está recebendo isso porque você (ou alguém) solicitou a recuperação da senha para sua conta.\n\n" +
                       $"Por favor, clique no link a seguir ou cole no seu navegador para completar o processo:\n\n" +
                       $"{resetLink}\n\n" +
                       "Se você não solicitou isso, por favor ignore este email e sua senha permanecerá a mesma.\n"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(GmailUsername, GmailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
