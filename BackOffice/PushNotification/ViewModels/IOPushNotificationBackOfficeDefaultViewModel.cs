using System;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.PushNotification.ViewModels;

public class IOPushNotificationBackOfficeDefaultViewModel : IOPushNotificationBackOfficeViewModel<IODatabaseContextDefaultImpl, IOPushNotificationDevicesDefaultEntity>
{
    public IOPushNotificationBackOfficeDefaultViewModel() : base()
    {
    }
}
