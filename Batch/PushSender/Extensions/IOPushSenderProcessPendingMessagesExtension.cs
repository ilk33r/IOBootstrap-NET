using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessPendingMessagesExtension
{

    private const int MaxLimit = 250;

    public static PushNotificationMessageEntity? GetPendingPushNotificationMessage<TConfig, TDBContext, TPushNotificationDevicesEntity>(
        this IOPushSenderProcess<TConfig, TDBContext, TPushNotificationDevicesEntity> input
    )
    where TConfig : IOBatchConfigurationModel
    where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
    where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
    {
        if (input.DatabaseContext == null)
        {
            return null;
        }

        PushNotificationMessageEntity? pushNotificationMessage = input.DatabaseContext.PushNotificationMessages
        .Include(p => p.DeliveredMessages)!
        .ThenInclude(p => p.Device)
        .Where(p => !p.IsCompleted)
        .OrderBy(p => p.CreatedDate)
        .FirstOrDefault();

        if (pushNotificationMessage == null)
        {
            return null;
        }

        if (pushNotificationMessage.DeliveredMessages?.Count() > 0)
        {
            int notSendedDeviceCount = pushNotificationMessage.DeliveredMessages
            .Where(d => !d.IsDelivered)
            .Count();

            if (notSendedDeviceCount > 0)
            {
                return pushNotificationMessage;
            }

            pushNotificationMessage.IsCompleted = true;
            input.DatabaseContext?.Update(pushNotificationMessage);
            input.DatabaseContext?.SaveChanges();
            return null;
        }

        input.CreatePushNotificationDeliveredMessage(pushNotificationMessage);
        return null;
    }

    public static IList<PushNotificationDeliveredMessagesEntity> GetPendingPushNotificationDevices<TConfig, TDBContext, TPushNotificationDevicesEntity>(
        this IOPushSenderProcess<TConfig, TDBContext, TPushNotificationDevicesEntity> input, 
        PushNotificationMessageEntity pushNotificationMessage
    )
    where TConfig : IOBatchConfigurationModel
    where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
    where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
    {
        return pushNotificationMessage.DeliveredMessages?
        .Where(d => !d.IsDelivered)
        .OrderBy(d => d.ID)
        .Take(MaxLimit)
        .ToList() ?? [];
    }

    private static void CreatePushNotificationDeliveredMessage<TConfig, TDBContext, TPushNotificationDevicesEntity>(
        this IOPushSenderProcess<TConfig, TDBContext, TPushNotificationDevicesEntity> input, 
        PushNotificationMessageEntity pushNotificationMessage
    )
    where TConfig : IOBatchConfigurationModel
    where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
    where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
    {
        IQueryable<TPushNotificationDevicesEntity>? pushNotificationDevicesQuery = input.DatabaseContext?.PushNotificationDevices
        .Where(d => d.IsActive);
        
        if (pushNotificationMessage.DeviceType != (int)DeviceTypes.Generic)
        {
            DeviceTypes deviceTYpe = ((DeviceTypes?)pushNotificationMessage.DeviceType) ?? DeviceTypes.Unkown;
            pushNotificationDevicesQuery = pushNotificationDevicesQuery?
            .Where(d => d.DeviceType == deviceTYpe);
        }

        IList<TPushNotificationDevicesEntity>? pushNotificationDevices = pushNotificationDevicesQuery?
        .OrderByDescending(d => d.LastUpdateTime)
        .ToList();

        if (pushNotificationDevices == null || pushNotificationDevices.Count() == 0)
        {
            pushNotificationMessage.IsCompleted = true;
            input.DatabaseContext?.Update(pushNotificationMessage);
            input.DatabaseContext?.SaveChanges();
            return;
        }

        foreach (TPushNotificationDevicesEntity item in pushNotificationDevices)
        {
            PushNotificationDeliveredMessagesEntity deliveredMessage = new PushNotificationDeliveredMessagesEntity()
            {
                Device = item,
                PushNotificationMessage = pushNotificationMessage,
                IsDelivered = false,
                CreatedDate = DateTimeOffset.UtcNow,
                DeliverDate = null
            };

            input.DatabaseContext?.Add(deliveredMessage);
        }

        input.DatabaseContext?.SaveChanges();
    }
}
