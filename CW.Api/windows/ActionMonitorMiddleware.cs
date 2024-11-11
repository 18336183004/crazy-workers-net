using Microsoft.AspNetCore.Authentication;

namespace CW.Api.windows
{
    /// <summary>
    /// WebAPI Action监控
    /// </summary>
    public class ActionMonitorMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next">下一个处理者</param>
        public ActionMonitorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //header('Access-Control-Allow-Origin: *');  // 允许所有域跨域请求，生产环境可将 '*' 改为具体的域名
            //header('Access-Control-Allow-Methods: GET, POST, OPTIONS');  // 允许的请求方法
            //header('Access-Control-Allow-Headers: token, Content-Type, Authorization');  // 允许的请求头

            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization,*");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,OPTIONS");
            if (context.Request.Method.ToUpper() == "OPTIONS")
            {
                return;
            }
            await _next(context);
        }
    }
}
