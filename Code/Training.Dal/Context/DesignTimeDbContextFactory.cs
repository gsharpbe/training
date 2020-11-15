using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Training.Configuration;

namespace Training.Dal.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"Connection: {Settings.Current.MigrationConnectionString}");
            Console.WriteLine($"Migration assembly: {Settings.Current.MigrationsAssembly}");
            Console.WriteLine("");

            var builder = new DbContextOptionsBuilder();

            builder.UseSqlServer(Settings.Current.MigrationConnectionString,
                options => options.MigrationsAssembly(Settings.Current.MigrationsAssembly));

            return new DataContext(builder.Options);
        }
    }
}
