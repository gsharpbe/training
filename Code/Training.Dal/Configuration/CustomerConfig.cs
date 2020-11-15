using Microsoft.EntityFrameworkCore;
using Training.Dal.Context;
using Training.Dal.Models;

namespace Training.Dal.Configuration
{
    public class CustomerConfig
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            ConfigureCustomerModel(modelBuilder);
            ConfigureAddressModel(modelBuilder);
        }

        private static void ConfigureCustomerModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerModel>()
                .ApplyDefaultConfiguration()
                .ToTable("Customer");
        }

        private static void ConfigureAddressModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressModel>()
                .ApplyDefaultConfiguration()
                .ToTable("Address");

            modelBuilder.Entity<AddressModel>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Addresses);
        }
    }
}
