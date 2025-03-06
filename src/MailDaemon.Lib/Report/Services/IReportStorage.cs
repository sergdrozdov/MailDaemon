namespace MailDaemon.Core.Report
{
    public interface IReportStorage
    {
        void Save(ReportInfo reportInfo);
    }
}
