using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Serilog;

namespace Core.Irc
{
    /// <summary>
    /// Singelton of IrcClient
    /// </summary>
    public sealed class IrcClientSingelton : Singelton<IrcClientSingelton.IrcClient>
    {
        private IrcClientSingelton()
        {
        }
        /// <summary>
        /// Generate Irc Client for communication with Twitch
        /// </summary>
        /// <param name="userName">Bot name</param>
        /// <param name="oAuth">OAuth key</param>
        /// <param name="endPoint">End point</param>
        /// <param name="receiveTimeout">Timeout receive messages</param>
        public static void Generate(string userName, string oAuth, IPEndPoint endPoint, int receiveTimeout = 60)
        {
            Generate(userName, oAuth, endPoint.Address.ToString(), endPoint.Port, receiveTimeout);
        }

        /// <summary>
        /// Generate Irc Client for communication with Twitch
        /// </summary>
        /// <param name="userName">Bot name</param>
        /// <param name="oAuth">OAuth key</param>
        /// <param name="host">Host or IPV4 address</param>
        /// <param name="port">Port (default 80)</param>
        /// <param name="receiveTimeout">Receive timeout of messages</param>
        public static void Generate(string userName, string oAuth, string host, int port = 80, int receiveTimeout = 60)
        {
            Object = new IrcClient(userName, oAuth, host, port, receiveTimeout);
        }
        
        /// <summary>
        /// Irc client for communication with Twitch
        /// </summary>
        public class IrcClient
        {
            
            /// <summary>
            /// Auth Oauth key
            /// </summary>
            private string OAuth { get; }
            
            /// <summary>
            /// Host of server
            /// </summary>
            public string Host { get; }
            
            /// <summary>
            /// Port 
            /// </summary>
            private int Port { get; }
            
            /// <summary>
            /// Receive timeout of messages
            /// </summary>
            private int ReceiveTimeout { get; }
            
            /// <summary>
            /// Login/Username of bot
            /// </summary>
            public string UserName { get; set; }
    
            /// <summary>
            /// Stream for getting data
            /// </summary>
            private StreamReader InputStreamWriter { get; set; }
            
            /// <summary>
            /// Stream for sending data
            /// </summary>
            private StreamWriter OutputStreamWriter { get; set; }
    
            /// <summary>
            /// Tcp Client for connection with host server
            /// </summary>
            private TcpClient _tcpClient { get; set; }
            
            /// <summary>
            /// When Authorize running, then value == true
            /// </summary>
            private bool AutorizeActive { get; set; }
    
            /// <summary>
            /// Irc Client for communication with Twitch
            /// </summary>
            /// <param name="userName">Bot name</param>
            /// <param name="oAuth">OAuth key</param>
            /// <param name="endPoint">End point</param>
            public IrcClient(string userName, string oAuth, IPEndPoint endPoint) : this(userName, oAuth, endPoint.Address.ToString(), endPoint.Port)
            {
            }
    
            /// <summary>
            /// Irc Client for communication with Twitch
            /// </summary>
            /// <param name="userName">Bot name</param>
            /// <param name="oAuth">OAuth key</param>
            /// <param name="host">Host or IPV4 address</param>
            /// <param name="port">Port (default 80)</param>
            /// <param name="receiveTimeout">Receive timeout of messages</param>
            public IrcClient(string userName, string oAuth, string host, int port = 80, int receiveTimeout = 60)
            {
                OAuth = oAuth;
                Host = host;
                Port = port;
                UserName = userName;
                ReceiveTimeout = receiveTimeout;
    
                _tcpClient = new TcpClient();
                _tcpClient.ReceiveTimeout = ReceiveTimeout;
            }
    
            /// <summary>
            /// Enter to the channel room
            /// </summary>
            /// <param name="channelName"></param>
            /// <returns></returns>
            public async Task JoinRoom(string channelName)
            {
                if (OutputStreamWriter != null)
                {
                    await OutputStreamWriter?.WriteLineAsync($"JOIN #{channelName}");
                    Log.Information($"JOIN #{channelName}");
                }
                else
                {
                    Log.Warning($"Message 'JOIN #{channelName}' hasn't been sent");
                    await Authorize().ConfigureAwait(false);
                    await JoinRoom(channelName);
                }
            }
    
