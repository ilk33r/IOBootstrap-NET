using System;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.FN;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.FunctionsApi.PushNotificationFunction.ViewModels;

public class PushNotificationFunctionViewModel<TDBContext> : IOFunctionsViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
{

    public void UpdateDeliveredMessages(IOFNUpdatePushNotificationDeliveredMessages requestModel)
    {
        List<PushNotificationEntity> attachedPushNotifications = new List<PushNotificationEntity>();
        List<PushNotificationMessageEntity> attachedPushNotificationMessages = new List<PushNotificationMessageEntity>();

        foreach (PushNotificationDeliveredMessageModel message in requestModel.DeliveredMessages!)
        {
            PushNotificationEntity? pushNotification = attachedPushNotifications.Where(p => p.ID == message.PushNotificationID)
                                                                                .FirstOrDefault();
            PushNotificationMessageEntity? pushNotificationMessage = attachedPushNotificationMessages.Where(p => p.ID == message.PushNotificationMessageID)
                                                                                                    .FirstOrDefault();

            if (pushNotification == null)
            {
                pushNotification = new PushNotificationEntity()
                {
                    ID = message.PushNotificationID
                };
                DatabaseContext.Attach(pushNotification);
                attachedPushNotifications.Add(pushNotification);
            }

            if (pushNotificationMessage == null)
            {
                pushNotificationMessage = new PushNotificationMessageEntity()
                {
                    ID = message.PushNotificationMessageID
                };
                DatabaseContext.Attach(pushNotificationMessage);
                attachedPushNotificationMessages.Add(pushNotificationMessage);
            }

            try
            {
                PushNotificationDeliveredMessagesEntity deliveredMessageEntity = new PushNotificationDeliveredMessagesEntity()
                {
                    PushNotification = pushNotification,
                    PushNotificationMessage = pushNotificationMessage
                };
                DatabaseContext.Add(deliveredMessageEntity);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.StackTrace);
            }
        }

        DatabaseContext.SaveChanges();
        DeleteInvalidDevices(requestModel.InvalidDevices);
    }

    public virtual void DeleteInvalidDevices(IList<PushNotificationDevicesModel>? invalidDevices)
    {
        if (invalidDevices != null && invalidDevices.Count > 0)
        {
            foreach (PushNotificationDevicesModel device in invalidDevices)
            {
                PushNotificationEntity? pushNotification = DatabaseContext.PushNotifications
                                                                            .Include(pushNotification => pushNotification.DeliveredMessages)
                                                                            .Where(pushNotification => pushNotification.ID == device.ID)
                                                                            .FirstOrDefault();

                if (pushNotification != null)
                {
                    if (pushNotification.DeliveredMessages != null)
                    {
                        DatabaseContext.Remove(pushNotification.DeliveredMessages);
                    }
                    DatabaseContext.Remove(pushNotification);
                }
            }

            DatabaseContext.SaveChanges();
        }
    }

    public void SetMessageSended(IOFNFindRequestModel requestModel)
    {
        PushNotificationMessageEntity? message = DatabaseContext.PushNotificationMessages
                                                                    .Find(requestModel.ID);

        if (message != null)
        {
            message.IsCompleted = 1;
            DatabaseContext.Update(message);
            DatabaseContext.SaveChanges();
        }
    }
}
