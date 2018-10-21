﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using IOBootstrap.NET.Core.HTTP.Enumerations;
using Newtonsoft.Json;

namespace IOBootstrap.NET.Core.HTTP.Utils
{
    public delegate void HttpResponse(bool status, string response);
    public delegate void HttpJsonResponse<TObject>(bool status, TObject responseObject);

    public class IOHTTPClient
    {
        private string baseUrl;
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
            if (this.requestMethod == IOHTTPClientRequestMethods.GET)
            {
                this.GetRequest(callback);
            } else {
                this.PostRequest(callback);
            } 
        }

        public void CallJSON<TObject>(HttpJsonResponse<TObject> callback)
        {
            this.Call((bool status, string response) =>
            {
                TObject jsonObject = JsonConvert.DeserializeObject<TObject>(response);
                callback(status, jsonObject);
            });
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

        private async void GetRequest(HttpResponse callback)
        {
            var task = this.httpClient.GetAsync(this.baseUrl);
            var response = await task;

            // Deserialize the response body.
            var responseBody = await response.Content.ReadAsStringAsync();
            callback(response.IsSuccessStatusCode, responseBody);
        }

        private async void PostRequest(HttpResponse callback)
        {
            HttpContent postContent = new StringContent(JsonConvert.SerializeObject(this.postBody));
            var task = this.httpClient.PostAsync(this.baseUrl, postContent);
            var response = await task;

            // Deserialize the response body.
            var responseBody = await response.Content.ReadAsStringAsync();
            callback(response.IsSuccessStatusCode, responseBody);
        }

        #endregion
    }
}