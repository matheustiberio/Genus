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

        public async static Task<BotSettings> LoadAsync()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, FileName);

            var settings = new BotSettings();

            if (!File.Exists(filePath))
            {
                var stream = File.Create(filePath);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(settings)));

                await stream.DisposeAsync();
            }
            else
            {
                settings = JsonConvert.DeserializeObject<BotSettings>(File.ReadAllText(filePath));
            }

            return settings ?? new BotSettings();
        }

        public static bool IsValid(BotSettings settings)
        {
            if (settings == null)
                return false;

            var properties = settings.GetType().GetProperties();

            var areStringPropertiesValid = properties
                .Where(propertyInfo => propertyInfo.GetValue(settings) is string)
                .All(p => !string.IsNullOrWhiteSpace(p.GetValue(settings) as string));

            return areStringPropertiesValid;
        }
    }
}
