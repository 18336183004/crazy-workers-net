using SqlSugar.IOC;
using SqlSugar;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;

namespace CW.Web.Services.BASE
{
    /// <summary>
    /// 服务层公用部分
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        protected string PlayerId { get; set; }
        /// <summary>
        /// 账号(手机号)
        /// </summary>
        protected long Account { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        protected string Username { get; set; }
        /// <summary>
        /// 区服Id
        /// </summary>
        protected string RegionId { get; set; }
        /// <summary>
        /// SugarScope
        /// </summary>
        protected SqlSugarScope _dbContext = DbScoped.SugarScope;

        public BaseService() { }

        public BaseService(IHttpContextAccessor context)
        {
            if (context?.HttpContext?.User?.Claims?.Count() > 0)
            {
                PlayerId = context.HttpContext.User.Claims.First(c => c.Type == "PlayerId").Value;
                Account = Convert.ToInt64(context.HttpContext.User.Claims.First(c => c.Type == "Account").Value);
                Username = context.HttpContext.User.Claims.First(c => c.Type == "Username").Value;
                RegionId = context.HttpContext.User.Claims.First(c => c.Type == "RegionId").Value;
            }
        }
    }
}
