﻿using System;
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
        private string baseUrl;
        private string contentType;
        private HttpClient httpClient;
        private Object postBody;
        private IOHTTPClientRequestMethods requestMethod;

        #region Initialization Methods

        public IOHTTPClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", "IOBootstrap.NET");
        }

        #endregion

        #region Publics

        public void AddAcceptHeader(string accept)
        {
            MediaTypeWithQualityHeaderValue headerValue = new MediaTypeWithQualityHeaderValue(accept);
            this.httpClient.DefaultRequestHeaders.Accept.Add(headerValue);
        }

        public void AddHeader(string name, string value)
        {
            this.httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public void Call(HttpResponse callback)
        {
            Task task;
            if (this.requestMethod == IOHTTPClientRequestMethods.GET)
            {
                task = this.GetRequest(callback);
            } else {
                task = this.PostRequest(callback);
            }
            task.Wait();
        }

        public void CallJSON<TObject>(HttpJsonResponse<TObject> callback) where TObject : IOModel, new()
        {
            this.Call((bool status, string response) =>
            {
                this.SetContentType("application/json");
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
            this.contentType = contentType;
        }

        public void SetPostBody(Object bodyObject)
        {
            this.postBody = bodyObject;
        }

        public void SetRequestMethod(IOHTTPClientRequestMethods requestMethod)
        {
            this.requestMethod = requestMethod;
        }

        #endregion

        #region Privates

        private async Task GetRequest(HttpResponse callback)
        {
            try 
            {
                var task = this.httpClient.GetAsync(this.baseUrl);
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
                HttpContent postContent = new StringContent(JsonConvert.SerializeObject(this.postBody), Encoding.UTF8, this.contentType);
                var task = this.httpClient.PostAsync(this.baseUrl, postContent);
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
