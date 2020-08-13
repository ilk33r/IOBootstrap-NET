using System;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.PushNotification.ViewModels
{
    public class IOPushNotificationBackOfficeDefaultViewModel : IOPushNotificationBackOfficeViewModel<IODatabaseContextDefaultImpl>
    {
        public IOPushNotificationBackOfficeDefaultViewModel() : base() 
        {
        }
    }
}
