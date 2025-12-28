using System;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.DataAccess.Context;

public class IODatabaseContextDefaultImpl : IODatabaseContext<IODatabaseContextDefaultImpl, IOPushNotificationDevicesDefaultEntity>
{
    public IODatabaseContextDefaultImpl(DbContextOptions<IODatabaseContextDefaultImpl> options) : base(options)
    {
    }
}
