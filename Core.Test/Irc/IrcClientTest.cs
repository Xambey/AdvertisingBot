using System.Threading.Tasks;
using Core.Irc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;

namespace Core.Test.Irc
{
    /// <summary>
    /// Test for Irc.IrcClient
    /// </summary>
    public class IrcClientTest : BaseTest
    {
        public IrcClientSingelton.IrcClient IrcClient { get; set; }
        public IrcClientTest()
        {
            //TODO: Do mock class for emulation Twitch IRC Server    
            IrcClientSingelton.Generate(AuthOptions);
            IrcClient = IrcClientSingelton.Instance;
        }
        
        [Fact]
        public async Task JoinRoomTest()
        {
            await IrcClient.JoinRoom("dudelka_krasnaya");
        }    

        [Fact]
        public async Task PartRoomTest()
        {
            await JoinRoomTest();
            await IrcClient.PartRoom("dudelka_krasnaya");
        }

        [Fact]
        public async Task AuhtorizeTest()
        {
            await IrcClient.Authorize();
        }

        [Fact]
        public async Task ReconnectTest()
        {
            await IrcClient.Reconnect();
        }

        [Fact]
        public async Task PrivmsgTest()
        {
            await IrcClient.Privmsg("dudelka_krasnaya", "test broadcast message");
            await Task.Delay(1500);
        }
        
        [Fact]
        public async Task PrivmsgWithGetterTest()
        {
            await IrcClient.Privmsg("dudelka_krasnaya", "test message", "Dudelka_Krasnaya");
        }

        [Fact]
        public async Task ReadMessageTest()
        {
            await IrcClient.Authorize();
            var message = await IrcClient.ReadMessage();
            Assert.NotEmpty(message);
        }
        
    }
}