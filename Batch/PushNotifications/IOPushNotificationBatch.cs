using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IOBootstrap.NET.Batch.Application;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.APNS;
using IOBootstrap.NET.Common.Models.Firebase;
using IOBootstrap.NET.Core.APNS;
using IOBootstrap.NET.Core.Firebase;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.PushNotifications
{
    public abstract class IOPushNotificationBatch<TDBContext> : IOBatchClass<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        private APNSHttpServiceUtils APNS;
        protected FirebaseUtils Firebase;

        #endregion

        #region Initialization Methods

        protected IOPushNotificationBatch(string environment, IConfiguration configuration, string configurationPath, TDBContext databaseContext, ILogger<IOLoggerType> logger) : base(environment, configuration, configurationPath, databaseContext, logger)
        {
            // Obtain firebase configuration
            string firebaseApiUrl = Configuration.GetValue<string>(IOConfigurationConstants.FirebaseApiUrl);
            string firebaseToken = Configuration.GetValue<string>(IOConfigurationConstants.FirebaseToken);

            Firebase = new FirebaseUtils(firebaseApiUrl, firebaseToken, logger);

            // Obtain apns configuration
            string apnsApiUrl = Configuration.GetValue<string>(IOConfigurationConstants.APNSApiURL);
            string apnsAuthKeyID = Configuration.GetValue<string>(IOConfigurationConstants.APNSAuthKeyID);
            string apnsBundleID = Configuration.GetValue<string>(IOConfigurationConstants.APNSBundleID);
            string keyFile = Configuration.GetValue<string>(IOConfigurationConstants.APNSKeyFilePath);
            string teamID = Configuration.GetValue<string>(IOConfigurationConstants.APNSTeamID);
            string keyFilePath = Path.Combine(configurationPath, keyFile);

            APNS = new APNSHttpServiceUtils(apnsApiUrl, apnsAuthKeyID, apnsBundleID, keyFilePath, teamID, logger);
        }

        #endregion

        #region Batch Methods

        public override void Run()
        {
            base.Run();

            // Check batch is working
            if (CheckDifferentBatchIsWorking())
            {
                // Log call
                Logger.LogDebug("A different operation is running");
                return;
            }

            // Obtain message for sending 
            IList<PushNotificationMessageEntity> pushNotificationMessages = GetPushNotificationMessages();

            // Send notification
            SendNotifications(pushNotificationMessages);

            // Finalize notification
            FinalizeNotificationService();
        }

        #endregion

        #region Helper Methods

        private bool CheckDifferentBatchIsWorking()
        {
            string tempPath = Path.GetTempPath();
            string lockFile = Path.Combine(tempPath, IOPushNotificationBatchConstants.BatchLockFileName);
            if (File.Exists(lockFile))
            {
                bool isWorking = true;
                using (StreamReader reader = new StreamReader(lockFile))
                {
                    string timeIntervalString = reader.ReadLine();
                    if (timeIntervalString != null)
                    {
                        long timeInterval = Int64.Parse(timeIntervalString);
                        long currentDateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                        if (timeInterval + IOPushNotificationBatchConstants.BatchTimeoutDuration < currentDateTime)
                        {
                            isWorking = false;
                            File.Delete(lockFile);
                        }
                    }
                }

                return isWorking;
            }

            return false;
        }

        private void CreateLockFile()
        {
            string tempPath = Path.GetTempPath();
            string lockFile = Path.Combine(tempPath, IOPushNotificationBatchConstants.BatchLockFileName);
            long currentDateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            using (StreamWriter writer = new StreamWriter(lockFile))
            {
                writer.WriteLine(currentDateTime.ToString());
            }
        }

        private IList<PushNotificationMessageEntity> GetPushNotificationMessages()
        {
            IList<PushNotificationMessageEntity> pushNotificationMessages = DatabaseContext.PushNotificationMessages
                                                                                                    .Where(pushNotificationMessages => pushNotificationMessages.IsCompleted == 0)
                                                                                                    .Include(pushNotificationMessages => pushNotificationMessages.Client)
                                                                                                    .Include(pushNotificationMessages => pushNotificationMessages.PushNotificationDeviceID)
                                                                                                    .OrderByDescending(pushNotificationMessages => pushNotificationMessages.NotificationDate)
                                                                                                    .Take(IOPushNotificationBatchConstants.BatchMaximumMessageCount)
                                                                                                    .ToList();

            return pushNotificationMessages;
        }

        private IList<PushNotificationEntity> PrepareDevices(DeviceTypes deviceType, int messageId, int? clientId)
        {
            IQueryable<PushNotificationEntity> pushNotifications = DatabaseContext.PushNotifications
                                                                                    .Include(pushNotifications => pushNotifications.Client)
                                                                                    .Include(pushNotifications => pushNotifications.DeliveredMessages)
                                                                                    .ThenInclude(pushNotificationDeliveredMessages => pushNotificationDeliveredMessages.PushNotificationMessage)
                                                                                    .Where(pushNotifications => pushNotifications.DeviceType == deviceType)
                                                                                    .Where(pn => pn.DeliveredMessages.All(dm => dm.PushNotificationMessage.ID != messageId))
                                                                                    .Take(IOPushNotificationBatchConstants.BatchEntityCount);

            if (clientId != null)
            {
                pushNotifications = pushNotifications.Where(pn => pn.Client.ID == (int)clientId);
            }

            return pushNotifications.ToList();
        }

        private void SendNotifications(IList<PushNotificationMessageEntity> pushNotificationMessages)
        {
            // Loop throught messages
            foreach(PushNotificationMessageEntity message in pushNotificationMessages)
            {
                // Check notification is not for single device
                if (message.PushNotificationDeviceID == null)
                {
                    // Obtain client id
                    int? clientId = (message.Client == null) ? new int?() : message.Client.ID;

                    // Obtain firebase devices
                    IList<PushNotificationEntity> firebaseDevices = PrepareDevices(DeviceTypes.Android, message.ID, clientId);
                    if (firebaseDevices == null)
                    {
                        firebaseDevices = new List<PushNotificationEntity>();
                    }

                    SendNotificationToAllFirebaseDevices(message, firebaseDevices);

                    // Obtain apns devices
                    IList<PushNotificationEntity> apnsDevices = PrepareDevices(DeviceTypes.iOS, message.ID, clientId);
                    if (apnsDevices == null)
                    {
                        apnsDevices = new List<PushNotificationEntity>();
                    }

                    SendNotificationToAllAPNSDevices(message, apnsDevices);

                    if (firebaseDevices.Count == 0 && apnsDevices.Count == 0)
                    {
                        SetMessageToSended(message);
                    }
                }
                else 
                {
                    IList<PushNotificationEntity> devices = new List<PushNotificationEntity>();
                    devices.Add(message.PushNotificationDeviceID);

                    if (message.PushNotificationDeviceID.DeviceType == DeviceTypes.Android) 
                    {
                        // Send firebase message
                        SendNotificationToAllFirebaseDevices(message, devices);
                    }
                    else if (message.PushNotificationDeviceID.DeviceType == DeviceTypes.iOS)
                    {
                        // Send apns message
                        SendNotificationToAllAPNSDevices(message, devices);
                    }

                    // Log call 
                    Logger.LogDebug("Single device found.");
                    SetMessageToSended(message);
                }
            }
        }

        private void SendNotificationToAllFirebaseDevices(PushNotificationMessageEntity message, IList<PushNotificationEntity> firebaseDevices)
        {
            // Log call 
            Logger.LogDebug("Firebase devices found size of {0}", firebaseDevices.Count());
            IList<PushNotificationEntity> invalidDevices = new List<PushNotificationEntity>();

            // Loop throught devices
            foreach (PushNotificationEntity pushNotification in firebaseDevices)
            {
                FirebaseModel firebaseModel = new FirebaseModel(pushNotification.DeviceToken,
                message.NotificationTitle,
                message.NotificationMessage,
                message.NotificationCategory,
                message.ID,
                message.NotificationData);
                FirebaseUtils.FirebaseUtilsMessageTypes response = Firebase.SendNotifications(firebaseModel);

                if (response == FirebaseUtils.FirebaseUtilsMessageTypes.Success)
                {
                    PushNotificationDeliveredMessagesEntity deliveredMessageEntity = new PushNotificationDeliveredMessagesEntity()
                    {
                        PushNotification = pushNotification,
                        PushNotificationMessage = message
                    };
                    DatabaseContext.Add(deliveredMessageEntity);
                }
                else if (response == FirebaseUtils.FirebaseUtilsMessageTypes.DeviceNotFound)
                {
                    invalidDevices.Add(pushNotification);
                }
            }

            DatabaseContext.SaveChanges();
            DeleteInvalidDevices(invalidDevices);
        }

        private void SendNotificationToAllAPNSDevices(PushNotificationMessageEntity message, IList<PushNotificationEntity> apnsDevices)
        {
            // Log call 
            Logger.LogDebug("APNS devices found size of {0}", apnsDevices.Count());
            IList<PushNotificationEntity> invalidDevices = new List<PushNotificationEntity>();

            // Loop throught devices
            foreach (PushNotificationEntity pushNotification in apnsDevices)
            {
                APNSSendPayloadModel sendPayloadModel = new APNSSendPayloadModel(message.NotificationTitle, 
                message.NotificationMessage, 
                1, 
                pushNotification.DeviceToken, 
                message.NotificationData,
                message.NotificationCategory);
                APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes response = APNS.SendNotifications(sendPayloadModel);

                if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.Success)
                {
                    PushNotificationDeliveredMessagesEntity deliveredMessageEntity = new PushNotificationDeliveredMessagesEntity() 
                    {
                        PushNotification = pushNotification,
                        PushNotificationMessage = message
                    };
                    DatabaseContext.Add(deliveredMessageEntity);
                }
                else if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.DeviceNotFound)
                {
                    invalidDevices.Add(pushNotification);
                }
            }

            DatabaseContext.SaveChanges();
            DeleteInvalidDevices(invalidDevices);
        }

        protected virtual void DeleteInvalidDevices(IList<PushNotificationEntity> invalidDevices)
        {
            if (invalidDevices == null)
            {
                return;
            }

            foreach (PushNotificationEntity pushNotification in invalidDevices)
            {
                DatabaseContext.Remove(pushNotification.DeliveredMessages);
                DatabaseContext.Remove(pushNotification);
            }

            DatabaseContext.SaveChanges();
        }

        protected virtual void SetMessageToSended(PushNotificationMessageEntity message)
        {
            message.IsCompleted = 1;
            DatabaseContext.Update(message);
            DatabaseContext.SaveChanges();
        }

        protected virtual void FinalizeNotificationService()
        {
            string tempPath = Path.GetTempPath();
            string lockFile = Path.Combine(tempPath, IOPushNotificationBatchConstants.BatchLockFileName);
            try
            {
                File.Delete(lockFile);
            }
            catch (Exception e)
            {
                Logger.LogDebug(e.Message);
            }
        }

        #endregion
    }
}
