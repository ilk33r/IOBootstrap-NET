using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using IOBootstrap.NET.Common.Models.APNS;
using IOBootstrap.NET.Core.Logger;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.APNS
{
    public class APNSocketServiceUtils
    {

        #region Properties

        private string apnsHost;
        private int apnsPort;
        private string certificateFile;
        private string certificatePassword;
        private ILogger<IOLoggerType> logger;

        #endregion

        #region Initialization Methods

        public APNSocketServiceUtils(string apnsHost, int apnsPort, string certificateFile, string certificatePassword, ILogger<IOLoggerType> logger)
        {
            // Setup properties
            this.apnsHost = apnsHost;
            this.apnsPort = apnsPort;
            this.certificateFile = certificateFile;
            this.certificatePassword = certificatePassword;
            this.logger = logger;
        }

        #endregion

        #region Utility Methods

        public void SendNotifications(List<APNSSendPayloadModel> apnsSendPayloads)
        {
            // Create tcp client
            TcpClient client = this.createTcpClientAndConnect();

            // Check client connected 
            if (client != null) {
                // Create ssl stream
                SslStream sslStream = this.CreateAndAuthenticateSslStream(client);

                // Check if stream is not null
                if (sslStream != null)
                {
                    // Loop throught payloads
                    foreach (APNSSendPayloadModel payloadModel in apnsSendPayloads)
                    {
                        this.SendNotificationToDevice(sslStream, payloadModel);
                    }

                    // Close ssl stream
                    sslStream.Close();
                    sslStream.Dispose();
                }

                // Close the client connection.
                client.Close();
            }
        }

        #endregion

        #region Helper Methods

        private SslStream CreateAndAuthenticateSslStream(TcpClient client)
        {
            // Load client certificate
            X509CertificateCollection certificatesCollection = this.GetCertificatesCollection();

            // Create ssl stream
            SslStream sslStream = new SslStream(client.GetStream());

            // Authenticate stream
            try
            {
                sslStream.AuthenticateAsClient(this.apnsHost, certificatesCollection, SslProtocols.Tls12, true);
                sslStream.ReadTimeout = 30;
            }
            catch (AuthenticationException ex)
            {
                this.logger.LogDebug("Apns Authentication Exception {0}", ex.Message);
                client.Close();
                return null;
            }

            // Return ssl stream
            return sslStream;
        }

        private TcpClient createTcpClientAndConnect() {
            // Create tcp client
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(this.apnsHost, this.apnsPort);
                return client;
            }
            catch (SocketException ex)
            {
                this.logger.LogDebug("An error occurred while connecting to APNS servers. {0}", ex.Message);
                return null;
            }
        }

        private X509CertificateCollection GetCertificatesCollection()
        {
            try {
                // Load client certificate
                X509Certificate2 clientCertificate = new X509Certificate2(File.ReadAllBytes(this.certificateFile),
                                                                          this.certificatePassword,
                                                                          X509KeyStorageFlags.MachineKeySet);
                X509CertificateCollection certificatesCollection = new X509CertificateCollection { clientCertificate };

                // Return certificate collection
                return certificatesCollection;
            }
            catch  (Exception ex)
            {
                this.logger.LogDebug("Apns X509Certificate2Collection Exception {0}", ex.Message);
                return null;
            }
        }

        private void SendNotificationToDevice(SslStream sslStream, APNSSendPayloadModel apnsPayloadModel)
        {
            // Encode a message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            // Write command
            writer.Write((byte)0);

            // The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)0);

            // Obtain device token length
            int deviceTokenLength = apnsPayloadModel.DeviceToken.Length / 2;

            // The deviceId length (big-endian second byte)
            writer.Write((byte)deviceTokenLength);

            //convert Devide token to HEX value.
            byte[] deviceToken = new byte[deviceTokenLength];
            for (int i = 0; i < deviceTokenLength; i++) {
                deviceToken[i] = byte.Parse(apnsPayloadModel.DeviceToken.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            
            // Write device token
            writer.Write(deviceToken);

            // First byte of payload length; (big-endian first byte)
            writer.Write((byte)0);

            // Convert payload to json
            string payloadJson = JsonSerializer.Serialize(apnsPayloadModel.Payload);

            // Payload length (big-endian second byte)
            writer.Write((byte)payloadJson.Length);

            // Convert payload string to bytes
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadJson);

            // Write bytes
            writer.Write(payloadBytes);
            writer.Flush();

            // Convert memory stream to byte array
            byte[] memoryStreamBytes = memoryStream.ToArray();

            // Send memory stream to socket
            sslStream.Write(memoryStreamBytes);

            // Check stream can read
            if (sslStream.CanRead)
            {
                try
                {
                    byte[] buffer = new byte[6];
                    sslStream.Read(buffer, 0, buffer.Length);
                    int status = BitConverter.ToInt32(buffer, 0);
                }
                catch (IOException ex)
                {
                    this.logger.LogDebug("Apns response failed {0}", ex.Message);
                }
            }

            // Sleep thread
            Thread.Sleep(100);
        }

        #endregion

    }
}
