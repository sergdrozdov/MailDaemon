namespace MailDaemon.Core
{
    public interface IValidator
    {
        bool IsMailAddressValid(string email);
    }
}
