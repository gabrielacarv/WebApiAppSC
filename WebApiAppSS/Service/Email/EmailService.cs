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
            message.Body = new TextPart("html")
            {
                Text = $@"
                <html>
                    <body style='background-color: #f2f2f2; font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                            <h2 style='color: #f2601d; text-align: center;'>Recuperação de Senha</h2>
                            <p>Você solicitou a recuperação de senha para sua conta.</p>
                            <p>Copie e cole o token abaixo no aplicativo para redefinir sua senha:</p>
                            <div style='text-align: center; margin: 20px 0;'>
                                <div style='display: inline-block; padding: 10px 20px; font-size: 16px; color: #ffffff; background-color: #f2601d; border-radius: 5px; word-break: break-all;'>
                                    {resetLink}
                                </div>
                            </div>
                            <p>Se não foi você, ignore este e-mail e sua senha permanecerá inalterada.</p>
                            <p style='text-align: center; color: #888888;'>© 2024 Sorte Cacau</p>
                        </div>
                    </body>
                </html>"
            };


            using var client = new SmtpClient();
            await client.ConnectAsync(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(GmailUsername, GmailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
