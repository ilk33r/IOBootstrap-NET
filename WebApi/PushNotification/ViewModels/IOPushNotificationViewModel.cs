using System;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.WebApi.PushNotification.ViewModels
{
    public class IOPushNotificationViewModel<TDBContext> : IOViewModel<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext> 
    {
		public void AddTokenV2(AddPushNotificationRequestModel requestModel) 
        {
			IOAESUtilities aesUtility = GetAesUtility();
			AddPushNotificationRequestModel request = requestModel;
			request.DeviceId = aesUtility.Decrypt(requestModel.DeviceId);
			request.DeviceToken = aesUtility.Decrypt(requestModel.DeviceToken);
			request.ClientId = ClientId;

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
