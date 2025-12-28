using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Extensions;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.PushNotification.ViewModels;

public class IOPushNotificationBackOfficeViewModel<TDBContext, TPushNotificationDevicesEntity> : IOBackOfficeViewModel<TDBContext>
where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
{

    #region Initialization Methods

    public IOPushNotificationBackOfficeViewModel()
    {
    }

    #endregion

    #region Back Office Methods

    public void CheckPushNotificationsIsEnabled()
    {
        bool isEnabled = Configuration.GetValue<bool>(IOConfigurationConstants.PushNotificationsEnabled);
        if (!isEnabled)
        {
            throw new IOInvalidAPIException();
        }
    }

    public virtual ListPushNotificationMessageResponseModel ListMessages(IOListPushNotificationsRequestModel requestModel)
    {
        int pushNotificationMessagesCount = DatabaseContext.PushNotificationMessages.Count();
        
        // Obtain push notification entity
        IList<PushNotificationMessageModel>? paginatedMessages = DatabaseContext.PushNotificationMessages
                                                                            .Select(pm => new PushNotificationMessageModel()
                                                                            {
                                                                                ID = pm.ID,
                                                                                DeviceType = pm.DeviceType,
                                                                                NotificationCategory = pm.NotificationCategory,
                                                                                NotificationData = pm.NotificationData,
                                                                                NotificationMessage = pm.NotificationMessage,
                                                                                NotificationTitle = pm.NotificationTitle,
                                                                                IsCompleted = pm.IsCompleted,
                                                                                CreatedBy = pm.CreatedBy,
                                                                                CreatedDate = pm.CreatedDate,
                                                                                UpdateDate = pm.UpdateDate,
                                                                                DeliveredDevicesCount = pm.DeliveredMessages!
                                                                                .Where(dm => dm.IsDelivered)
                                                                                .Count()
                                                                            })
                                                                            .OrderByDescending(p => p.ID)
                                                                            .Skip(requestModel.Start ?? 0)
                                                                            .Take(requestModel.Count ?? 0)
                                                                            .ToList();

        return new ListPushNotificationMessageResponseModel(pushNotificationMessagesCount, paginatedMessages);
    }

    public void SendNotifications(SendPushNotificationRequestModel requestModel)
    {
        // Create push notification message entity
        PushNotificationMessageEntity pushNotificationMessageEntity = new PushNotificationMessageEntity()
        {
            DeviceType = (int)requestModel.DeviceType,
            NotificationCategory = requestModel.NotificationCategory,
            NotificationData = requestModel.NotificationData?.SanitizeHtml(),
            NotificationMessage = requestModel.NotificationMessage?.SanitizeHtml(),
            NotificationTitle = requestModel.NotificationTitle?.SanitizeHtml(),
            IsCompleted = false,
            CreatedBy = UserModel?.UserName,
            CreatedDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow
        };

        // Write message to database
        DatabaseContext.Add(pushNotificationMessageEntity);
        DatabaseContext.SaveChanges();
    }

    public void DeleteMessage(int? messageId)
    {
        if (messageId == null)
        {
            throw new IOInvalidRequestException();
        }
        
        // Obtain message 
        PushNotificationMessageEntity? messageEntity = DatabaseContext.PushNotificationMessages.Find(messageId);

        // Check message entity
        if (messageEntity == null)
        {
            throw new IOInvalidRequestException();
        }

        // Set message completed
        if (!messageEntity.IsCompleted)
        {
            messageEntity.IsCompleted = true;
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
