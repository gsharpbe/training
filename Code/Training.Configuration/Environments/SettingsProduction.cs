using System;

namespace Training.Configuration.Environments
{
    public class SettingsProduction : SettingsShared, ISettings
    {
        public BuildConfiguration BuildConfiguration => BuildConfiguration.Production;

        public string ConnectionString => Environment.GetEnvironmentVariable("SQLAZURECONNSTR_API_CONNECTION_STRING");
        public string DboConnectionString => Environment.GetEnvironmentVariable("SQLAZURECONNSTR_API_CONNECTION_STRING_DBO");

        public string MigrationsAssembly => "Training.Migrations.Production";

        // uri's
        public string SeqUrl => Environment.GetEnvironmentVariable("APPSETTING_API_SEQ_URL");
    }
}
