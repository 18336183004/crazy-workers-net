using Microsoft.AspNetCore.Builder;
using CW.Common.Middlewares.Api;

namespace CW.Common.Middlewares
{
    public static class MiddlewareHelpers
    {
        /// <summary>
        /// JWT认证-玩家
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtTokenAuthForApi(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtTokenAuthForApiMiddleware>();
        }
        /// <summary>
        /// 请求响应日志
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseReuestResponseLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLogMiddleware>();
        }
    }
}
