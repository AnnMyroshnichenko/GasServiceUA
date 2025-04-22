using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

public class EmailService:IEmailService
{
    private readonly EmailSettings _mailSettings;
    public EmailService(IOptions<EmailSettings> mailSettingsOptions)
    {
        _mailSettings = mailSettingsOptions.Value;
    }

    public bool SendEmail(EmailData mailData)
    {
        try
        {
            using (MimeMessage emailMessage = new MimeMessage())
            {
                Console.WriteLine(_mailSettings.SenderName + " " + _mailSettings.SenderEmail + " " + mailData.EmailToName + " " + mailData.EmailToId);
                MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress(mailData.EmailToName, mailData.EmailToId);
                emailMessage.To.Add(emailTo);
                emailMessage.Subject = mailData.EmailSubject;

                var emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = mailData.EmailBody;
                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                using (SmtpClient mailClient = new SmtpClient())
                {
                    mailClient.Connect(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    mailClient.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                    mailClient.Send(emailMessage);
                    mailClient.Disconnect(true);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}