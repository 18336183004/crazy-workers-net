using Microsoft.AspNetCore.Builder;
using CW.Common.Middlewares.Api;

namespace CW.Common.Middlewares
{
    public static class MiddlewareHelpers
    {
        /// <summary>
        /// JWT��֤-���
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtTokenAuthForApi(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtTokenAuthForApiMiddleware>();
        }
        /// <summary>
        /// ������Ӧ��־
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseReuestResponseLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLogMiddleware>();
        }
    }
}
