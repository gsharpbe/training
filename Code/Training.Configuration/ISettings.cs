namespace Training.Configuration
{
    public interface ISettings
    {
        BuildConfiguration BuildConfiguration { get; }

        // connection strings
        string ConnectionString { get; }
        string DboConnectionString { get; }
        string MigrationConnectionString { get; }

        // migrations
        string MigrationsAssembly { get; }

        // uri's
        string SeqUrl { get; }
    }
}
