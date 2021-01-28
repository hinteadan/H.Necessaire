using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace H.Necessaire.Notification
{
    public class SmtpNotifier : INotify
    {
        #region Construct
        readonly string smtpServer = "smtp.sendgrid.net";
        readonly int smtpPort = 587;//TLS
        readonly string smtpUser = "apikey";
        readonly string smtpPass = "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";
        readonly TimeSpan smtpSendTimeout = TimeSpan.FromSeconds(30);
        readonly bool isSslDisabled = false;
        readonly bool hasCredentials = true;

        public SmtpNotifier(SmtpNotifierConfiguration configuration)
        {
            this.smtpServer = configuration.ServerUrl;
            this.smtpPort = configuration.Port;
            this.smtpUser = configuration.Username;
            this.smtpPass = configuration.Password;
            this.smtpSendTimeout = configuration.SendTimeout;
            this.isSslDisabled = configuration.IsSslDisabled;
            this.hasCredentials = !string.IsNullOrEmpty(smtpUser) || !string.IsNullOrEmpty(smtpPass);
        }
        #endregion

        public async Task<OperationResult> Send(NotificationMessage message, NotificationAddress from, params NotificationEndpoint[] to)
        {
            if (!to?.Any() ?? true)
                return OperationResult.Win("No destinations were specified");

            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(async () =>
                {
                    using (SmtpClient smtpClient = ConstructSmtpClient())
                    using (MailMessage mailMessage = ConstructMailMessage(message, from, to))
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                })
                .TryOrFailWithGrace(
                    numberOfTimes: 1,
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }

        private MailMessage ConstructMailMessage(NotificationMessage message, NotificationAddress from, NotificationEndpoint[] to)
        {
            NotificationEndpoint[] actualTo = to.Where(x => !x.IsPrivate && !x.IsOptional).ToArray();
            NotificationEndpoint[] cc = to.Where(x => !x.IsPrivate && x.IsOptional).ToArray();
            NotificationEndpoint[] bcc = to.Where(x => x.IsPrivate).ToArray();

            return
                new MailMessage()
                .And(x => x.From = Map(from))
                .And(x => Array.ForEach(actualTo, a => x.To.Add(Map(a.Address))))
                .And(x => Array.ForEach(cc, a => x.CC.Add(Map(a.Address))))
                .And(x => Array.ForEach(bcc, a => x.Bcc.Add(Map(a.Address))))
                .And(x => x.Subject = message.Subject)
                .And(x => x.SubjectEncoding = message.Encoding)
                .And(x => x.Body = message.Content)
                .And(x => x.BodyEncoding = message.Encoding)
                .And(x => x.IsBodyHtml = message.ContentType.In(NotificationMessageContentType.Html))
                ;
        }

        private MailAddress Map(NotificationAddress notificationAddress)
        {
            return
                string.IsNullOrWhiteSpace(notificationAddress.Name)
                ? new MailAddress(notificationAddress.Address)
                : new MailAddress(notificationAddress.Address, notificationAddress.Name)
                ;
        }

        private SmtpClient ConstructSmtpClient()
        {
            SmtpClient smtpClient
                = new SmtpClient(smtpServer, smtpPort);

            smtpClient.EnableSsl = !isSslDisabled;
            smtpClient.DeliveryFormat = SmtpDeliveryFormat.International;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = !hasCredentials;
            if (hasCredentials)
                smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
            smtpClient.Timeout = (int)smtpSendTimeout.TotalMilliseconds;

            return smtpClient;
        }
    }
}
