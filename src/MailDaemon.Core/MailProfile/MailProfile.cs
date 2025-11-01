using System.Collections.Generic;
using Newtonsoft.Json;

namespace MailDaemon.Core
{
    public class MailProfile
    {
        [JsonIgnore]
        public string FileName { get; set; } = string.Empty;

        [JsonProperty("operator")]
        public SenderInfo Operator { get; set; } = new();

        [JsonProperty("sender")]
        public SenderInfo Sender { get; set; } = new();

        [JsonProperty("recipients")]
        public List<RecipientInfo> Recipients { get; set; } = new();

        [JsonProperty("subject")]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Mail template file name.
        /// </summary>
        [JsonProperty("template")]
        public string MailBodyTemplateFileName { get; set; } = string.Empty;

        /// <summary>
        /// Path to mail template file.
        /// </summary>
        public string MailBodyTemplateFullPath { get; set; } = string.Empty;

        /// <summary>
        /// Template content.
        /// </summary>
        [JsonIgnore]
        public string MailBody { get; set; } = string.Empty;

        [JsonProperty("attachments")]
        public List<AttachmentInfo> Attachments { get; set; } = new();

        /// <summary>
        /// List of data to replace text in the mail body template.
        /// </summary>
        [JsonProperty("replace")]
        public Dictionary<string, string> Replace { get; set; } = new();
    }
}
