using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessSendDeviceExtension
{
    private const int MaxLimit = 250;

    public static IList<PushNotificationEntity> GetDevices<TConfig, TDBContext>(
        this IOPushSenderProcess<TConfig, TDBContext> input, 
        DeviceTypes deviceType, 
        int messageId, 
        int? clientId
    )
    where TConfig : IOBatchConfigurationModel
    where TDBContext : IODatabaseContext<TDBContext>
    {
        #pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        IList<PushNotificationEntity>? devices = input.DatabaseContext?.PushNotifications
                                                                        .Include(d => d.Client)
                                                                        .Include(d => d.DeliveredMessages)
                                                                        .ThenInclude(d => d.PushNotificationMessage)
                                                                        .Where(d => d.DeviceType == deviceType)
                                                                        .Where(d => d.DeliveredMessages!.All(dm => dm.PushNotificationMessage!.ID != messageId))
                                                                        .Take(MaxLimit)
                                                                        .ToList();
        #pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        if (clientId != null)
        {
            devices = devices?.Where(pn => pn.Client?.ID == clientId)
                                .ToList();
        }

        return devices ?? [];
    }

    public static void DeleteInvalidDevices<TConfig, TDBContext>(
        this IOPushSenderProcess<TConfig, TDBContext> input, 
        IList<PushNotificationEntity> invalidDevices
    )
    where TConfig : IOBatchConfigurationModel
    where TDBContext : IODatabaseContext<TDBContext>
    {
        if (invalidDevices == null || invalidDevices.Count == 0)
        {
            return;
        }

        foreach (PushNotificationEntity device in invalidDevices)
        {
            if (device.DeliveredMessages != null && device.DeliveredMessages.Count > 0)
            {
                input.DatabaseContext?.Remove(device.DeliveredMessages);
            }
            input.DatabaseContext?.Remove(device);
        }

        input.DatabaseContext?.SaveChanges();
    }
}
