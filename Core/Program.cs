using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Core.Irc;
using Core.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace Core
{
    class Program
    {   
        static async Task Main(string[] args)
        {   
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", "pt-applications")
                .WriteTo.Console()
                .WriteTo.RollingFile(Path.Combine(Environment.CurrentDirectory, "/Logs/Errors"), LogEventLevel.Error)
                .CreateLogger();
            
            var config = StartupConfig("appsettings.json");
            
            var core = new Core(config);
            
            IrcClientSingelton.Generate(config.Login, config.OAuth, config.IrcHost, config.Port);

            var client = IrcClientSingelton.Instance;
            await client.Authorize();
            await client.JoinRoom("dudelka_krasnaya");
            await client.Privmsg("dudelka_krasnaya", "test", "Dudelka_Krasnaya");

            await client.Reconnect();
            while (true)
            {
                var message = await client.ReadMessage();
            }
        }

        static void ShowConfiguration(AuthOptions config)
        {
            Log.Information("Startup configuration:");
            Log.Information($"IrcHost: {config.IrcHost}");
            Log.Information($"WebHost: {config.WebHost}");
            Log.Information($"Port: {config.Port}");
            Log.Information($"Login: {config.Login}");
            Log.Information($"OAuth: {config.OAuth}");
            Log.Information($"Client-ID: {config.ClientId}");
            Log.Information($"Secret-Client-ID: {config.SecretClientId}");
            Log.CloseAndFlush();
        }

        static AuthOptions StartupConfig(string fileName)
        {
            var path = Path.Combine(Environment.CurrentDirectory, fileName);
            if (!File.Exists(path))
                throw new Exception("Config not found");
            var file = File.ReadAllText(path);
            return JsonConvert.DeserializeObject(file, typeof(AuthOptions)) as AuthOptions;
        }
        
    }
}