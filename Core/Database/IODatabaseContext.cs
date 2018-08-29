using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.WebApi.PushNotification.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace IOBootstrap.NET.Core.Database
{
    public abstract class IODatabaseContext<TContext> : DbContext where TContext: DbContext
    {

        public DbSet<IOClientsEntity> Clients { get; set; }
        public DbSet<IOUserEntity> Users { get; set; }
        public DbSet<PushNotificationEntity> PushNotifications { get; set; }
        public DbSet<PushNotificationMessageEntity> PushNotificationMessageEntity { get; set; }
        public DbSet<PushNotificationDeliveredMessagesEntity> PushNotificationDeliveredMessagesEntity { get; set; }

        public IODatabaseContext(DbContextOptions<TContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IOClientsEntity>().HasIndex(
                clientEntity => new { clientEntity.ClientId }).IsUnique(false);
            modelBuilder.Entity<IOUserEntity>().HasIndex(
                userEntity => new { userEntity.UserName }).IsUnique(false);
            modelBuilder.Entity<PushNotificationEntity>().HasIndex(
                pushNotificationEntity => new
                {
                    pushNotificationEntity.DeviceId,
                    pushNotificationEntity.DeviceType,
                    pushNotificationEntity.LastUpdateTime
                }).IsUnique(false);
            modelBuilder.Entity<PushNotificationMessageEntity>().HasIndex(
                pushNotificationMessageEntity => new
                {
                    pushNotificationMessageEntity.NotificationDate,
                    pushNotificationMessageEntity.DeviceType,
                    pushNotificationMessageEntity.IsCompleted
                });
        }
    }
}
