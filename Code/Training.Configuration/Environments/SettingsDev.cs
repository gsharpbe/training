namespace Training.Configuration.Environments
{
    public class SettingsDev : SettingsShared, ISettings
    {
        public BuildConfiguration BuildConfiguration => BuildConfiguration.Development;

        // connection strings
        public string ConnectionString
            => "Data Source=.\\SqlExpress;Initial Catalog=Training;Integrated Security=True";

        public string DboConnectionString
            => "Data Source=.\\SqlExpress;Initial Catalog=Training;Integrated Security=True";
        
        // migrations
        public string MigrationsAssembly => "Training.Migrations.Development";

        // uri's
        public string SeqUrl => "http://localhost:5341";
    }
}
