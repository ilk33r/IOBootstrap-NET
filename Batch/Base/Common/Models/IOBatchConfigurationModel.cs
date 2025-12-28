using System;

namespace IOBootstrap.NET.Batch.Base.Common.Models;

public class IOBatchConfigurationModel
{

    public required string IOConnectionStrings { get; set; }
    public required string IOAPNSApiURL { get; set; }
    public required string IOAPNSEncryptedKeyFilePath { get; set; }
    public required string IOEncryptionKey { get; set; }
    public required string IOEncryptionIV { get; set; }
    public required string IOFirebaseApiUrl { get; set; }
    public required string IOFirebaseEncryptedKeyFile { get; set; }
    public required string IOFirebaseProjectID { get; set; }
}
