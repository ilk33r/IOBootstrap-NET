using System;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.WebApi.PushNotification.ViewModels
{
    public class IOPushNotificationViewModel : IOViewModel
    {
		//TODO: Migrate with MW.
		/*
		public void AddTokenV2(AddPushNotificationRequestModel requestModel) 
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
			IOAESUtilities aesUtility = GetAesUtility();
			string decryptedDeviceId = aesUtility.Decrypt(requestModel.DeviceId);
			string decryptedDeviceToken = aesUtility.Decrypt(requestModel.DeviceToken);
			
			if (client != null)
			{
				pushNotificationsEntities = DatabaseContext.PushNotifications
                                                                .Where(pn => pn.DeviceId == decryptedDeviceId && pn.Client.ClientId == client.ClientId);
			}
			else 
			{
				pushNotificationsEntities = DatabaseContext.PushNotifications
                                                                .Where(pn => pn.DeviceId == decryptedDeviceId);
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
				pushNotificationEntity.DeviceToken = decryptedDeviceToken;
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
				DeviceId = decryptedDeviceId,
				DeviceName = requestModel.DeviceName,
				DeviceToken = decryptedDeviceToken,
				DeviceType = requestModel.DeviceType,
				LastUpdateTime = DateTime.UtcNow
			};

            // Write push notification to database
            DatabaseContext.Add(newPushNotificationDeviceEntity);
            DatabaseContext.SaveChanges();
        }
		*/
    }
}
