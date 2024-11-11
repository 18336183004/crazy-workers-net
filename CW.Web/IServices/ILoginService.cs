using CW.Common.BASE;
using CW.Framework.Entities;
using CW.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CW.Web.IServices
{
    public interface ILoginService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<ResponseModel> LoginAccount(LoginAccountModel model);
    }
}
