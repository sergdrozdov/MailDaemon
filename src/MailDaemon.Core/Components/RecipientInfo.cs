using System.Collections.Generic;
using Newtonsoft.Json;

namespace MailDaemon.Core
{
    public class RecipientInfo
    {
        /// <summary>
        /// Email address of the recipient.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Name of the recipient.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Custom subject for this recipient.
        /// </summary>
        [JsonProperty("subject")]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Custom mail template file name for this recipient.
        /// Overrides the mail profile template if set.
        /// </summary>
        [JsonProperty("template")]
        public string MailBodyTemplateFileName { get; set; } = string.Empty;

        /// <summary>
        /// Path to mail template file.
        /// </summary>
        public string MailBodyTemplateFullPath { get; set; } = string.Empty;

        public string MailBody { get; set; } = string.Empty;

        /// <summary>
        /// List of data to replace text in the mail body template.
        /// </summary>
        [JsonProperty("replace")]
        public Dictionary<string,string> Replace { get; set; } = new();

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("attachments")]
        public List<AttachmentInfo> Attachments { get; set; } = new();

        /// <summary>
        /// If true, the mail is not sending to the recipient.
        /// </summary>
        [JsonProperty("skip")]
        public bool? Skip { get; set; }
    }
}
