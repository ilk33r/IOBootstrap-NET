using System;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.PushNotification.ViewModels
{
    public class IOPushNotificationBackOfficeViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext> 
    {
        
        #region Initialization Methods

        public IOPushNotificationBackOfficeViewModel()
        {
        }

        #endregion

        #region Back Office Methods

        public virtual IList<PushNotificationMessageModel> ListMessages()
        {
            // Obtain push notification entity
            IList<PushNotificationMessageModel> messages = DatabaseContext.PushNotificationMessages
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

            if (messages == null)
            {
                return new List<PushNotificationMessageModel>();
            }

            return messages;
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

        public void DeleteMessage(int messageId)
        {
            // Obtain message 
            PushNotificationMessageEntity messageEntity = DatabaseContext.PushNotificationMessages.Find(messageId);

            // Check message entity
            if (messageEntity == null)
            {
                throw new IOInvalidRequestException();
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
        }

        #endregion
    }
}
