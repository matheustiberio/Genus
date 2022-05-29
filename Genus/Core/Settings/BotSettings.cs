using GenusBot.Core.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace GenusBot.Core.Settings
{
    public class BotSettings
    {
        [JsonProperty]
        public string Token { get; private set; } = string.Empty;

        [JsonProperty]
        public string CommandPrefix { get; private set; } = string.Empty;

        [JsonProperty]
        public string LavalinkHostname { get; private set; } = string.Empty;

        [JsonProperty]
        public string LavalinkPort { get; private set; } = string.Empty;

        [JsonProperty]
        public string LavalinkAuthorization { get; private set; } = string.Empty;

        [JsonProperty]
        public bool IsSSLLavalinkHost { get; private set; } = true;

        public static string FileName { get; } = "botsettings.json";

        public async Task<BotSettings?> LoadAsync()
        {
            try
            {
                LoggingService.Log(GetType(), LogLevel.Information, "Loading settings.");

                string filePath = Path.Combine(AppContext.BaseDirectory, FileName);

                if (File.Exists(filePath))
                    return JsonConvert.DeserializeObject<BotSettings>(File.ReadAllText(filePath));

                LoggingService.Log(GetType(), LogLevel.Warning, "The file does not exists. Creating a new one.");
                
                var settings = new BotSettings();
                
                var stream = File.Create(filePath);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(settings)));

                await stream.DisposeAsync();

                LoggingService.Log(GetType(), LogLevel.Information, "File created successfully.");
                
                LoggingService.Log(GetType(), LogLevel.Warning, "You need to provide valid settings to initialize the bot.");

                return settings;
            }
            catch (Exception ex)
            {
                LoggingService.LogCritical(GetType(), "Unhandled error at loading bot settings.", ex);
                return null;
            }
        }

        public static bool IsValid(BotSettings settings)
        {
            if (settings == null)
                return false;

            var properties = settings.GetType().GetProperties();

            var areStringPropertiesValid = properties
                .Where(propertyInfo => propertyInfo.GetValue(settings) is string)
                .All(p => !string.IsNullOrEmpty(p.GetValue(settings) as string));

            return areStringPropertiesValid;
        }
    }
}
