using Newtonsoft.Json;

namespace MailDaemon.Core
{
    public class AttachmentInfo
    {
        /// <summary>
        /// File full path.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Custom filename for recipient.
        /// </summary>
        [JsonProperty("filename")]
        public string FileName { get; set; } = string.Empty;
    }
}
