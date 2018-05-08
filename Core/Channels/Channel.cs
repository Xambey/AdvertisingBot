using Core.Options;

namespace Core.Channels
{
    public class Channel
    {
        /// <summary>
        /// Name of twitch channel
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Token for manage channel
        /// </summary>
        public string Token { get; set; }

        public Channel(ChannelOptions options)
        {
            Name = options.Name;
            Token = options.Token;
        }
    }
}