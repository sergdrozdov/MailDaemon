using System.Net.Mail;

namespace MailDaemon.Core
{
    /// <summary>
    /// Defines the contract for sending email messages.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for handling the process of sending email
    /// messages and returning the result of the operation. The behavior and capabilities may vary depending on the
    /// specific implementation.</remarks>
    public interface IMailAgent
    {
        MailSendResult Send(MailMessage mailMessage);
    }
}
