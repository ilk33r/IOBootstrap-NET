using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Models.APNS;
using System.Security.Cryptography.X509Certificates;
using IOBootstrap.NET.Common.HTTP;
using IOBootstrap.NET.Common.HTTP.Enumerations;
using System.Security.Authentication;

namespace IOBootstrap.NET.Common.APNS;

public class APNSHttpServiceUtils
{
    public enum APNSHttpServiceUtilsMessageTypes
    {
        Success = 0,
        DeviceNotFound = 1,
        Failure = 2,
        InvalidCertificate = 3
    }

    private const string EndpointFormat = "{0}/3/device/{1}";

    #region Properties

    private string APNSApiUrl;
    private string APNSBundleID;
    private ILogger<IOLoggerType>? Logger;
    private X509Certificate2 Certificate;

    #endregion

    #region Initialization Methods

    public APNSHttpServiceUtils(
        ILogger<IOLoggerType>? logger,
        string apnsApiUrl, 
        string apnsBundleID, 
        byte[] apnsKeyFileData,
        string apnsKeyFilePassword
    )
    {
        // Setup properties
        this.Logger = logger;
        this.APNSApiUrl = apnsApiUrl;
        this.APNSBundleID = apnsBundleID;

        this.Certificate = new X509Certificate2(
            apnsKeyFileData,
            apnsKeyFilePassword,
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable
        );
    }

    #endregion

    #region Utility Methods

    public async Task<APNSHttpServiceUtilsMessageTypes> SendNotifications(APNSSendPayloadModel payloadData)
    {
        // Create http client
        string endpoint = String.Format(EndpointFormat, APNSApiUrl, payloadData.DeviceToken);
        HttpClientHandler handler = new HttpClientHandler
        {
            SslProtocols = SslProtocols.Tls12
        };
        handler.ClientCertificates.Add(Certificate);

        IOHTTPClient httpClient = new IOHTTPClient(endpoint, Logger!, handler);
        httpClient.UseHttp2 = true;
        httpClient.IgnoreNullValues = true;

        // Add headers
        httpClient.AddHeader("apns-topic", APNSBundleID);
        httpClient.AddHeader("apns-push-type", "alert");

        // Optional
        // request.Headers.TryAddWithoutValidation("apns-priority", "10"); // 10 = high, 5 = low
        // request.Headers.TryAddWithoutValidation("apns-expiration", "0");

        // Set request method
        httpClient.SetRequestMethod(IOHTTPClientRequestMethods.POST);

        // Set request body
        httpClient.SetPostBody(payloadData.Payload);

        // Call http client
        var response = await httpClient.CallJSONAsync<APNSResponseModel>();
        if (response.Item1 && response.Item2 == null)
        {
            Logger?.LogInformation("APNS api called successfully.");
            return APNSHttpServiceUtilsMessageTypes.Success;
        }
        
        if (response.Item2 != null && response.Item2?.Reason == "BadCertificate")
        {
            Logger?.LogError("APNS api call failed. Invalid certificate.");
            return APNSHttpServiceUtilsMessageTypes.InvalidCertificate;
        }

        if (response.Item2 != null && response.Item2?.Reason == "BadCertificateEnvironment")
        {
            Logger?.LogError("APNS api call failed. Invalid certificate.");
            return APNSHttpServiceUtilsMessageTypes.InvalidCertificate;
        }

        if (response.Item2 != null && response.Item2?.Reason == "TopicDisallowed")
        {
            Logger?.LogError("APNS api call failed. Invalid certificate.");
            return APNSHttpServiceUtilsMessageTypes.InvalidCertificate;
        }

        if (response.Item2 != null && response.Item2?.Reason == "BadDeviceToken")
        {
            Logger?.LogError("APNS api call failed. Device not found.");
            return APNSHttpServiceUtilsMessageTypes.DeviceNotFound;
        }

        if (response.Item2 != null && response.Item2?.Reason == "Unregistered")
        {
            Logger?.LogError("APNS api call failed. Device not found.");
            return APNSHttpServiceUtilsMessageTypes.DeviceNotFound;
        }

        Logger?.LogError("APNS api call failed.");
        return APNSHttpServiceUtilsMessageTypes.Failure;
    }

    #endregion
}
