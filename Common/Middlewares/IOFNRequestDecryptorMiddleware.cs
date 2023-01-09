using System;
using System.Text;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Utilities;

namespace IOBootstrap.NET.Common.Middlewares
{
    public class IOFNRequestDecryptorMiddleware
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<IOLoggerType> Logger;
        private readonly RequestDelegate RequestDelegate;
        private IOAESUtilities AESUtilities;

        public IOFNRequestDecryptorMiddleware(RequestDelegate next, ILogger<IOLoggerType> logger, IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            RequestDelegate = next;
            Logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            HttpContext updatedContext = context;
            if (
                context.Request.Headers.ContainsKey(IORequestHeaderConstants.IsEncrypted) &&
                context.Request.Headers[IORequestHeaderConstants.IsEncrypted].Equals("true") && 
                context.Request.Method.Equals("POST") && 
                context.Request.ContentType.Contains("text/plain")
            )
            {
                byte[] keyBytes = Convert.FromBase64String(Configuration.GetValue<string>(IOMWConfigurationConstants.EncryptionKey));
			    byte[] ivBytes = Convert.FromBase64String(Configuration.GetValue<string>(IOMWConfigurationConstants.EncryptionIV));
			    AESUtilities = new IOAESUtilities(keyBytes, ivBytes);

                Stream stream = context.Request.Body;
                StreamReader streamReader = new StreamReader(stream);
                Task<string> readerTask = streamReader.ReadToEndAsync();
                var response = await readerTask;

                string decryptedBody = AESUtilities.Decrypt(response);
                StringContent requestContent = new StringContent(decryptedBody, Encoding.UTF8, "application/json");
                stream = await requestContent.ReadAsStreamAsync();

                updatedContext.Request.ContentType = "application/json";
                updatedContext.Request.Body = stream;
            }

            await RequestDelegate(updatedContext);
        }
    }
}
