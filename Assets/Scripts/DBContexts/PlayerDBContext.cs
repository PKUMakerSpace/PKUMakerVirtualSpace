using System.Data.Entity;
using MySql.Data.EntityFramework;
using PKU.DataModels;
using System.Data.Common;

namespace PKU.DBContext
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class PlayerDBContext : DbContext
    {
        public DbSet<PlayerData> Players { get; set; }

        public PlayerDBContext() : base()
        {
       
        }

        public PlayerDBContext(DbConnection existingConnection, bool contextOwnsConnection):base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PlayerData>().MapToStoredProcedures();
        }
    }
}