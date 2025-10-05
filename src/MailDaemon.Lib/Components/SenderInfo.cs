using Newtonsoft.Json;

namespace MailDaemon.Core
{
    public class SenderInfo
    {
        public string SmtpUsername { get; set; }

        /// <summary>
        /// Mail address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
