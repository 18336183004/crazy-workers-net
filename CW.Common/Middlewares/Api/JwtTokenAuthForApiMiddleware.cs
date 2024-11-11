using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Extensions.Configuration;
using CW.Common.JwtAuth;
using CW.Common.BASE;

namespace CW.Common.Middlewares.Api
{
    /// <summary>
    /// Token认证
    /// </summary>
    public class JwtTokenAuthForApiMiddleware
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
        /// 登录许可Key
        /// </summary>
        private readonly string LOGIN_PERMISSION_KEY;
        /// <summary>
        /// 路径过滤器
        /// 字符全部小写
        /// </summary>
        private readonly string[] WHITE_LIST;


        /// <summary>
        /// 中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="redis"></param>
        public JwtTokenAuthForApiMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            //LOGIN_PERMISSION_KEY = _configuration.GetSection("Redis").GetSection("Redis.Key.LoginPermission.Key").Value;
            //WHITE_LIST = _configuration.GetSection("WhiteList").GetSection("RequestFilter.WhiteList").Value?.ToLower()?.Split(",");
        }

        private void PreProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke preproceed");
        }
        private void PostProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke postproceed");
        }

        /// <summary>
        /// Token认证
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            PreProceed(httpContext);

            //健康检查
            if (httpContext.Request.Path.Value.ToLower().Contains("/home/healthcheck"))
            {
                PostProceed(httpContext);
                await _next(httpContext);
                return;
            }
            //无需token校验
            if (httpContext.Request.Path.Value.ToLower().Contains("/api/tokenpass"))
            {
                PostProceed(httpContext);
                await _next(httpContext);
                return;
            }
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            //白名单
            if (configuration.GetSection("WhiteList").GetSection("RequestFilter.WhiteList").Value.ToLower().Split(",").ToList().Contains(httpContext.Request.Path.Value.ToLower()))
            {
                PostProceed(httpContext);
                await _next(httpContext);
                return;
            }

            //是否包含Authorization请求头
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }

            //获取header中的JWTToken
            var token = httpContext.Request.Headers["Authorization"].ToString();

            //校验是否有Bearer前缀
            if(!token.Contains("Bearer "))
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }
            else
            {
                token = token.Replace("Bearer ", ""); //将Bearer去除
            }

            //jwttoken长度不能小于128
            if (token.Length < 128)
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }

            //解析JWT
            JwtSSOTokenModel jwt = SsoJwtHelper.SerializeJwt(token);

            //滑动过期时间下，令牌中的过期时间不能过期大于1天
            if ((jwt.Expires - DateTime.Now).TotalDays > 1)
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }

            // 是否过期 小于0表示未过期
            if (DateTime.Now.CompareTo(jwt.Expires) > 0)
            {
                await FailAuth(httpContext, GetExpirationTokenResponse());
                return;
            }
            //校验令牌
            if (!SsoJwtHelper.ValidateJwtToken(token))
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }

            //校验时间戳
            //string timestamp = await _redis.StringGetAsync(LOGIN_PERMISSION_KEY + jwt.PlayerId);
            //if (timestamp != jwt.Timestamp)
            //{
            //    await FailAuth(httpContext, GetInvalidTokenResponse());
            //    return;
            //}

            //如果身份不对，直接返回无效令牌
                ////用户信息Claims存入httpcontext.user中
                //var claims = new List<Claim> {
                //    new Claim("UserId", jwt.UserId.ToString()),
                //    new Claim("Account", jwt.Account),
                //    new Claim("Username", jwt.Username),
                //    new Claim("Avatar", jwt.Avatar),
                //    new Claim("Channel",jwt.Channel.ToString()),
                //    new Claim("Identity",jwt.Identity.ToString()),
                //    new Claim("Timestamp", jwt.Timestamp.ToString()),
                //};
                //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //var principal = new ClaimsPrincipal(identity);
                //httpContext.User = principal;

                PostProceed(httpContext);
                await _next(httpContext);
                return;
        }

        /// <summary>
        /// 校验失败
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="responseModel"></param>
        /// <returns></returns>
        private async Task FailAuth(HttpContext httpContext, ResponseModel responseModel)
        {
            httpContext.Response.StatusCode = 200;
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(responseModel));
        }
        /// <summary>
        /// 获取 过期令牌 响应
        /// </summary>
        /// <returns></returns>
        private ResponseModel GetExpirationTokenResponse()
        {
            ResponseModel response = new ResponseModel
            {
                Code = ResponseCode.ExpirationToken,
                Message = MessageModel.ExpirationToken
            };
            return response;
        }
        /// <summary>
        /// 获取 无效令牌 响应
        /// </summary>
        /// <returns></returns>
        private ResponseModel GetInvalidTokenResponse()
        {
            ResponseModel response = new ResponseModel
            {
                Code = ResponseCode.InvalidToken,
                Message = MessageModel.InvalidToken
            };
            return response;
        }
        /// <summary>
        /// 获取 无效身份 响应
        /// </summary>
        /// <returns></returns>
        private ResponseModel GetInvalidIdentityResponse()
        {
            ResponseModel response = new ResponseModel
            {
                Code = ResponseCode.InvalidToken,
                Message = MessageModel.InvalidIdentity
            };
            return response;
        }
    }
}