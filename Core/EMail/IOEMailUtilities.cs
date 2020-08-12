using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IOBootstrap.NET.Core.EMail
{
    public class IOEMailUtilities
    {

        private string ApiKey;
        private string FromName;
        private string FromEmail;

        private SendGridMessage GeneratedMessage;

        public IOEMailUtilities(string apiKey, string fromName, string fromEmail) 
        {
            this.ApiKey = apiKey;
            this.FromName = fromName;
            this.FromEmail = fromEmail;
        }

        public void CreateEMailMessage(string toEmail, string toName, string subject, string htmlBody, string textBody)
        {
            // Create email message
            SendGridMessage msg = new SendGridMessage();

            // Set from
            msg.SetFrom(new EmailAddress(FromEmail, FromName));

            // Set recipients
            var recipients = new List<EmailAddress>
            {
                new EmailAddress(toEmail, toName)
            };
            msg.AddTos(recipients);

            // Set subject and content
            msg.SetSubject(subject);
            msg.AddContent(MimeType.Text, textBody);
            msg.AddContent(MimeType.Html, htmlBody);

            GeneratedMessage = msg;
        }

        public HttpStatusCode SendEmail()
        {
            SendGridClient client = new SendGridClient(ApiKey);
            Task<Response> response = client.SendEmailAsync(GeneratedMessage);
            response.Wait();

            return response.Result.StatusCode;
        }
    }
}
