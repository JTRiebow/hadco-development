using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.Utilities
{
    public class Email
    {
        public static void SendMail(EmailInformation message)
        {
            using (SmtpClient emailClient = new SmtpClient())
            {
                MailAddress toAddress = new MailAddress(message.RecipientEmailAddress, message.RecipientFirstName + " " + message.RecipientLastName);
                MailAddress fromAddress;
                if (string.IsNullOrEmpty(message.FromEmail))
                {
                    fromAddress = new MailAddress((ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection).From);
                }
                else
                {
                    fromAddress = new MailAddress(message.FromEmail, message.FromName);
                }

                MailMessage mail = new MailMessage(fromAddress, toAddress);

                mail.Subject = message.Subject;
                mail.Body = message.HtmlMessage;
                mail.IsBodyHtml = true;

                emailClient.Send(mail);
            }

        }
    }
}