            /// <summary>
            /// Leave channel room
            /// </summary>
            /// <param name="channelName">Channel name</param>
            /// <returns></returns>
            public async Task PartRoom(string channelName)
            {
                if (OutputStreamWriter != null)
                {
                    await OutputStreamWriter.WriteLineAsync($"PART #{channelName}");
                    Log.Information($"PART #{channelName}");
                }
                else
                {
                    Log.Warning($"Message 'PART #{channelName}' hasn't been sent");
                    await Authorize().ConfigureAwait(false);
                    await PartRoom(channelName);
                }
            }
    
            /// <summary>
            /// Authorize bot
            /// </summary>
            /// <returns></returns>
            public async Task Authorize()
            {
                if (AutorizeActive)
                    return;
                
                AutorizeActive = true; // Running
                while (true)
                {
                    try
                    {
                        if(_tcpClient == null)
                            _tcpClient = new TcpClient(Host, Port);
                        if(!_tcpClient.Connected)
                            await _tcpClient.ConnectAsync(Host, Port);
    
                        InputStreamWriter = new StreamReader(_tcpClient.GetStream());
                        OutputStreamWriter = new StreamWriter(_tcpClient.GetStream());
                
                        OutputStreamWriter.WriteLine($"PASS oauth:{OAuth}");
                        OutputStreamWriter.WriteLine($"NICK {UserName}");
                        OutputStreamWriter.WriteLine("CAP REQ :twitch.tv/membership");
                        OutputStreamWriter.WriteLine("CAP REQ :twitch.tv/commands");
                        OutputStreamWriter.WriteLine("CAP REQ :twitch.tv/tags");
    
    
                        await OutputStreamWriter.FlushAsync();
    
                        Log.Information("Authorizing...");
                        Log.Information($"> PASS oauth:{OAuth}");
                        Log.Information($"> NICK {UserName}");
                        Log.Information("> CAP REQ :twitch.tv/membership");
                        Log.Information("> CAP REQ :twitch.tv/commands");
                        Log.Information("> CAP REQ :twitch.tv/tags");
    
                        break;
                    }
                    catch (SocketException exception)
                    {
                        Log.Error(exception, "Error connection...");
                        await Task.Delay(TimeSpan.FromMinutes(1));
                    }
                }

                AutorizeActive = false;
            }

            /// <summary>
            /// Reconnect inner Tcp Client to End point and authorize
            /// </summary>
            /// <returns></returns>
            public async Task Reconnect()
            {
                Log.Information("Reconnect...");
                _tcpClient.Dispose();
                _tcpClient = null;
                
                InputStreamWriter = null;
                OutputStreamWriter = null;
                //TODO: Подумать, что сделать с тем, что после перепоключения, бот не входит в каналы, где находился
                await Authorize();
            }
    
            /// <summary>
            /// Send message to chat
            /// </summary>
            /// <param name="channel">Channel name in lowcase</param>
            /// <param name="message">Text of message</param>
            /// <param name="getter">UserName of getter</param>
            /// <returns></returns>
            public async Task Privmsg(string channel, string message, string getter = null)
            {
                if (OutputStreamWriter != null)
                {
                    await OutputStreamWriter.WriteLineAsync(
                        $"PRIVMSG #{channel} :{(getter != null ? $"@{getter}, " : "")}{message}");
                    await OutputStreamWriter.FlushAsync();
    
                    Log.Information($"> PRIVMSG #{channel} :{(getter != null ? $"@{getter}, " : "")}{message}");
                }
                else
                {
                    Log.Warning(
                        $"Message 'PRIVMSG #{channel} :{(getter != null ? $"@{getter}, " : "")}{message}' hasn't been sent");
                    await Authorize().ConfigureAwait(false);
                    await Privmsg(channel, message, getter).ConfigureAwait(false);
                }
            }
    
            /// <summary>
            /// Read message from server
            /// </summary>
            /// <returns></returns>
            public async Task<string> ReadMessage()
            {
                try
                {
                    if (InputStreamWriter != null)
                    {
                        var message = await InputStreamWriter.ReadLineAsync();
                        Log.Information($"< {message}");
                        return message;
                    }

                    Log.Warning("Input stream is null... Try fix it");
                    await Authorize().ConfigureAwait(false);
                    return await ReadMessage().ConfigureAwait(false);
                }
                catch (InvalidCastException)
                {
                    Log.Warning("Read message aborted, invalid operation...");
                    Log.Warning("Try initialize connection...");
                    await Authorize().ConfigureAwait(false);
                    return await ReadMessage().ConfigureAwait(false);
                }
            }
        }
    }
}