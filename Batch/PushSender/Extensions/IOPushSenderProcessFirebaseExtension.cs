using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Common.Firebase;
using IOBootstrap.NET.Common.Models.Firebase;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessFirebaseExtension
{

    public static IList<PushNotificationEntity> SendNotificationToAllFirebaseDevices<TConfig, TDBContext>(
        this IOPushSenderProcess<TConfig, TDBContext> input,
        PushNotificationMessageEntity message,
        IList<PushNotificationEntity> googleDevices
    )
    where TConfig : IOBatchConfigurationModel
    where TDBContext : IODatabaseContext<TDBContext>
    {
        input.Logger?.LogDebug("Firebase devices found size of {0}", googleDevices.Count);
        IList<PushNotificationEntity> invalidDevices = new List<PushNotificationEntity>();
        IList<PushNotificationEntity> deliveredMessages = new List<PushNotificationEntity>();

        // Loop throught devices
        foreach (PushNotificationEntity pushNotification in googleDevices)
        {
            FirebaseModel firebaseModel = new FirebaseModel(pushNotification.DeviceToken ?? "",
                                                            message.NotificationTitle ?? "",
                                                            message.NotificationMessage ?? "",
                                                            message.NotificationCategory ?? "",
                                                            message.ID,
                                                            message.NotificationData ?? "",
                                                            pushNotification.BadgeCount);

            FirebaseUtils.FirebaseUtilsMessageTypes? response = input.FirebaseMessageUtilities?.SendNotifications(firebaseModel);

            if (response == FirebaseUtils.FirebaseUtilsMessageTypes.Success)
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
