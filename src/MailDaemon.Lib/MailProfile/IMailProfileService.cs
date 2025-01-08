using MailDaemon.Lib;
using System.Collections.Generic;

namespace MailDaemon.Core
{
    public interface IMailProfileService
    {
        MailProfile ReadProfile(string filePath);
        string ReadMailBodyTemplate(string filePath);

        List<ValidationInfo> ValidateMailProfile(MailProfile mailProfile);
    }
}
