using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessPendingMessagesExtension
{

    private const int MaxLimit = 250;

    public static IList<PushNotificationMessageEntity> GetPendingPushNotificationMessages<TConfig, TDBContext>(this IOPushSenderProcess<TConfig, TDBContext> input)
    where TConfig : IOBatchConfigurationModel
    where TDBContext : IODatabaseContext<TDBContext>
    {
        IList<PushNotificationMessageEntity>? pushNotificationMessages = input.DatabaseContext?.PushNotificationMessages
                                                                                            .Include(p => p.Client)
                                                                                            .Include(p => p.PushNotificationDeviceID)
                                                                                            .Where(p => p.IsCompleted == 0)
                                                                                            .OrderBy(p => p.NotificationDate)
                                                                                            .Take(MaxLimit)
                                                                                            .ToList();

        return pushNotificationMessages ?? [];
    }
}
