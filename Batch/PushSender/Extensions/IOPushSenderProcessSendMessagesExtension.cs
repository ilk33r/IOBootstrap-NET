using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessSendMessagesExtension
{

    public static async Task SendMessages<TConfig, TDBContext, TPushNotificationDevicesEntity>(
        this IOPushSenderProcess<TConfig, TDBContext, TPushNotificationDevicesEntity> input,
        PushNotificationMessageEntity pushNotificationMessage,
        IList<PushNotificationDeliveredMessagesEntity> pendingDevices
    )
    where TConfig : IOBatchConfigurationModel
    where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
    where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
    {
        if (pushNotificationMessage.DeviceType == (int)DeviceTypes.Generic)
        {
            IList<PushNotificationDeliveredMessagesEntity> apnsDevices = pendingDevices
                .Where(d => d.Device?.DeviceType == DeviceTypes.iOS)
                .ToList();

            IList<PushNotificationDeliveredMessagesEntity> firebaseDevices = pendingDevices
                .Where(d => d.Device?.DeviceType == DeviceTypes.AndroidGoogle)
                .ToList();

            IList<PushNotificationDeliveredMessagesEntity> unsupportedDevices = pendingDevices
                .Where(d => d.Device?.DeviceType != DeviceTypes.iOS && d.Device?.DeviceType != DeviceTypes.AndroidGoogle)
                .ToList();

            if (apnsDevices.Count > 0)
            {
                await input.SendNotificationToAllApnsDevices(pushNotificationMessage, apnsDevices);
            }

            if (firebaseDevices.Count > 0)
            {
                await input.SendNotificationToAllFirebaseDevices(pushNotificationMessage, firebaseDevices);
            }

            foreach (PushNotificationDeliveredMessagesEntity deliveredMessage in unsupportedDevices)
            {
                deliveredMessage.IsDelivered = true;
                deliveredMessage.DeliverDate = DateTimeOffset.UtcNow;
                input.DatabaseContext?.Update(deliveredMessage);
            }
        }
        else if (pushNotificationMessage.DeviceType == (int)DeviceTypes.AndroidGoogle)
        {
            await input.SendNotificationToAllFirebaseDevices(pushNotificationMessage, pendingDevices);
        }
        else if (pushNotificationMessage.DeviceType == (int)DeviceTypes.iOS)
        {
            await input.SendNotificationToAllApnsDevices(pushNotificationMessage, pendingDevices);
        }
        else
        {
            foreach (PushNotificationDeliveredMessagesEntity deliveredMessage in pendingDevices)
            {
                deliveredMessage.IsDelivered = true;
                deliveredMessage.DeliverDate = DateTimeOffset.UtcNow;
                input.DatabaseContext?.Update(deliveredMessage);
            }
        }

        input.DatabaseContext?.SaveChanges();
    }
}
