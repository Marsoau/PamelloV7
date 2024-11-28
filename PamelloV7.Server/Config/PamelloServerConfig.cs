using PamelloV7.Core.Config;
using System.ComponentModel;
using System.Reflection;

namespace PamelloV7.Server.Config
{
    public static class PamelloServerConfig
    {
        public static string MainBotToken;
        public static string[] SpeakerTokens;

        private static IConfigurationRoot config;

        static PamelloServerConfig() {
            config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Config/config.json")
                .Build();

            LoadValues();
        }

        public static void LoadValues() {
            var fields = typeof(PamelloServerConfig).GetFields();
            foreach (var field in fields) {
                if (field.IsPublic) {
                    LoadField(field);
                }
            }
        }
        private static void LoadField(FieldInfo field) {
            var section = config.GetSection(field.Name);

            var configValue = section.Get(field.FieldType);
            field.SetValue(null, configValue);

            Console.WriteLine($"Loaded config field \"{field.Name}\": {configValue}");
        }
    }
}
