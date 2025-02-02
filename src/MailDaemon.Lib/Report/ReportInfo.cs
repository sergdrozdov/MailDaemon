﻿using System.Collections.Generic;

namespace MailDaemon.Core.Report
{
    public class ReportInfo
    {
        public string Title { get; set; }
        public string FileName { get; set; }
        public ReportType ReportType { get; set; }
        public List<string> Content { get; set; }
    }
}
