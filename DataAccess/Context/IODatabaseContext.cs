using System;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.DataAccess.Context;

public abstract class IODatabaseContext<TContext> : DbContext where TContext : DbContext
{

    public virtual DbSet<IOConfigurationEntity> Configurations { get; set; }
    public virtual DbSet<IOImagesEntity> Images { get; set; }
    public virtual DbSet<IOLogsEntity> Logs { get; set; }
    public virtual DbSet<IOMenuEntity> Menu { get; set; }
    public virtual DbSet<IOBackOfficeMessageEntity> Messages { get; set; }
    public virtual DbSet<IOUserEntity> Users { get; set; }
    public virtual DbSet<PushNotificationEntity> PushNotifications { get; set; }
    public virtual DbSet<PushNotificationMessageEntity> PushNotificationMessages { get; set; }
    public virtual DbSet<PushNotificationDeliveredMessagesEntity> PushNotificationDeliveredMessages { get; set; }

    public IODatabaseContext(DbContextOptions<TContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IOConfigurationEntity>().HasIndex(
            configurationEntity => new { configurationEntity.ConfigKey }).IsUnique(true);

        modelBuilder.Entity<IOMenuEntity>().HasIndex(
            menuEntity => new { menuEntity.ParentEntityID, menuEntity.MenuOrder, menuEntity.RequiredRole }).IsUnique(false);

        CreateUserModel(modelBuilder);

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

        modelBuilder.Entity<IOBackOfficeMessageEntity>().HasIndex(
            messagesEntity => new
            {
                messagesEntity.MessageCreateDate,
                messagesEntity.MessageEndDate,
                messagesEntity.MessageStartDate
            });
    }

    private void CreateUserModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IOUserEntity>().HasIndex(
            userEntity => new { userEntity.UserName }).IsUnique(true);
        
        modelBuilder.Entity<IOUserEntity>().HasIndex(u => u.IsActive);
    }
}
