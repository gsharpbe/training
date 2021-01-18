using Microsoft.EntityFrameworkCore;
using Serilog;
using Training.Dal.Configuration;

namespace Training.Dal.Context
{
    public class DataContext : DbContext
    {
        private static readonly ILogger Logger = Log.ForContext<DataContext>();

        public DataContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            DomainDataConfig.Configure(modelBuilder);
            CustomerConfig.Configure(modelBuilder);
            ProjectConfig.Configure(modelBuilder);
        }

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        //{
        //    var now = DateTimeOffset.UtcNow;

        //    var addedEntities = GetEntities(EntityState.Added);
        //    var modifiedEntities = GetEntities(EntityState.Modified);

        //    foreach (var entity in addedEntities)
        //    {
        //        entity.CreateDate = now;
        //        entity.ModifyDate = now;
        //        entity.Creator = "SYSTEM";
        //        entity.Modifier = "SYSTEM";
        //    }

        //    foreach (var entity in modifiedEntities)
        //    {
        //        entity.ModifyDate = now;
        //        entity.Modifier = "SYSTEM";
        //    }

        //    return base.SaveChangesAsync(cancellationToken);
        //}

        //private List<ModelBase> GetEntities(EntityState entityState)
        //{
        //    var entities = ChangeTracker.Entries()
        //        .Where(x => x.State == entityState)
        //        .Select(x =>x.Entity)
        //        .OfType<ModelBase>()
        //        .ToList();

        //    return entities;
        //}
    }
}
