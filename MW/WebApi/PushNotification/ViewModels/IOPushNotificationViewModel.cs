using System;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.PushNotification.ViewModels
{
    public class IOPushNotificationViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        public void AddTokenV2(AddPushNotificationRequestModel requestModel) 
        {
            // Obtain client
            IOClientsEntity client = null;
            if (!String.IsNullOrEmpty(requestModel.ClientId))
            {
                client = DatabaseContext.Clients
                                            .Where(client => client.ClientId.Equals(requestModel.ClientId))
                                            .FirstOrDefault();
            }


			// Obtain push notification entity
            IQueryable<PushNotificationEntity> pushNotificationsEntities;
			if (client != null)
			{
				pushNotificationsEntities = DatabaseContext.PushNotifications
                                                                .Where(pn => pn.DeviceId.Equals(requestModel.DeviceId) && pn.Client.ClientId == client.ClientId);
			}
			else 
			{
				pushNotificationsEntities = DatabaseContext.PushNotifications
                                                                .Where(pn => pn.DeviceId.Equals(requestModel.DeviceId));
			}

			// Check push notification entity exists
			if (pushNotificationsEntities != null && pushNotificationsEntities.Count() > 0)
			{
				// Loop throught push notification entity
				foreach (PushNotificationEntity pushEntity in pushNotificationsEntities) {
					pushEntity.DeviceType = DeviceTypes.Unkown;
					DatabaseContext.Update(pushEntity);
				}

				// Obtain push notification entity
				PushNotificationEntity pushNotificationEntity = pushNotificationsEntities.First();

				// Update entity properties
				pushNotificationEntity.AppBuildNumber = requestModel.AppBuildNumber;
				pushNotificationEntity.AppBundleId = requestModel.AppBundleId;
				pushNotificationEntity.AppVersion = requestModel.AppVersion;
				pushNotificationEntity.DeviceName = requestModel.DeviceName;
				pushNotificationEntity.DeviceToken = requestModel.DeviceToken;
				pushNotificationEntity.DeviceType = requestModel.DeviceType;
				pushNotificationEntity.LastUpdateTime = DateTime.UtcNow;

                // Update entity
                DatabaseContext.Update(pushNotificationEntity);

                // Write transaction
                DatabaseContext.SaveChanges();
				return;
			}

			// Create a push notification entity
			PushNotificationEntity newPushNotificationDeviceEntity = new PushNotificationEntity()
			{
				AppBuildNumber = requestModel.AppBuildNumber,
				AppBundleId = requestModel.AppBundleId,
				AppVersion = requestModel.AppVersion,
				BadgeCount = 0,
                Client = client,
				DeviceId = requestModel.DeviceId,
				DeviceName = requestModel.DeviceName,
				DeviceToken = requestModel.DeviceToken,
				DeviceType = requestModel.DeviceType,
				LastUpdateTime = DateTime.UtcNow
			};

            // Write push notification to database
            DatabaseContext.Add(newPushNotificationDeviceEntity);
            DatabaseContext.SaveChanges();
        }
    }
}
