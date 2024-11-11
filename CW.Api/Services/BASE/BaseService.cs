using SqlSugar.IOC;
using SqlSugar;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Net.Http;

namespace CW.Api.Services.BASE
{
    /// <summary>
    /// 服务层公用部分
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        protected long UserId { get; set; }
        /// <summary>
        /// 玩家openId
        /// </summary>
        protected string OpenId { get; set; }
        /// <summary>
        /// SugarScope
        /// </summary>
        protected SqlSugarScope _dbContext = DbScoped.SugarScope;

        public BaseService() { }

        public BaseService(IHttpContextAccessor context)
        {
            if (context?.HttpContext?.User?.Claims?.Count() > 0)
            {
                UserId = Convert.ToInt64(context.HttpContext.User.Claims.First(c => c.Type == "UserId").Value);
                OpenId = context.HttpContext.User.Claims.First(c => c.Type == "OpenId").Value;
                //Account = Convert.ToInt64(context.HttpContext.User.Claims.First(c => c.Type == "Account").Value);
                //Username = context.HttpContext.User.Claims.First(c => c.Type == "Username").Value;
                //RegionId = context.HttpContext.User.Claims.First(c => c.Type == "RegionId").Value;
            }
        }
    }
}
