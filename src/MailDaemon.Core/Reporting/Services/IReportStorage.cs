namespace MailDaemon.Core.Reporting
{
    public interface IReportStorage
    {
        void Save(ReportInfo reportInfo);
    }
}
