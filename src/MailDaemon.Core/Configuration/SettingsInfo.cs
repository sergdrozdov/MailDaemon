namespace MailDaemon.Core
{
    public class SettingsInfo
    {
        public string AppDirectory { get; set; }
        public string MailProfilesDirectory { get; set; } = "MailProfiles";
        public string MailTemplatesDirectory { get; set; } = "MailTemplates";
        public string AttachmentsDirectory { get; set; } = "Attachments";
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool SmtpEnableSSL { get; set; }
        
        /// <summary>
        /// Profile file name.
        /// </summary>
        public string MailProfileFileName { get; set; } = string.Empty;

        public string MailProfileFullPath { get; set; } = string.Empty;

        /// <summary>
        /// The operator is the contact who sends mail messages.
        /// </summary>
        public SenderInfo Operator { get; set; } = new();
    }
}
