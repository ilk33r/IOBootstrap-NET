using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public class IOPushNotificationBackOfficeViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        public virtual IList<PushNotificationMessageModel> ListMessages()
        {
            // Obtain push notification entity
            return DatabaseContext.PushNotificationMessages
                                                        .Select(pm => new PushNotificationMessageModel()
                                                        {
                                                            ID = pm.ID,
                                                            Client = (pm.Client == null) ? null : new IOClientInfoModel(pm.Client.ID, 
                                                                                                                        pm.Client.ClientId,
                                                                                                                        pm.Client.ClientSecret,
                                                                                                                        pm.Client.ClientDescription,
                                                                                                                        pm.Client.IsEnabled,
                                                                                                                        pm.Client.RequestCount,
                                                                                                                        pm.Client.MaxRequestCount),
                                                            DeviceType = pm.DeviceType,
                                                            NotificationCategory = pm.NotificationCategory,
                                                            NotificationData = pm.NotificationData,
                                                            NotificationDate = pm.NotificationDate,
                                                            NotificationTitle = pm.NotificationTitle,
                                                            NotificationMessage = pm.NotificationMessage,
                                                            IsCompleted = pm.IsCompleted
                                                        })
                                                        .OrderByDescending(p => p.ID)
                                                        .ToList();
        }

        public void SendNotifications(SendPushNotificationRequestModel requestModel)
        {
            // Obtain client
            IOClientsEntity clientsEntity = null;
            
            if (requestModel.ClientId != null)
            {
                clientsEntity = DatabaseContext.Clients.Find(requestModel.ClientId);
            }

            // Create push notification message entity
            PushNotificationMessageEntity pushNotificationMessageEntity = new PushNotificationMessageEntity()
            {
                Client = clientsEntity,
                DeviceType = (int)requestModel.DeviceType,
                NotificationCategory = requestModel.NotificationCategory,
                NotificationData = requestModel.NotificationData,
                NotificationMessage = requestModel.NotificationMessage,
                NotificationTitle = requestModel.NotificationTitle,
                NotificationDate = DateTime.UtcNow,
                IsCompleted = 0
            };

            // Write message to database
            DatabaseContext.Add(pushNotificationMessageEntity);
            DatabaseContext.SaveChanges();
        }

        public int DeleteMessage(IOMWFindRequestModel requestModel)
        {
            // Obtain message 
            PushNotificationMessageEntity messageEntity = DatabaseContext.PushNotificationMessages.Find(requestModel.ID);

            // Check message entity
            if (messageEntity == null)
            {
                return IOResponseStatusMessages.UnkownException;
            }

            // Set message completed
            if (messageEntity.IsCompleted == 0) 
            {
                messageEntity.IsCompleted = 1;
                DatabaseContext.Update(messageEntity);
                DatabaseContext.SaveChanges();
            }

            // Obtain delivered messages
            var deliveredMessages = DatabaseContext.PushNotificationDeliveredMessages
                                                   .Where((arg) => arg.PushNotificationMessage == messageEntity);

            // Loop throught delivered messages
            foreach (PushNotificationDeliveredMessagesEntity deliveredMessage in deliveredMessages) 
            {
                DatabaseContext.Remove(deliveredMessage);
            }

            if (deliveredMessages.Count() > 0)
            {
                DatabaseContext.SaveChanges();
            }

            DatabaseContext.Remove(messageEntity);
            DatabaseContext.SaveChanges();

            return IOResponseStatusMessages.OK;
        }
    }
}
