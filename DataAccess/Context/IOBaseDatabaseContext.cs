using System;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.DataAccess.Context;

public abstract class IOBaseDatabaseContext<TContext> : DbContext
where TContext : DbContext
{
    public virtual DbSet<IOConfigurationEntity> Configurations { get; set; }
    public virtual DbSet<IOExceptionEntity> Exceptions { get; set; }
    public virtual DbSet<IOFilesEntity> Files { get; set; }
    public virtual DbSet<IOImagesEntity> Images { get; set; }
    public virtual DbSet<IOLogsEntity> Logs { get; set; }
    public virtual DbSet<IOMenuEntity> Menu { get; set; }
    public virtual DbSet<IOBackOfficeMessageEntity> Messages { get; set; }
    public virtual DbSet<IOUserEntity> Users { get; set; }
    public virtual DbSet<PushNotificationMessageEntity> PushNotificationMessages { get; set; }
    public virtual DbSet<PushNotificationDeliveredMessagesEntity> PushNotificationDeliveredMessages { get; set; }

    public IOBaseDatabaseContext(DbContextOptions<TContext> options) : base(options)
    {
    }
}
