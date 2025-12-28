using System;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.WebApi.PushNotification.ViewModels;

public class IOPushNotificationDefaultViewModel : IOPushNotificationViewModel<IODatabaseContextDefaultImpl, IOPushNotificationDevicesDefaultEntity>
{
}
