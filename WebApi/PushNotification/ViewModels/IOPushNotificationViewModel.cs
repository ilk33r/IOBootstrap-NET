using System;
using System.Linq;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.WebApi.PushNotification.ViewModels
{
    public class IOPushNotificationViewModel<TDBContext> : IOViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        public void AddToken(AddPushNotificationRequestModel requestModel) 
        {
            // Obtain client
            IQueryable<IOClientsEntity> clients = DatabaseContext.Clients
                                                                    .Where(client => client.ClientId == ClientId);
            IOClientsEntity client = null;

            // Set client
            if (clients != null && clients.Count() > 0)
            {
                client = clients.First();
            }

			// Obtain push notification entity
            IQueryable<PushNotificationEntity> pushNotificationsEntities;
			
			if (client != null)
			{
				pushNotificationsEntities = DatabaseContext.PushNotifications
                                                                .Where(pn => pn.DeviceId == requestModel.DeviceId && pn.DeviceType == requestModel.DeviceType && pn.Client.ClientId == client.ClientId);
			}
			else 
			{
				pushNotificationsEntities = DatabaseContext.PushNotifications
                                                                .Where(pn => pn.DeviceId == requestModel.DeviceId && pn.DeviceType == requestModel.DeviceType);
			}
			 

			// Check push notification entity exists
			if (pushNotificationsEntities != null && pushNotificationsEntities.Count() > 0)
			{
				// Obtain push notification entity
				PushNotificationEntity pushNotificationEntity = pushNotificationsEntities.First();

				// Update entity properties
				pushNotificationEntity.AppBuildNumber = requestModel.AppBuildNumber;
				pushNotificationEntity.AppBundleId = requestModel.AppBundleId;
				pushNotificationEntity.AppVersion = requestModel.AppVersion;
				pushNotificationEntity.BadgeCount = 0;
				pushNotificationEntity.DeviceName = requestModel.DeviceName;
				pushNotificationEntity.DeviceToken = requestModel.DeviceToken;
				pushNotificationEntity.LastUpdateTime = DateTime.UtcNow;

                // Delete all entit
                DatabaseContext.Update(pushNotificationEntity);

                // Write transaction
                DatabaseContext.SaveChanges();
			}
			else
			{
				// Create a push notification entity
				PushNotificationEntity pushNotificationEntity = new PushNotificationEntity()
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
                DatabaseContext.Add(pushNotificationEntity);
                DatabaseContext.SaveChanges();
			}
        }

    }
}
