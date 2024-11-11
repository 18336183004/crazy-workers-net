using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using CW.Common.Logs;
using CW.Common.JwtAuth;

namespace CW.Common.Middlewares
{
    /// <summary>
    /// 请求和响应
    /// </summary>
    public class RequestLogMiddleware
    {
        /// <summary>
        /// 委托
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 配置获取
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 路径过滤器
        /// 字符全部小写
        /// </summary>
        //private readonly string[] _requestUrl;

        /// <summary>
        /// 中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        public RequestLogMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            //_requestUrl = _configuration.GetValue<string>("RequestFilter.WhiteList")?.ToLower()?.Split(",");
        }

        /// <summary>
        /// 通道中间件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            //健康检查
            if (context.Request.Path.Value.ToLower().Contains("/home/healthcheck"))
            {
                await _next(context);
                return;
            }
            //拦截API接口
            if (context.Request.Path.Value.ToLower().Contains("/api"))
            {
                context.Request.EnableBuffering();

                Stream originalBody = context.Response.Body;

                try
                {
                    // 请求
                    await RequestDataLog(context.Request);

                    using (var ms = new MemoryStream())
                    {
                        context.Response.Body = ms;

                        await _next(context);

                        try
                        {
                            // 响应
                            ResponseDataLog(context.Response, ms);

                            ms.Position = 0;
                            await ms.CopyToAsync(originalBody);
                        }
                        catch (Exception ex)
                        {
                            // 记录
                            NLogHelper.ErrorLog(context.Response.ToString(), ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 记录
                    NLogHelper.ErrorLog(context.Response.ToString(), ex);
                    await _next(context);
                }
                finally
                {
                    context.Response.Body = originalBody;
                }
            }
            else
            {
                await _next(context);
                return;
            }
        }

        /// <summary>
        /// 访问
        /// </summary>
        /// <param name="request"></param>
        private async Task RequestDataLog(HttpRequest request)
        {
            try
            {
                var sr = new StreamReader(request.Body);
                var content = $"QueryData:{request.Path + request.QueryString}\r\n" +
                              $"BodyData:{await sr.ReadToEndAsync()}\r\n" +
                              $"Operator:{GetRequestOperatorByToken(request)}\r\n" +
                              $"RequestPath:{request.GetDisplayUrl()}\r\n";

                if (!string.IsNullOrEmpty(content))
                {
                    Parallel.For(0, 1, e =>
                    {
                        LogLock log = new LogLock();
                        LogLock.OutSql2Log("RequestResponseLog", new string[] { "Request Data:", content });

                    });

                    request.Body.Position = 0;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 响应
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ms"></param>
        private void ResponseDataLog(HttpResponse response, MemoryStream ms)
        {
            try
            {
                ms.Position = 0;
                var ResponseBody = new StreamReader(ms).ReadToEnd();

                if (!string.IsNullOrEmpty(ResponseBody))
                {
                    Parallel.For(0, 1, e =>
                    {
                        LogLock log = new LogLock();
                        LogLock.OutSql2Log("RequestResponseLog", new string[] { "Response Data:", ResponseBody });
                    });
                }

            }
            catch (Exception) { }
        }
        /// <summary>
        /// 获取token中的请求操作人信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GetRequestOperatorByToken(HttpRequest request)
        {
            string operatorName = null;
            if (request.Headers.ContainsKey("Authorization"))
            {
                try
                {
                    string token = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        JwtSSOTokenModel jwtToken = SsoJwtHelper.SerializeJwt(token);
                        operatorName = $"[{jwtToken.UserId}]-[{jwtToken.OpenId}]";
                    }
                }
                catch (Exception ex)
                {
                    NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - 解析token失败!", ex);
                }
            }
            return operatorName;
        }
    }
}

