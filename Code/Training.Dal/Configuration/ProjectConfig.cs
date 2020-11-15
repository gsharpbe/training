using Microsoft.EntityFrameworkCore;
using Training.Dal.Context;
using Training.Dal.Models;

namespace Training.Dal.Configuration
{
    public class ProjectConfig
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            ConfigureProjectModel(modelBuilder);
        }

        private static void ConfigureProjectModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectModel>()
                .ApplyDefaultConfiguration()
                .ToTable("Project");

            modelBuilder.Entity<ProjectModel>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Projects);
        }
    }
}