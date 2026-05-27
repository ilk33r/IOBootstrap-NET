using System;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.DataAccess.Context;

public abstract class IODatabaseContext<TContext, TPushNotificationDevicesEntity> : IOBaseDatabaseContext<TContext>
where TContext : DbContext
where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity
{
    public virtual DbSet<TPushNotificationDevicesEntity> PushNotificationDevices { get; set; }

    public IODatabaseContext(DbContextOptions<TContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CreateConfigurationModel(modelBuilder);
        CreateExceptionModel(modelBuilder);
        CreateFilesModel(modelBuilder);

        modelBuilder.Entity<IOMenuEntity>().HasIndex(
            menuEntity => new { menuEntity.ParentEntityID, menuEntity.MenuOrder, menuEntity.RequiredRole }).IsUnique(false);

        CreateUserModel(modelBuilder);
        CreatePushNotificationsModel(modelBuilder);

        modelBuilder.Entity<IOBackOfficeMessageEntity>().HasIndex(
            messagesEntity => new
            {
                messagesEntity.MessageCreateDate,
                messagesEntity.MessageEndDate,
                messagesEntity.MessageStartDate
            });
    }

    private void CreateConfigurationModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IOConfigurationEntity>().HasIndex(
            configurationEntity => new { configurationEntity.ConfigKey }).IsUnique(true);
    }

    private void CreateExceptionModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IOExceptionEntity>().HasIndex(
            userEntity => new { userEntity.RequestDate });
    }

    private void CreateFilesModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IOFilesEntity>().HasIndex(
            it => new { it.Description });

        modelBuilder.Entity<IOFilesEntity>().HasIndex(
            it => new { it.FileType });

        modelBuilder.Entity<IOFilesEntity>().HasIndex(
            it => new { it.CreatedDate });
    }
    
    private void CreateUserModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IOUserEntity>().HasIndex(
            userEntity => new { userEntity.UserName }).IsUnique(true);

        modelBuilder.Entity<IOUserEntity>().HasIndex(u => u.IsActive);
    }

    private void CreatePushNotificationsModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TPushNotificationDevicesEntity>().HasIndex(
            d => new
            {
                d.DeviceId,
                d.DeviceType,
                d.LastUpdateTime
            });

        modelBuilder.Entity<TPushNotificationDevicesEntity>().HasIndex(
            d => new
            {
                d.IsActive
            });

        modelBuilder.Entity<PushNotificationMessageEntity>().HasIndex(
            pushNotificationMessageEntity => new
            {
                pushNotificationMessageEntity.CreatedDate,
                pushNotificationMessageEntity.DeviceType,
                pushNotificationMessageEntity.IsCompleted
            });

        modelBuilder.Entity<PushNotificationDeliveredMessagesEntity>().HasIndex(
            dm => new
            {
                dm.CreatedDate,
                dm.IsDelivered
            });
    }
}
