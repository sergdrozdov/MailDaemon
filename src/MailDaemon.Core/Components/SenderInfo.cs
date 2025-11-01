using Newtonsoft.Json;

namespace MailDaemon.Core
{
    public class SenderInfo
    {
        public string SmtpUsername { get; set; } = string.Empty;

        /// <summary>
        /// Mail address of the sender. 
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Name of the sender.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}
