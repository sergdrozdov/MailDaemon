using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace MailDaemon.Core
{
    public class MailMessageService : IMailMessageService
    {
        public string FormatSubject(MailProfile mailProfile, RecipientInfo recipientInfo)
        {
            var subject = !string.IsNullOrEmpty(recipientInfo.Subject) ? recipientInfo.Subject : mailProfile.Subject;
            if (recipientInfo.Replace != null)
            {
                foreach (var replaceData in recipientInfo.Replace)
                {
                    subject = subject.Replace(replaceData.Key, replaceData.Value, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            //subject = subject
            //	.Replace("{PERSON_NAME}", recipientInfo.Name)
            //	.Replace("{COMPANY_NAME}", recipientInfo.Company);

            return subject;
        }

        public string FormatMessageBody(MailProfile mailProfile, RecipientInfo recipientInfo)
        {
            var body = !string.IsNullOrEmpty(recipientInfo.MailBody) ? recipientInfo.MailBody : mailProfile.MailBody;
            //body = body
            //	.Replace("{PERSON_NAME}", recipientInfo.Name)
            //	.Replace("{COMPANY_NAME}", recipientInfo.Company)
            //             .Replace("{CONTACT_PERSON}", recipientInfo.ContactPerson);

            // recipient's replacement dictionary has higher priority
            if (recipientInfo.Replace != null)
            {
                foreach (var replaceData in recipientInfo.Replace)
                {
                    body = body.Replace(replaceData.Key, replaceData.Value, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            if (mailProfile.Replace != null)
            {
                foreach (var replaceData in mailProfile.Replace)
                {
                    body = body.Replace(replaceData.Key, replaceData.Value, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            return body;
        }

        public MailMessage GenerateMailMessage(SenderInfo operatorInfo, MailProfile mailProfile, RecipientInfo recipientInfo, bool sendDemo = false)
        {
            var mailMessage = new MailMessage();

            if (sendDemo)
            {
                // send as demo to sender
                mailMessage.From = GetMailAddress(operatorInfo.SmtpUsername, operatorInfo.Name);
                mailMessage.To.Add(GetMailAddress(operatorInfo.Address, operatorInfo.Name));
                mailMessage.ReplyToList.Add(new MailAddress(mailProfile.Sender.Address, mailProfile.Sender.Name));
            }
            else
            {
                // send to recipient
                mailMessage.From = GetMailAddress(operatorInfo.SmtpUsername, operatorInfo.Name);
                mailMessage.To.Add(GetMailAddress(recipientInfo.Address, recipientInfo.Name));
                mailMessage.ReplyToList.Add(new MailAddress(mailProfile.Sender.Address, mailProfile.Sender.Name));
            }

            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.Subject = FormatSubject(mailProfile, recipientInfo);
            if (sendDemo)
                mailMessage.Subject += " [DEMO MAIL]";

            if (string.Equals(Path.GetExtension(recipientInfo.MailBodyTemplateFileName).ToLower(), ".html", StringComparison.InvariantCultureIgnoreCase))
                mailMessage.IsBodyHtml = true;

            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.Body = FormatMessageBody(mailProfile, recipientInfo);

            // recipient related attachments use at first
            if (recipientInfo.Attachments != null)
            {
                foreach (var attachment in recipientInfo.Attachments)
                {
                    if (File.Exists(attachment.Path))
                    {
                        var fileStream = new StreamReader(attachment.Path);
                        if (!string.IsNullOrEmpty(attachment.FileName))
                            mailMessage.Attachments.Add(new Attachment(fileStream.BaseStream, attachment.FileName, Helper.GetMediaType(attachment.Path)));
                        else
                            mailMessage.Attachments.Add(new Attachment(fileStream.BaseStream, Path.GetFileName(attachment.Path), Helper.GetMediaType(attachment.Path)));
                    }
                }
            }

            // mail profile attachments
            if (mailProfile.Attachments != null)
            {
                foreach (var attachment in mailProfile.Attachments)
                {
                    if (!File.Exists(attachment.Path))
                        continue;
                    var fileStream = new StreamReader(attachment.Path);
                    if (!string.IsNullOrEmpty(attachment.FileName))
                        mailMessage.Attachments.Add(new Attachment(fileStream.BaseStream, attachment.FileName, Helper.GetMediaType(attachment.Path)));
                    else
                        mailMessage.Attachments.Add(new Attachment(fileStream.BaseStream, Path.GetFileName(attachment.Path), Helper.GetMediaType(attachment.Path)));
                }
            }

            return mailMessage;
        }

        public MailAddress GetMailAddress(string address, string name = "")
        {
            if (string.IsNullOrEmpty(address) && string.IsNullOrEmpty(name))
                throw new ArgumentException("Address and name both empty.");
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Address is empty.");

            return !string.IsNullOrEmpty(name) ? new MailAddress(address, name) : new MailAddress(address);
        }
    }
}
