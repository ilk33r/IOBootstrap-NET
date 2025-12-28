using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Common.Firebase;
using IOBootstrap.NET.Common.Models.Firebase;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessFirebaseExtension
{

    public static async Task SendNotificationToAllFirebaseDevices<TConfig, TDBContext, TPushNotificationDevicesEntity>(
        this IOPushSenderProcess<TConfig, TDBContext, TPushNotificationDevicesEntity> input,
        PushNotificationMessageEntity pushNotificationMessage,
        IList<PushNotificationDeliveredMessagesEntity> pendingDevices
    )
    where TConfig : IOBatchConfigurationModel
    where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
    where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
    {
        if (input.FirebaseMessageUtilities == null)
        {
            return;
        }

        string firebaseToken = await input.FirebaseMessageUtilities.GetAccessTokenAsync();
        foreach (PushNotificationDeliveredMessagesEntity deliveredMessage in pendingDevices)
        {
            int badgeCount = deliveredMessage.Device?.BadgeCount ?? 0;
            badgeCount += 1;

            FirebaseModel firebaseModel = new FirebaseModel(
                deliveredMessage.Device?.DeviceToken ?? String.Empty,
                pushNotificationMessage.NotificationTitle ?? String.Empty,
                pushNotificationMessage.NotificationMessage ?? String.Empty,
                pushNotificationMessage.NotificationCategory ?? String.Empty,
                pushNotificationMessage.ID,
                pushNotificationMessage.NotificationData ?? String.Empty,
                badgeCount
            );

            if (input.FirebaseMessageUtilities == null)
            {
                continue;
            }

            FirebaseUtils.FirebaseUtilsMessageTypes? response = await input.FirebaseMessageUtilities.SendNotificationsAsync(firebaseModel, firebaseToken);
            if (response == FirebaseUtils.FirebaseUtilsMessageTypes.Success)
            {
                deliveredMessage.IsDelivered = true;
                deliveredMessage.DeliverDate = DateTimeOffset.UtcNow;
                input.DatabaseContext?.Update(deliveredMessage);
            }
            else if (response == FirebaseUtils.FirebaseUtilsMessageTypes.DeviceNotFound && deliveredMessage.Device != null)
            {
                int wrongAttemptCount = deliveredMessage.Device.WrongAttemptCount;
                wrongAttemptCount += 1;

                if (wrongAttemptCount > 3)
                {
                    deliveredMessage.Device.LastUpdateTime = DateTimeOffset.UtcNow;
                    deliveredMessage.Device.IsActive = false;
                }
                else
                {
                    deliveredMessage.Device.WrongAttemptCount = wrongAttemptCount;
                }

                input.DatabaseContext?.Update(deliveredMessage.Device);
            }
        }
    }
}
