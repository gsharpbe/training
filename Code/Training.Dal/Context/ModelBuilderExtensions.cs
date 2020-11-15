using Metanous.Model.Core.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Training.Dal.Context
{
    public static class ModelBuilderExtensions
    {
        // apply mappings for ModelBase by extension method as TPC inheritance strategy is not implemented yet
        public static EntityTypeBuilder<T> ApplyDefaultConfiguration<T>(this EntityTypeBuilder<T> builder)
            where T : ModelBase
        {
            builder.HasIndex(x => x.Guid).IsUnique();

            return builder;
        }
    }
}