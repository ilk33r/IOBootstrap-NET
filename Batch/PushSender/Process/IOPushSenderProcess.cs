using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.Base.Core.Interface;
using IOBootstrap.NET.Batch.PushSender.Extensions;
using IOBootstrap.NET.Common.APNS;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Firebase;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Process;

public class IOPushSenderProcess<TConfig, TDBContext, TPushNotificationDevicesEntity> : IIOBatchProcess<TConfig, TDBContext, TPushNotificationDevicesEntity>
where TConfig : IOBatchConfigurationModel
where TPushNotificationDevicesEntity : IOPushNotificationDevicesEntity, new()
where TDBContext : IODatabaseContext<TDBContext, TPushNotificationDevicesEntity>
{
    public string? Environment { get; set; }
    public ILogger<IOLoggerType>? Logger { get; set; }
    public TConfig? Configuration { get; set; }
    public TDBContext? DatabaseContext { get; set; }
    public IServiceProvider? ServiceProvider { get; set; }
    public FirebaseUtils? FirebaseMessageUtilities;
    public APNSHttpServiceUtils? APNSUtilities;

    public virtual void OnLoad()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string apnsFilePath = Path.Combine(currentDirectory, Configuration!.IOAPNSEncryptedKeyFilePath);
        if (!File.Exists(apnsFilePath))
        {
            Logger?.LogError("APNS Key File not found at {0}", Configuration!.IOAPNSEncryptedKeyFilePath);
            return;
        }

        // Read file
        byte[] apnsFileContent = File.ReadAllBytes(apnsFilePath);

        // Convert key and iv to byte array
        byte[] key = Convert.FromBase64String(
            System.Environment.GetEnvironmentVariable(IOEnvironmentConstants.EncryptionKey) ?? string.Empty
        );
        byte[] iv = Convert.FromBase64String(
            System.Environment.GetEnvironmentVariable(IOEnvironmentConstants.EncryptionIV) ?? string.Empty
        );

        // Base 64 encode user token data
        IOAESUtilities aesUtilities = new IOAESUtilities(key, iv);

        string firebaseFilePath = Path.Combine(currentDirectory, Configuration!.IOFirebaseEncryptedKeyFile);
        if (!File.Exists(firebaseFilePath))
        {
            Logger?.LogError("Firebase Key File not found at {0}", Configuration!.IOFirebaseEncryptedKeyFile);
            return;
        }

        // Read file
        string firebaseFileContent = File.ReadAllText(firebaseFilePath);

        FirebaseMessageUtilities = new FirebaseUtils(
            Logger!,
            Configuration?.IOFirebaseApiUrl ?? String.Empty,
            Configuration?.IOFirebaseProjectID ?? String.Empty,
            firebaseFileContent
        );

        IOConfigurationEntity? apnsBundleID = DatabaseContext?.Configurations.Where(c => c.ConfigKey!.Equals(IOConfigurationKeys.APNSBundleIDKey)).FirstOrDefault();
        if (apnsBundleID == null || String.IsNullOrEmpty(apnsBundleID.ConfigStringValue))
        {
            Logger?.LogError("apnsBundleID not found.");
            return;
        }

        IOConfigurationEntity? encryptedAPNSKeyPassword = DatabaseContext?.Configurations.Where(c => c.ConfigKey!.Equals(IOConfigurationKeys.EncryptedAPNSKeyPasswordKey)).FirstOrDefault();
        if (encryptedAPNSKeyPassword == null || String.IsNullOrEmpty(encryptedAPNSKeyPassword.ConfigStringValue))
        {
            Logger?.LogError("encryptedAPNSKeyPassword not found.");
            return;
        }

        byte[] encryptedAPNSKeyPasswordBytes = Convert.FromBase64String(encryptedAPNSKeyPassword.ConfigStringValue);
        string encryptedAPNSKeyPasswordString = aesUtilities.Decrypt(encryptedAPNSKeyPasswordBytes);

        this.APNSUtilities = new APNSHttpServiceUtils(
            Logger,
            Configuration?.IOAPNSApiURL ?? String.Empty, 
            apnsBundleID.ConfigStringValue,
            apnsFileContent,
            encryptedAPNSKeyPasswordString 
        );
    }

    public virtual async Task Run()
    {
        PushNotificationMessageEntity? pushNotificationMessages = this.GetPendingPushNotificationMessage();
        if (pushNotificationMessages == null)
        {
            return;
        }

        IList<PushNotificationDeliveredMessagesEntity> pendingDevices = this.GetPendingPushNotificationDevices(pushNotificationMessages);
        await this.SendMessages(pushNotificationMessages, pendingDevices);
    }
}