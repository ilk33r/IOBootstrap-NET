using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.Core.HTTP.Enumerations;
using Newtonsoft.Json;

namespace IOBootstrap.NET.Core.HTTP.Utils
{
    public delegate void HttpResponse(bool status, string response);
    public delegate void HttpJsonResponse<TObject>(bool status, TObject responseObject) where TObject : IOModel, new();

    public class IOHTTPClient
    {
        private string BaseUrl { get; }
        private string ContentType;
        private HttpClient HttpClient;
        private Object PostBody;
        private IOHTTPClientRequestMethods RequestMethod;

        #region Initialization Methods

        public IOHTTPClient(string baseUrl)
        {
            BaseUrl = baseUrl;
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "IOBootstrap.NET");
        }

        #endregion

        #region Publics

        public void AddAcceptHeader(string accept)
        {
            MediaTypeWithQualityHeaderValue headerValue = new MediaTypeWithQualityHeaderValue(accept);
            HttpClient.DefaultRequestHeaders.Accept.Add(headerValue);
        }

        public void AddHeader(string name, string value)
        {
            HttpClient.DefaultRequestHeaders.Add(name, value);
        }

        public void Call(HttpResponse callback)
        {
            Task task;
            if (RequestMethod == IOHTTPClientRequestMethods.GET)
            {
                task = GetRequest(callback);
            } else {
                task = PostRequest(callback);
            }
            task.Wait();
        }

        public void CallJSON<TObject>(HttpJsonResponse<TObject> callback) where TObject : IOModel, new()
        {
            Call((bool status, string response) =>
            {
                SetContentType("application/json");
                try
                {
                    TObject jsonObject = JsonConvert.DeserializeObject<TObject>(response);
                    callback(status, jsonObject);
                } 
                catch (Exception)
                {
                    callback(status, new TObject());
                }
            });
        }

        public void SetContentType(string contentType)
        {
            ContentType = contentType;
        }

        public void SetPostBody(Object bodyObject)
        {
            PostBody = bodyObject;
        }

        public void SetRequestMethod(IOHTTPClientRequestMethods requestMethod)
        {
            RequestMethod = requestMethod;
        }

        #endregion

        #region Privates

        private async Task GetRequest(HttpResponse callback)
        {
            try 
            {
                var task = HttpClient.GetAsync(BaseUrl);
                var response = await task;

                // Deserialize the response body.
                var responseBody = await response.Content.ReadAsStringAsync();
                callback(response.IsSuccessStatusCode, responseBody);
            }
            catch (Exception e)
            {
                callback(false, e.Message);
            }
        }

        private async Task PostRequest(HttpResponse callback)
        {
            try
            {
                HttpContent postContent = new StringContent(JsonConvert.SerializeObject(PostBody), Encoding.UTF8, ContentType);
                var task = HttpClient.PostAsync(BaseUrl, postContent);
                var response = await task;

                // Deserialize the response body.
                var responseBody = await response.Content.ReadAsStringAsync();
                callback(response.IsSuccessStatusCode, responseBody);
            }
            catch (Exception e)
            {
                callback(false, e.Message);
            }
        }

        #endregion
    }
}
