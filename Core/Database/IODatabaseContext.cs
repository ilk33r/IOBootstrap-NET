using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;

namespace IOBootstrap.NET.Core.Database
{
    public abstract class IODatabaseContext<TContext> : DbContext where TContext: DbContext
    {

        public DbSet<IOClientsEntity> Clients { get; set; }
        public DbSet<IOUserEntity> Users { get; set; }

        public IODatabaseContext(DbContextOptions<TContext> options) : base(options)
        {
        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<IOClientsEntity>().HasIndex(
                clientEntity => new { clientEntity.ClientId }).IsUnique(false);
			modelBuilder.Entity<IOUserEntity>().HasIndex(
                userEntity => new { userEntity.UserName }).IsUnique(false);
		}
    }
}
