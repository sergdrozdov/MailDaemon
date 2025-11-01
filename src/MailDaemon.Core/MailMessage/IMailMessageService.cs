using System.Net.Mail;

namespace MailDaemon.Core
{
    public interface IMailMessageService
    {
        string FormatSubject(MailProfile mailProfile, RecipientInfo recipientInfo);
        string FormatMessageBody(MailProfile mailProfile, RecipientInfo recipientInfo);
        MailMessage GenerateMailMessage(SenderInfo operatorInfo, MailProfile mailProfile, RecipientInfo recipientInfo, bool sendDemo = false);
    }
}
