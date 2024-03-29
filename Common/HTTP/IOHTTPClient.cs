﻿using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.Common.HTTP.Enumerations;

namespace IOBootstrap.NET.Common.HTTP.Utils
{
    public delegate void HttpResponse(bool status, string response, HttpResponseHeaders headers);
    public delegate void HttpJsonResponse<TObject>(bool status, TObject responseObject) where TObject : IOModel, new();

    public class IOHTTPClient
    {
        public bool UseHttp2;
        public bool IgnoreNullValues;

        private string BaseUrl { get; }
        private string ContentType;
        private HttpClient HttpClient;
        private Object PostBody;
        private IOHTTPClientRequestMethods RequestMethod;

        #region Initialization Methods

        public IOHTTPClient(string baseUrl, ILogger logger)
        {
            UseHttp2 = false;
            IgnoreNullValues = false;
            BaseUrl = baseUrl;
            IOHttpClientHandler httpClientHandler = new IOHttpClientHandler(new HttpClientHandler(), logger);
            HttpClient = new HttpClient(httpClientHandler);
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

        public void AddAuthorizationHeader(string authorization)
        {
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
        }

        public void AddHeader(string name, string value)
        {
            HttpClient.DefaultRequestHeaders.Add(name, value);
        }

        public Task Call(HttpResponse callback)
        {
            Task task;
            if (RequestMethod == IOHTTPClientRequestMethods.GET)
            {
                task = GetRequest(callback);
            } else {
                task = PostRequest(callback);
            }
            
            return task;
        }

        public Task Call(string path, HttpResponse callback)
        {
            Task task;
            if (RequestMethod == IOHTTPClientRequestMethods.GET)
            {
                task = GetRequest(callback);
            } else {
                task = PostRequest(path, callback);
            }
            
            return task;
        }

        public void CallJSON<TObject>(HttpJsonResponse<TObject> callback) where TObject : IOModel, new()
        {
            SetContentType("application/json");
            Task task = Call((bool status, string response, HttpResponseHeaders headers) =>
            {
                try
                {
                    TObject jsonObject = JsonSerializer.Deserialize<TObject>(response);
                    callback(status, jsonObject);
                } 
                catch (Exception)
                {
                    callback(status, new TObject());
                }
            });

            task.Wait();
        }

        public TObject CallJSONSync<TObject>() where TObject : IOModel, new()
        {
            TObject jsonObject = null;
            SetContentType("application/json");
            Task task = Call((bool status, string response, HttpResponseHeaders headers) =>
            {
                try
                {
                    jsonObject = JsonSerializer.Deserialize<TObject>(response);
                } 
                catch (Exception)
                {
                }
            });

            task.Wait();
            return jsonObject;
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
                callback(response.IsSuccessStatusCode, responseBody, response.Headers);
            }
            catch (Exception e)
            {
                callback(false, e.Message, null);
            }
        }

        private async Task PostRequest(HttpResponse callback)
        {
            await PostRequest("", callback);
        }

        private async Task PostRequest(string path, HttpResponse callback)
        {
            try
            {
                string serializedBody = "";

                if (ContentType.Contains("application/json"))
                {
                    serializedBody = JsonSerializer.Serialize(PostBody, new JsonSerializerOptions()
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });
                }
                else if (ContentType.Contains("text/plain"))
                {
                    serializedBody = (string)PostBody;
                }
                HttpContent postContent = new StringContent(serializedBody, Encoding.UTF8, ContentType);
                var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + path)
                {
                    Content = postContent
                };

                if (UseHttp2)
                {
                    request.Version = new Version(2, 0);
                }
                
                var task = HttpClient.SendAsync(request);
                var response = await task;

                // Deserialize the response body.
                var responseBody = await response.Content.ReadAsStringAsync();
                callback(response.IsSuccessStatusCode, responseBody, response.Headers);
            }
            catch (Exception e)
            {
                callback(false, e.Message, null);
            }
        }

        #endregion
    }
}
