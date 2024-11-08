using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Reflection;

namespace PamelloV7.Core.Config
{
    public abstract class PamelloConfig
    {
        private IConfigurationRoot config;

        protected PamelloConfig(string path) {
            config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddYamlFile(path)
                .Build();

            LoadValues();
        }

        public void LoadValues() {
            var fields = GetType().GetFields();
            foreach (var field in fields) {
                if (!field.IsPrivate) {
                    LoadField(field);
                }
            }
        }
        private void LoadField(FieldInfo field) {
            var section = config.GetSection(field.Name);
            var converter = TypeDescriptor.GetConverter(field.FieldType);

            if (section.Value is null) throw new Exception($"Cant find \"{field.Name}\" field in config");

            var configValue = converter.ConvertFromString(section.Value);

            field.SetValue(this, configValue);

            Console.WriteLine($"Loaded config field \"{field.Name}\": {configValue}");
        }
    }
}
