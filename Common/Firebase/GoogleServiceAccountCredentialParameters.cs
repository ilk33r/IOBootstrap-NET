using System;

namespace IOBootstrap.NET.Common.Firebase;

internal class GoogleServiceAccountCredentialParameters
{
    internal string? Type { get; set; }
    internal string? ProjectId { get; set; }
    internal string? PrivateKeyId { get; set; }
    internal string? PrivateKey { get; set; }
    internal string? ClientEmail { get; set; }
    internal string? ClientId { get; set; }
    internal string? AuthUri { get; set; }
    internal string? TokenUri { get; set; }
    internal string? AuthProviderX509CertUrl { get; set; }
    internal string? ClientX509CertUrl { get; set; }
}
