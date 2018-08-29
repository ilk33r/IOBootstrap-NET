using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.PushNotification.Entities;
using System;
using System.Linq;

namespace IOBootstrap.NET.WebApi.PushNotification.ViewModels
{
    public class IOPushNotificationViewModel<TDBContext> : IOViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        public IOPushNotificationViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public void AddToken(int appBuildNumber, string appBundleId, 
                             string appVersion, 
                             string deviceId, 
                             string deviceName, 
                             string deviceToken, 
                             DeviceTypes deviceType) 
        {
			// Obtain push notification entity
            var pushNotificationsEntities = _databaseContext.PushNotifications
												 .Where((arg) => arg.DeviceId == deviceId && (int)arg.DeviceType == (int)deviceType);

			// Check push notification entity exists
			if (pushNotificationsEntities.Count() > 0)
			{
				// Obtain push notification entity
				PushNotificationEntity pushNotificationEntity = pushNotificationsEntities.First();

				// Update entity properties
				pushNotificationEntity.AppBuildNumber = appBuildNumber;
				pushNotificationEntity.AppBundleId = appBundleId;
				pushNotificationEntity.AppVersion = appVersion;
				pushNotificationEntity.BadgeCount = 0;
				pushNotificationEntity.DeviceName = deviceName;
				pushNotificationEntity.DeviceToken = deviceToken;
				pushNotificationEntity.LastUpdateTime = DateTime.UtcNow;

                // Delete all entit
                _databaseContext.Update(pushNotificationEntity);

                // Write transaction
                _databaseContext.SaveChanges();
			}
			else
			{
				// Create a push notification entity
				PushNotificationEntity pushNotificationEntity = new PushNotificationEntity()
				{
					AppBuildNumber = appBuildNumber,
					AppBundleId = appBundleId,
					AppVersion = appVersion,
					BadgeCount = 0,
					DeviceId = deviceId,
					DeviceName = deviceName,
					DeviceToken = deviceToken,
					DeviceType = (int)deviceType,
					LastUpdateTime = DateTime.UtcNow
				};

                // Write push notification to database
                _databaseContext.Add(pushNotificationEntity);
                _databaseContext.SaveChanges();
			}
        }

        #endregion

    }
}
