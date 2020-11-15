using System;
using System.Linq;
using System.Threading.Tasks;
using Metanous.Model.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Metanous.WebApi.Core.Extensions
{
    public static class DbSetExtensions
    {
        public static T Get<T>(this DbSet<T> dbSet, Guid guid) where T : ModelBase
        {
            return dbSet.FirstOrDefault(x => Equals(x.Guid, guid));
        }

        public static async Task<T> GetAsync<T>(this DbSet<T> dbSet, Guid guid) where T : ModelBase
        {
            return await dbSet.FirstOrDefaultAsync(x => Equals(x.Guid, guid));
        }

        public static T Get<T>(this DbSet<T> dbSet, int id) where T : ModelBase
        {
            return dbSet.Find(id);
        }

        public static async Task<T> GetAsync<T>(this DbSet<T> dbSet, int id) where T : ModelBase
        {
            return await dbSet.FindAsync(id);
        }
    }
}
