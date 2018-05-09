using System;
using System.IO;
using Core.Options;
using Newtonsoft.Json;

namespace Core.Test
{
    /// <summary>
    /// Base class for testing
    /// </summary>
    public class BaseTest
    {
        protected AuthOptions AuthOptions { get; set; }
        
        public BaseTest()
        {
            AuthOptions = StartupConfig("appsettings.json");
        }
        
        private AuthOptions StartupConfig(string fileName)
        {
            var path = Path.Combine(Environment.CurrentDirectory, fileName);
            if (!File.Exists(path))
                throw new Exception("Config not found");
            var file = File.ReadAllText(path);
            return JsonConvert.DeserializeObject(file, typeof(AuthOptions)) as AuthOptions;
        }
    }
}