using System.Text.Json;
using CW.Common.Logs;


namespace CW.Common.WeChat
{
    public class WeChatApiHelper
    {
        private readonly string _appId;
        private readonly string _appSecret;

        public WeChatApiHelper(string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
        }

        public async Task<SessionResponse?> GetSessionKeyAsync(string jsCode)
        {
            using var client = new HttpClient();
            var tokenUrl = $"https://api.weixin.qq.com/sns/jscode2session?appid={_appId}&secret={_appSecret}&js_code={jsCode}&grant_type=authorization_code";

            try
            {
                HttpResponseMessage response = await client.GetAsync(tokenUrl);
                string jsonString = await response.Content.ReadAsStringAsync();
          
                NLogHelper.ErrorLog("访问微信api返回参数：" + jsonString + "/n");
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = $"Error calling WeChat API: {response.StatusCode}, {jsonString}";
                    NLogHelper.ErrorLog(errorMessage);
                    // 可以考虑抛出异常或记录日志
                    return null;
                }

                var sessionResponse = JsonSerializer.Deserialize<SessionResponse>(jsonString);
                NLogHelper.ErrorLog("访问微信api返回参数：sessionResponse" + sessionResponse + "/n");
                if (sessionResponse != null && !string.IsNullOrEmpty(sessionResponse.session_key))
                {
                    return sessionResponse;
                }
                return null;
            }
            catch (HttpRequestException e)
            {
                NLogHelper.ErrorLog("\nException Caught!报错：" + e.Message);
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
        // 定义一个简单的类来映射响应JSON  
        public class SessionResponse
        {
            public string? openid { get; set; }
            public string? session_key { get; set; }
        }
    }
}