using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Training.Configuration.Environments;

namespace Training.Configuration
{
    public static class Settings
    {
        public static BuildConfiguration? CurrentBuildConfiguration { get; set; }

        private static ISettings _current;

        public static ISettings Current => _current ?? (_current = GetSettings());

        private static ISettings GetSettings()
        {
            if (!CurrentBuildConfiguration.HasValue)
            {
                try
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();

                    if (!Enum.TryParse(configuration["BuildConfiguration"], out BuildConfiguration buildConfiguration))
                    {
                        return null;
                    }

                    CurrentBuildConfiguration = buildConfiguration;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            switch (CurrentBuildConfiguration.Value)
            {
                case BuildConfiguration.Development:
                    return new SettingsDev();
                
                case BuildConfiguration.Production:
                    return new SettingsProduction();

                default:
                    return new SettingsDev();
            }
        }
    }
}
