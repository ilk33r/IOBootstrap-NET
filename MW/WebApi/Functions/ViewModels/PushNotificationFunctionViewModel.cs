using System;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.MW.WebApi.Functions.ViewModels
{
    public class PushNotificationFunctionViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        private const int MaxLimit = 500;

        public IList<PushNotificationMessageModel> GetPendingPushNotificationMessages()
        {
            IList<PushNotificationMessageModel> pushNotificationMessages = DatabaseContext.PushNotificationMessages
                                                                                                .Select(push => new PushNotificationMessageModel()
                                                                                                {
                                                                                                    ID = push.ID,
                                                                                                    Client = new IOClientInfoModel()
                                                                                                    {
                                                                                                        Id = (push.Client != null) ? push.Client.ID : 0,
                                                                                                        ClientID = (push.Client != null) ? push.Client.ClientId : null,
                                                                                                        ClientSecret = (push.Client != null) ? push.Client.ClientSecret : null
                                                                                                    },
                                                                                                    DeviceType = push.DeviceType,
                                                                                                    IsCompleted = push.IsCompleted,
                                                                                                    NotificationDate = push.NotificationDate,
                                                                                                    NotificationTitle = push.NotificationTitle,
                                                                                                    NotificationMessage = push.NotificationMessage,
                                                                                                    NotificationCategory = push.NotificationCategory,
                                                                                                    NotificationData = push.NotificationData,
                                                                                                    PushNotificationDeviceID = new PushNotificationModel()
                                                                                                    {
                                                                                                        ID = (push.PushNotificationDeviceID != null) ? push.PushNotificationDeviceID.ID : 0,
                                                                                                        DeviceType = (push.PushNotificationDeviceID != null) ? push.PushNotificationDeviceID.DeviceType : DeviceTypes.Generic,
                                                                                                        DeviceToken = (push.PushNotificationDeviceID != null) ? push.PushNotificationDeviceID.DeviceToken : "",
                                                                                                        BadgeCount = (push.PushNotificationDeviceID != null) ? push.PushNotificationDeviceID.BadgeCount : 0
                                                                                                    }
                                                                                                })
                                                                                                .Where(push => push.IsCompleted == 0)
                                                                                                .OrderByDescending(push => push.NotificationDate)
                                                                                                .Take(MaxLimit)
                                                                                                .ToList();

            return pushNotificationMessages;
        }

        public IList<PushNotificationDevicesModel> GetDevices(IOMWPushNotificationDevicesRequestModel requestModel)
        {
            IList<PushNotificationDevicesModel> devices = DatabaseContext.PushNotifications
                                                                            .Select(device => new PushNotificationDevicesModel()
                                                                            {
                                                                                ID = device.ID,
                                                                                Client = new IOClientInfoModel()
                                                                                {
                                                                                    Id = (device.Client != null) ? device.Client.ID : 0
                                                                                },
                                                                                BadgeCount = device.BadgeCount,
                                                                                DeviceId = device.DeviceId,
                                                                                DeviceToken = device.DeviceToken,
                                                                                DeviceType = device.DeviceType,
                                                                                DeliveredMessages = device.DeliveredMessages.Select(dm => dm.PushNotificationMessage.ID).ToList()
                                                                            })
                                                                            .Where(device => device.DeviceType == requestModel.DeviceType)
                                                                            .Where(device => device.DeliveredMessages.All(dm => dm != requestModel.MessageId))
                                                                            .Take(MaxLimit)
                                                                            .ToList();

            if (requestModel.ClientId != null)
            {
                devices = devices.Where(pn => pn.Client.Id == (int)requestModel.ClientId)
                                    .ToList();
            }

            return devices;
        }

        public void UpdateDeliveredMessages(IOMWUpdatePushNotificationDeliveredMessages requestModel)
        {
            List<PushNotificationEntity> attachedPushNotifications = new List<PushNotificationEntity>();
            List<PushNotificationMessageEntity> attachedPushNotificationMessages = new List<PushNotificationMessageEntity>();

            foreach (PushNotificationDeliveredMessageModel message in requestModel.DeliveredMessages)
            {
                PushNotificationEntity pushNotification = attachedPushNotifications.Where(p => p.ID == message.PushNotificationID)
                                                                                    .FirstOrDefault();
                PushNotificationMessageEntity pushNotificationMessage = attachedPushNotificationMessages.Where(p => p.ID == message.PushNotificationMessageID)
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

        public virtual void DeleteInvalidDevices(IList<PushNotificationDevicesModel> invalidDevices)
        {
            if (invalidDevices != null && invalidDevices.Count > 0)
            {
                foreach (PushNotificationDevicesModel device in invalidDevices)
                {
                    PushNotificationEntity pushNotification = DatabaseContext.PushNotifications
                                                                                .Include(pushNotification => pushNotification.DeliveredMessages)
                                                                                .Where(pushNotification => pushNotification.ID == device.ID)
                                                                                .FirstOrDefault();

                    if (pushNotification != null)
                    {
                        DatabaseContext.Remove(pushNotification.DeliveredMessages);
                        DatabaseContext.Remove(pushNotification);
                    }
                }

                DatabaseContext.SaveChanges();
            }
        }

        public void SetMessageSended(IOMWFindRequestModel requestModel)
        {
            PushNotificationMessageEntity message = DatabaseContext.PushNotificationMessages
                                                                        .Find(requestModel.ID);

            if (message != null)
            {
                message.IsCompleted = 1;
                DatabaseContext.Update(message);
                DatabaseContext.SaveChanges();
            }
        }
    }
}
