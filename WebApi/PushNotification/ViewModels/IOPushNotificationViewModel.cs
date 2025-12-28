using System;
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.WebApi.PushNotification.ViewModels;

public class IOPushNotificationViewModel<TDBContext, TPushNotificationDevicesEntity> : IOViewModel<TDBContext>
where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
{


	public async Task AddPushNotificationToken(AddPushNotificationRequestModel requestModel)
	{
		// Decrypt values
		string? deviceId = await DecryptString(requestModel.EncryptedDeviceId!);
		string? bundleId = await DecryptString(requestModel.EncryptedAppBundleId!);
		string? deviceName = await DecryptString(requestModel.EncryptedDeviceName!);
		string? deviceToken = await DecryptString(requestModel.EncryptedDeviceToken!);

		// Obtain push notification entity
		if (String.IsNullOrEmpty(deviceId))
        {
            throw new IOInvalidRequestException();
        }

		IQueryable<TPushNotificationDevicesEntity> pushNotificationsEntities = DatabaseContext.PushNotificationDevices
															.Where(pn => pn.DeviceId!.Equals(deviceId));

		// Check push notification entity exists
		if (pushNotificationsEntities != null && pushNotificationsEntities.Count() > 0)
		{
			// Loop throught push notification entity
			foreach (TPushNotificationDevicesEntity pushEntity in pushNotificationsEntities)
			{
				pushEntity.DeviceType = DeviceTypes.Unkown;
				DatabaseContext.Update(pushEntity);
			}

			// Obtain push notification entity
			TPushNotificationDevicesEntity pushNotificationEntity = pushNotificationsEntities.First();

			// Update entity properties
			pushNotificationEntity.AppBuildNumber = requestModel.AppBuildNumber ?? 0;
			pushNotificationEntity.AppBundleId = bundleId;
			pushNotificationEntity.AppVersion = requestModel.AppVersion;
			pushNotificationEntity.WrongAttemptCount = 0;
			pushNotificationEntity.IsActive = true;
			pushNotificationEntity.DeviceName = deviceName;
			pushNotificationEntity.DeviceToken = deviceToken;
			pushNotificationEntity.DeviceType = requestModel.DeviceType ?? DeviceTypes.Unkown;
			pushNotificationEntity.LastUpdateTime = DateTime.UtcNow;

			// Update entity
			UpdatePushNotificationDeviceEntity(ref pushNotificationEntity);

			// Update entity
			DatabaseContext.Update(pushNotificationEntity);

			// Write transaction
			DatabaseContext.SaveChanges();
			return;
		}

		// Create a push notification entity
		TPushNotificationDevicesEntity newPushNotificationDeviceEntity = new TPushNotificationDevicesEntity()
		{
			AppBuildNumber = requestModel.AppBuildNumber ?? 0,
			AppBundleId = bundleId,
			AppVersion = requestModel.AppVersion,
			BadgeCount = 0,
			WrongAttemptCount = 0,
			IsActive = true,
			DeviceId = deviceId,
			DeviceName = deviceName,
			DeviceToken = deviceToken,
			DeviceType = requestModel.DeviceType ?? DeviceTypes.Unkown,
			LastUpdateTime = DateTime.UtcNow
		};

		// Update entity
		UpdatePushNotificationDeviceEntity(ref newPushNotificationDeviceEntity);

		// Write push notification to database
		DatabaseContext.Add(newPushNotificationDeviceEntity);
		DatabaseContext.SaveChanges();
	}

	public virtual void UpdatePushNotificationDeviceEntity(ref TPushNotificationDevicesEntity pushNotificationEntity)
    {
    }
}
