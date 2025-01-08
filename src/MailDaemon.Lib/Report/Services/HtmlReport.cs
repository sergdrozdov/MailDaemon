using System;

namespace MailDaemon.Core.Report
{
    public class HtmlReport : IReport, IReportStorage
    {
        public string Generate()
        {
            throw new NotImplementedException();
        }

        public void Save(ReportInfo reportInfo)
        {
            throw new NotImplementedException();
        }
    }
}
