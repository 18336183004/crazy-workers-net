using CW.Api.Models;
using CW.Common.BASE;
using CW.Common.JwtAuth;
using CW.Framework.Entities;

namespace CW.Api.IServices
{
    public interface ILoginService
    {
        /// <summary>
        /// 获取玩家信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public Task<user> GetUserinfo(string openId);

        public Task<ResponseModel> Login(LoginModel model,user user);

        public Task<ResponseModel> LoginAccount(LoginModel model);

        public Task<ResponseModel> EditAccount(string openId,int bay);

        public Task<ResponseModel> Map();

        /// <summary>
        /// 查询职级名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<PositionLevelModel> GetPositionLevelName(int id);

        /// <summary>
        /// 每日总数记录
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> DailyRecord();
    }
}
