using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Common.APNS;
using IOBootstrap.NET.Common.Models.APNS;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessAPNSExtension
{
    public static async Task SendNotificationToAllApnsDevices<TConfig, TDBContext, TPushNotificationDevicesEntity>(
        this IOPushSenderProcess<TConfig, TDBContext, TPushNotificationDevicesEntity> input,
        PushNotificationMessageEntity pushNotificationMessage,
        IList<PushNotificationDeliveredMessagesEntity> pendingDevices
    )
    where TConfig : IOBatchConfigurationModel
    where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
    where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
    {
        foreach (PushNotificationDeliveredMessagesEntity deliveredMessage in pendingDevices)
        {
            int badgeCount = deliveredMessage.Device?.BadgeCount ?? 0;
            badgeCount += 1;

            APNSSendPayloadModel sendPayloadModel = new APNSSendPayloadModel(
                pushNotificationMessage.NotificationTitle ?? String.Empty,
                pushNotificationMessage.NotificationMessage ?? String.Empty,
                badgeCount,
                deliveredMessage.Device?.DeviceToken ?? String.Empty,
                pushNotificationMessage.NotificationData ?? String.Empty,
                pushNotificationMessage.NotificationCategory ?? String.Empty
            );

            if (input.APNSUtilities == null)
            {
                continue;
            }

            APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes? response = await input.APNSUtilities.SendNotifications(sendPayloadModel);
            if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.Success)
            {
                deliveredMessage.IsDelivered = true;
                deliveredMessage.DeliverDate = DateTimeOffset.UtcNow;
                input.DatabaseContext?.Update(deliveredMessage);
            }
            else if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.DeviceNotFound && deliveredMessage.Device != null)
            {
                int wrongAttemptCount = deliveredMessage.Device.WrongAttemptCount;
                wrongAttemptCount += 1;

                if (wrongAttemptCount > 3)
                {
                    deliveredMessage.Device.LastUpdateTime = DateTimeOffset.UtcNow;
                    deliveredMessage.Device.IsActive = false;
                    deliveredMessage.IsDelivered = true;
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
