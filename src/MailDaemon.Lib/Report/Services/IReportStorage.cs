using System;
using System.Collections.Generic;
using System.Text;

namespace MailDaemon.Core.Report
{
    public interface IReportStorage
    {
        void Save(ReportInfo reportInfo);
    }
}
