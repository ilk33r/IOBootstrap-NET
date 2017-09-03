using IOBootstrap.NET.Core.APNS.Utils.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IOBootstrap.NET.Core.APNS.Utils
{
    public class APNSUtils
    {

        #region Properties

        private string apnsHost;
        private int apnsPort;
        private string certificateFile;
        private string certificatePassword;
        private ILogger logger;

        #endregion

        #region Initialization Methods

        public APNSUtils(string apnsHost, int apnsPort, string certificateFile, string certificatePassword, ILogger logger)
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
            // Start async task
            Task.Factory.StartNew(() =>
            {
                // Create tcp client
                TcpClient client = new TcpClient(this.apnsHost, this.apnsPort);

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

					// Close the client connection.
					client.Close();
                }
            });
        }

        #endregion

        #region Helper Methods

        private SslStream CreateAndAuthenticateSslStream(TcpClient client)
        {
            // Load client certificate
            X509Certificate2Collection certificatesCollection = this.GetCertificatesCollection();

            // Create ssl stream
            SslStream sslStream = new SslStream(client.GetStream());

            // Authenticate stream
            try
            {
                sslStream.AuthenticateAsClient(this.apnsHost, certificatesCollection, SslProtocols.Tls, true);
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

        private X509Certificate2Collection GetCertificatesCollection()
        {
            try {
				// Load client certificate
				X509Certificate2 clientCertificate = new X509Certificate2(File.ReadAllBytes(this.certificateFile),
																		  this.certificatePassword,
																		  X509KeyStorageFlags.MachineKeySet);
				X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

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

			// The deviceId length (big-endian second byte)
			writer.Write((byte)32);

			// Write device token
			writer.Write(Encoding.ASCII.GetBytes(apnsPayloadModel.token.ToUpper()));

			// Convert payload to json
			string payloadJson = JsonConvert.SerializeObject(apnsPayloadModel.payload);

			// First byte of payload length; (big-endian first byte)
			writer.Write((byte)0);

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
			sslStream.Flush();

			// Sleep thread
			Thread.Sleep(3000);
        }

        #endregion

    }
}
