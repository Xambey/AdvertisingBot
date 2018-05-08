using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core.Options
{
    public class ChannelOptions
    {
        /// <summary>
        /// Name of channel
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Token for manage channel
        /// </summary>
        [JsonProperty("Token")]
        public string Token { get; set; }
    }
}