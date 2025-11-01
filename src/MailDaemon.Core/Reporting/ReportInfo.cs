using System.Collections.Generic;

namespace MailDaemon.Core.Reporting
{
    public class ReportInfo
    {
        public string Title { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        public List<string> Content { get; set; } = new();
        public int RecipientsCount { get; set; }
        public int SentMailCount { get; set; }
        public int SkippedMailCount { get; set; }
        public int ErrorsMailCount { get; set; }
        public string Body { get; set; } = string.Empty;
    }
}
