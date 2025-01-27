using System;
using System.Collections.Generic;
using System.Text;

namespace MailDaemon.Core
{
    public class MailProfileBuilderService : IMailProfileBuilderService
    {
        public MailProfile Build()
        {
            return new MailProfile();
        }

        public void SaveToFile(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
