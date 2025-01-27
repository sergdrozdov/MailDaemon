using System;
using System.Collections.Generic;
using System.Text;

namespace MailDaemon.Core
{
    public interface IMailProfileBuilderService
    {
        MailProfile Build();
        void SaveToFile(string filePath);
    }
}
