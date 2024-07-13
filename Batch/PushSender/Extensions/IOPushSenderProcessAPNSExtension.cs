using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Common.APNS;
using IOBootstrap.NET.Common.Models.APNS;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessAPNSExtension
{

    public static IList<PushNotificationEntity> SendNotificationToAllApnsDevices<TConfig, TDBContext>(
        this IOPushSenderProcess<TConfig, TDBContext> input,
        PushNotificationMessageEntity message,
        IList<PushNotificationEntity> apnsDevices
    )
    where TConfig : IOBatchConfigurationModel
    where TDBContext : IODatabaseContext<TDBContext>
    {
        input.Logger?.LogDebug("APNS devices found size of {0}", apnsDevices.Count);
        IList<PushNotificationEntity> invalidDevices = new List<PushNotificationEntity>();
        IList<PushNotificationEntity> deliveredMessages = new List<PushNotificationEntity>();

        // Loop throught devices
        foreach (PushNotificationEntity pushNotification in apnsDevices)
        {
            APNSSendPayloadModel sendPayloadModel = new APNSSendPayloadModel(message.NotificationTitle ?? "",
                                                                            message.NotificationMessage ?? "",
                                                                            pushNotification.BadgeCount,
                                                                            pushNotification.DeviceToken ?? "",
                                                                            message.NotificationData ?? "",
                                                                            message.NotificationCategory?? "");

            APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes? response = input.APNSUtilities?.SendNotifications(sendPayloadModel);

            if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.Success)
            {
                deliveredMessages.Add(pushNotification);
            }
            else
            {
                invalidDevices.Add(pushNotification);
            }
        }

        if (deliveredMessages.Count > 0)
        {
            input.UpdateDeliveredMessages(message, deliveredMessages);
        }
        
        return invalidDevices;
    }
}
