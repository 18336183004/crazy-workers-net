using CW.Common.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CW.Common.Network
{
    /// <summary>
    /// HttpClient帮助类
    /// 需要依赖注入
    /// </summary>
    public class HttpClientHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientHelper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        #region 内部方法
        /// <summary>
        /// 构建HttpClient
        /// </summary>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        private HttpClient BuildHttpClient(Dictionary<string, string> dicDefaultHeaders, int? timeoutSecond)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Clear();   //in order that the client is not affected by the last request,it need to clear DefaultRequestHeaders
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
            if (dicDefaultHeaders != null && dicDefaultHeaders?.Count > 0)
            {
                foreach (var headerItem in dicDefaultHeaders)
                {
                    if (!httpClient.DefaultRequestHeaders.Contains(headerItem.Key))
                    {
                        httpClient.DefaultRequestHeaders.Add(headerItem.Key, headerItem.Value);
                    }
                }
            }
            if (timeoutSecond != (int?)null)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutSecond.Value);
            }
            return httpClient;
        }
        /// <summary>
        /// 生成HttpRequestMessage
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="method"></param>
        /// <param name="dicHeaders"></param>
        /// <returns></returns>
        private static HttpRequestMessage GenerateHttpRequestMessage(string url, string requestBody, HttpMethod method, Dictionary<string, string> dicHeaders)
        {
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(requestBody))
            {
                request.Content = new StringContent(requestBody);
            }
            if (dicHeaders != null && dicHeaders?.Count > 0)
            {
                foreach (var header in dicHeaders)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            return request;
        }
        /// <summary>
        ///  生成请求体(包含请求头)
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="dicHeaders"></param>
        /// <returns></returns>
        private static StringContent GenerateStringContent(string requestBody, Dictionary<string, string> dicHeaders)
        {
            var content = new StringContent(requestBody);
            if (dicHeaders != null && dicHeaders?.Count > 0)
            {
                foreach (var headerItem in dicHeaders)
                {
                    content.Headers.Add(headerItem.Key, headerItem.Value);
                }
            }
            return content;
        }
        /// <summary>
        /// 转换响应内容Json格式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="responseContent"></param>
        /// <returns></returns>
        private static TResult ConvertResponseContentToResult<TResult>(string responseContent)
        {
            //空校验
            if (string.IsNullOrEmpty(responseContent))
            {
                return default;
            }
            var result = JsonConvert.DeserializeObject<TResult>(responseContent);
            return result;
        }
        #endregion

        #region 通用方法
        /// <summary>
        /// 获取HTTPClient
        /// </summary>
        /// <param name="dicHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public HttpClient GetHttpClient(Dictionary<string, string> dicHeaders = null, int timeoutSecond = 180)
        {
            return BuildHttpClient(dicHeaders, timeoutSecond);
        }
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dicHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> dicHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(dicHeaders, timeoutSecond);
                var response = await client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - HTTP请求失败，HTTP-Get:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="dicHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string url, string requestBody, Dictionary<string, string> dicHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(null, timeoutSecond);
                var requestContent = GenerateStringContent(requestBody, dicHeaders);
                var response = await client.PostAsync(url, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - HTTP请求失败，HTTP-Post:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="dicHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<T> PutAsync<T>(string url, string requestBody, Dictionary<string, string> dicHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(null, timeoutSecond);
                var requestContent = GenerateStringContent(requestBody, dicHeaders);
                var response = await client.PutAsync(url, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - HTTP请求失败，HTTP-Put:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Patch
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestString"></param>
        /// <param name="dicHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<T> PatchAsync<T>(string url, string requestBody, Dictionary<string, string> dicHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(null, timeoutSecond);
                var requestContent = GenerateStringContent(requestBody, dicHeaders);
                var response = await client.PatchAsync(url, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - HTTP请求失败，HTTP-Patch:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dicHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<T> DeleteAsync<T>(string url, Dictionary<string, string> dicHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(dicHeaders, timeoutSecond);
                var response = await client.DeleteAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - HTTP请求失败，HTTP-Delete:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// common request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="requestBody"></param>
        /// <param name="dicHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(string url, HttpMethod method, string requestBody, Dictionary<string, string> dicHeaders, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(null, timeoutSecond);
                var request = GenerateHttpRequestMessage(url, requestBody, method, dicHeaders);
                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return ConvertResponseContentToResult<T>(responseContent);
                }
                else
                {
                    throw new Exception("Http Code:" + response.StatusCode + ", Response:" + responseContent);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - HTTP请求失败，HTTP-Execute:{url} Error:{ex.Message}", ex);
                throw new Exception(ex.Message);
            }

        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// HTTP-Get请求
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="token">身份校验Token</param>
        /// <returns></returns>
        public async Task<TResult> HttpGetAsync<TResult>(string url, string token = null)
        {
            //请求头
            Dictionary<string, string> header = null;
            if (!string.IsNullOrEmpty(token))
            {
                header = new Dictionary<string, string>() { { "Authorization", token } };
            }

            return await GetAsync<TResult>(url, header);
        }
        /// <summary>
        /// HTTP-Post请求
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="obj">请求体</param>
        /// <param name="token">身份校验Token</param>
        /// <returns></returns>
        public async Task<TResult> HttpPostAsync<TResult>(string url, object obj = null, string token = null)
        {
            //请求头
            Dictionary<string, string> header = null;
            if (!string.IsNullOrEmpty(token))
            {
                header = new Dictionary<string, string>() { { "Authorization", token } };
            }

            //请求体
            string objStr = string.Empty;
            if (obj != null)
            {
                objStr = JsonConvert.SerializeObject(obj);
            }

            //返回结果
            return await PostAsync<TResult>(url, objStr, header);
        }
        #endregion

    }
}
