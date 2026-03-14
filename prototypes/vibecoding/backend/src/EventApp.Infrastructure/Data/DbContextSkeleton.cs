using Microsoft.EntityFrameworkCore;

namespace EventApp.Infrastructure.Data
{
    public class DbContextSkeleton : DbContext
    {
        public DbContextSkeleton(DbContextOptions<DbContextSkeleton> options) : base(options)
        {
        }

        // No DbSets defined here: placeholder for later domain entities and migrations.
    }
}
using Microsoft.EntityFrameworkCore;









}
n    // No DbSets defined here: placeholder for later domain entities and migrations.    }    {    public DbContextSkeleton(DbContextOptions<DbContextSkeleton> options) : base(options){
npublic class DbContextSkeleton : DbContextnnamespace EventApp.Infrastructure.Data;