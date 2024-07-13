using System;

namespace IOBootstrap.NET.Batch.Base.Common.Models;

public class IOBatchConfigurationModel
{

    public required string IOConnectionStrings { get; set; }
    public required string IOAPNSApiURL { get; set; }
    public required string IOAPNSAuthKeyID { get; set; }
    public required string IOAPNSBundleID { get; set; }
    public required string IOAPNSKeyFilePath { get; set; }
    public required string IOAPNSTeamID { get; set; }
    public required string IOFirebasePrivateKeyFile { get; set; }
}
