using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.Base.Core.Interface;
using IOBootstrap.NET.Batch.PushSender.Extensions;
using IOBootstrap.NET.Common.APNS;
using IOBootstrap.NET.Common.Firebase;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Process;

public class IOPushSenderProcess<TConfig, TDBContext> : IIOBatchProcess<TConfig, TDBContext>
where TConfig : IOBatchConfigurationModel
where TDBContext : IODatabaseContext<TDBContext>
{
    public string? Environment { get; set; }
    public ILogger<IOLoggerType>? Logger { get; set; }
    public TConfig? Configuration { get; set; }
    public TDBContext? DatabaseContext { get; set; }

    public FirebaseUtils? FirebaseMessageUtilities;
    public APNSHttpServiceUtils? APNSUtilities;

    public virtual void OnLoad()
    {
        FirebaseMessageUtilities = new FirebaseUtils(
            Configuration!.IOFirebasePrivateKeyFile,
            Logger!
        );

        string currentDirectory = Directory.GetCurrentDirectory();
        string apnsFilePath = Path.Combine(currentDirectory, Configuration!.IOAPNSKeyFilePath);
        APNSUtilities = new APNSHttpServiceUtils(
            Configuration!.IOAPNSApiURL, 
            Configuration!.IOAPNSAuthKeyID, 
            Configuration!.IOAPNSBundleID, 
            apnsFilePath, 
            Configuration!.IOAPNSTeamID, 
            Logger!
        );
    }

    public virtual async Task Run()
    {
        IList<PushNotificationMessageEntity> pendingMessages = this.GetPendingPushNotificationMessages();
        this.SendMessages(pendingMessages);
    }
}
