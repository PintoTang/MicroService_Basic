using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Feng.Basic
{
    public class WebApiHelper
    {
        /// <summary>
        /// 服务同步get
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="url"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static T1 CallGetWebApi<T1>(string url, string serviceUrl, int? timeOut = 60, string mediaType = "application/json")
        {
            using (var client = new HttpClient(new HeaderClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                client.Timeout = new TimeSpan(0, 0, timeOut.HasValue ? timeOut.Value : 60);
                client.BaseAddress = new Uri(serviceUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                var taskRes = client.GetAsync(url);
                var response = taskRes.Result;
                T1 result = default(T1);
                try { result = response.Content.ReadAsAsync<T1>().Result; }
                catch (Exception) { }
                return result;
            }
        }




        /// <summary>
        /// 服务同步post
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static T1 CallPostWebApi<T1, T2>(string url, T2 request, string serviceUrl,int? timeOut = 60, string mediaType = "application/json")
        {
            using (var client = new HttpClient(new HeaderClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                client.Timeout = new TimeSpan(0, 0, timeOut.HasValue ? timeOut.Value : 60);
                client.BaseAddress = new Uri(serviceUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                //client.DefaultRequestHeaders.Add("Accept", mediaType);
                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
                HttpContent content = new ObjectContent<T2>(request, jsonFormatter);
                var taskRes = client.PostAsync(url, content);
                var response = taskRes.Result;
                T1 result = default(T1);
                try { result = response.Content.ReadAsAsync<T1>().Result; }
                catch (Exception) { }
                return result;
            }
        }



        /// <summary>
        /// 服务同步批量接口
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="batchRequestModels"></param>
        /// <param name="url"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static List<TResponse> CallWebApiBatch<TRequest, TResponse>(HttpMethod method, string endpoint, List<TRequest> batchRequestModels, string url, string serviceUrl, int? timeOut = 60, string mediaType = "application/json")
        {
            var result = new List<TResponse>();
            using (var client = new HttpClient(new HeaderClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                client.BaseAddress = new Uri(serviceUrl);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                client.DefaultRequestHeaders.Add("Accept", mediaType);
                var multiContents = new MultipartContent("mixed");
                if (method == HttpMethod.Post)
                {
                    foreach (var batchRequestModel in batchRequestModels)
                    {
                        multiContents.Add(new HttpMessageContent(new HttpRequestMessage(HttpMethod.Post, serviceUrl + url)
                        {
                            Content = new ObjectContent<TRequest>(batchRequestModel, new JsonMediaTypeFormatter())
                        }));
                    }
                }
                if (method == HttpMethod.Get)
                {
                    foreach (var batchRequestModel in batchRequestModels)
                    {
                        multiContents.Add(new HttpMessageContent(new HttpRequestMessage(HttpMethod.Get, serviceUrl + url + batchRequestModel)));
                    }
                }
                var batchRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = multiContents
                };
                var batchResponse = client.SendAsync(batchRequest).Result;
                var streamProvider = batchResponse.Content.ReadAsMultipartAsync().Result;
                foreach (var content in streamProvider.Contents)
                {
                    var responseMessage = content.ReadAsHttpResponseMessageAsync().Result;
                    var response = responseMessage.Content.ReadAsAsync<TResponse>(new[] { new JsonMediaTypeFormatter() }).Result;
                    result.Add(response);
                }
                return result;
            }
        }


        /// <summary>
        /// 服务异步post
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static async Task<T1> CallPostWebApiAsync<T1, T2>(string url, T2 request, string serviceUrl, int? timeOut = 60, string mediaType = "application/json")
        {
            using (var client = new HttpClient(new HeaderClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                client.Timeout = new TimeSpan(0, 0, timeOut.HasValue ? timeOut.Value : 60);
                client.BaseAddress = new Uri(serviceUrl);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                client.DefaultRequestHeaders.Add("Accept", mediaType);
                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
                HttpContent content = new ObjectContent<T2>(request, jsonFormatter);
                var taskRes = client.PostAsync(url, content);
                var response = await taskRes;
                T1 result = await response.Content.ReadAsAsync<T1>();
                return result;
            }
        }


        /// <summary>
        /// 服务异步get
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="url"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static async Task<T1> CallGetWebApiAsync<T1>(string url, string serviceUrl, int? timeOut = 60, string mediaType = "application/json")
        {
            using (var client = new HttpClient(new HeaderClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                client.Timeout = new TimeSpan(0, 0, timeOut.HasValue ? timeOut.Value : 60);
                client.BaseAddress = new Uri(serviceUrl);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                client.DefaultRequestHeaders.Add("Accept", mediaType);
                var taskRes = client.GetAsync(url);
                var response = await taskRes;
                T1 result = await response.Content.ReadAsAsync<T1>();
                return result;
            }
        }

        /// <summary>
        /// 服务异步批量
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="batchRequestModels"></param>
        /// <param name="url"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="timeOut"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static async Task<List<TResponse>> CallWebApiBatchAsync<TRequest, TResponse>(HttpMethod method, string endpoint, List<TRequest> batchRequestModels, string url, string serviceUrl, int? timeOut = 60, string mediaType = "application/json")
        {
            var result = new List<TResponse>();
            using (var client = new HttpClient(new HeaderClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                client.BaseAddress = new Uri(serviceUrl);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                client.DefaultRequestHeaders.Add("Accept", mediaType);
                var multiContents = new MultipartContent("mixed");
                if (method == HttpMethod.Post)
                {
                    foreach (var batchRequestModel in batchRequestModels)
                    {
                        multiContents.Add(
                            new HttpMessageContent(new HttpRequestMessage(HttpMethod.Post, serviceUrl + url)
                            {
                                Content = new ObjectContent<TRequest>(batchRequestModel, new JsonMediaTypeFormatter())
                            }));
                    }
                }
                if (method == HttpMethod.Get)
                {
                    foreach (var batchRequestModel in batchRequestModels)
                    {
                        multiContents.Add(new HttpMessageContent(new HttpRequestMessage(HttpMethod.Get, serviceUrl + url + batchRequestModel)));
                    }
                }
                var batchRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = multiContents
                };
                var batchResponse = await client.SendAsync(batchRequest);
                var streamProvider = await batchResponse.Content.ReadAsMultipartAsync();
                foreach (var content in streamProvider.Contents)
                {
                    var responseMessage = await content.ReadAsHttpResponseMessageAsync();
                    var response =
                        responseMessage.Content.ReadAsAsync<TResponse>(new[] { new JsonMediaTypeFormatter() }).Result;
                    result.Add(response);
                }
                return result;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HeaderClientHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //request.Headers.Add("X-Feng", "项目名称");

            return base.SendAsync(request, cancellationToken);
        }
    }
}
