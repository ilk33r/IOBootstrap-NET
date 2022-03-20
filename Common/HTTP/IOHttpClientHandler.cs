using System;

namespace IOBootstrap.NET.Common.HTTP
{
    public class IOHttpClientHandler : DelegatingHandler
    {

        private ILogger Logger;

        public IOHttpClientHandler(HttpMessageHandler innerHandler, ILogger logger) : base(innerHandler) {
            this.Logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Request: \n{0}\n", request.ToString());

            if (request.Content != null)
            {
                string requestContent = await request.Content.ReadAsStringAsync();
                Logger.LogInformation(requestContent);
            }

            Logger.LogInformation("\n\n");
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            Logger.LogInformation("Response: \n{0}\n", response.ToString());

            if (response.Content != null)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Logger.LogInformation(responseContent);
            }

            return response;
        }
    }
}
