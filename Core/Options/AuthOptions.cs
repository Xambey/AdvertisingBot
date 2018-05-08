using Newtonsoft.Json;

namespace Core.Options
{
    public class AuthOptions
    {
        /// <summary>
        /// OAuth key for authorization on Twitch.Tv
        /// </summary>
        [JsonProperty("OAuth")]
        public string OAuth { get; set; }

        /// <summary>
        /// Login for IRC connection
        /// </summary>
        [JsonProperty("Login")]
        public string Login { get; set; }
        
        /// <summary>
        /// Client-ID of application in the twitch.devs
        /// </summary>
        [JsonProperty("Client-ID")]
        public string ClientId { get; set; }
        
        /// <summary>
        /// Secret ClientId of application in the twitch.devs
        /// </summary>
        [JsonProperty("Secret-Client-ID")]
        public string SecretClientId { get; set; }

        /// <summary>
        /// Host of twitch.tv web server
        /// </summary>
        [JsonProperty("WebHost")]
        public string WebHost { get; set; } = "https://id.twitch.tv";

        /// <summary>
        /// Host of twitch.tv for irc connection to server
        /// </summary>
        [JsonProperty("IrcHost")] 
        public string IrcHost { get; set; } = "irc.chat.twitch.tv";

        /// <summary>
        /// Port for connection
        /// </summary>
        [JsonProperty("Port")]
        public int Port { get; set; } = 80;
    }
}