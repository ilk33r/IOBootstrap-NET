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
        foreach (PushNotificationDeliveredMessagesEntity deliveredMessage in pendingDevices)
        {
            if (pushNotificationMessage.DeviceType == (int)DeviceTypes.Generic)
            {
                if (deliveredMessage.Device?.DeviceType == DeviceTypes.iOS)
                {
                    await input.SendNotificationToAllApnsDevices(
                        pushNotificationMessage,
                        pendingDevices
                    );
                }
                else if (deliveredMessage.Device?.DeviceType == DeviceTypes.AndroidGoogle)
                {
                    await input.SendNotificationToAllFirebaseDevices(
                        pushNotificationMessage,
                        pendingDevices
                    );
                }
            }
            else if (pushNotificationMessage.DeviceType == (int)DeviceTypes.AndroidGoogle)
            {
                await input.SendNotificationToAllFirebaseDevices(
                    pushNotificationMessage,
                    pendingDevices
                );
            }
            else if (pushNotificationMessage.DeviceType == (int)DeviceTypes.iOS)
            {
                await input.SendNotificationToAllApnsDevices(
                    pushNotificationMessage,
                    pendingDevices
                );
            }
            else
            {
                deliveredMessage.IsDelivered = true;
                input.DatabaseContext?.Update(deliveredMessage);
            }
        }

        input.DatabaseContext?.SaveChanges();
    }
}
