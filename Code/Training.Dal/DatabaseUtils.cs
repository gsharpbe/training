using Microsoft.EntityFrameworkCore;
using Training.Configuration;
using Training.Dal.Context;

namespace Training.Dal
{
    public static class DatabaseUtils
    {
        public static void Migrate()
        {
            var builder = GetBuilder();

            using (var dataContext = new DataContext(builder.Options))
            {
                dataContext.Database.Migrate();
            }
        }

        private static DbContextOptionsBuilder GetBuilder()
        {
            var builder = new DbContextOptionsBuilder();

            builder.UseSqlServer(Settings.Current.DboConnectionString,
                options => options.MigrationsAssembly(Settings.Current.MigrationsAssembly));

            return builder;
        }
    }
}
