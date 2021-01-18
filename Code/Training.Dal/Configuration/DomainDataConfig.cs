using Microsoft.EntityFrameworkCore;
using Training.Dal.Context;
using Training.Dal.Models;

namespace Training.Dal.Configuration
{
    public class DomainDataConfig
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            ConfigureCountryModel(modelBuilder);
        }

        private static void ConfigureCountryModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CountryModel>()
                .ApplyDefaultConfiguration()
                .ToTable("Country");
        }
    }
}
