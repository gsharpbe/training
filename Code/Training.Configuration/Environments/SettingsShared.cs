namespace Training.Configuration.Environments
{
    public class SettingsShared
    {
        public virtual string MigrationConnectionString
            => "Data Source=.\\SqlExpress;Initial Catalog=Training;Integrated Security=True";
    }
}
