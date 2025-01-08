namespace MailDaemon.Core
{
    public class SettingsInfo
    {
        public string AppDirectory { get; set; }
        public string MailProfilesDirectory { get; set; } = "MailProfiles";
        public string MailTemplatesDirectory { get; set; } = "MailTemplates";
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpEnableSSL { get; set; }
        
        /// <summary>
        /// Profile file name.
        /// </summary>
        public string MailProfile { get; set; }
        
        public SenderInfo Operator { get; set; }
    }
}
