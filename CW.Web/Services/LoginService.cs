using CW.Web.Services.BASE;
using CW.Framework.Entities;
using CW.Web.IServices;
using CW.Web.Models;
using Microsoft.AspNetCore.Mvc;
using CW.Common.BASE;

namespace CW.Web.Services
{
    public class LoginService : BaseService, ILoginService
    {
        public LoginService() : base()
        {

        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> LoginAccount(LoginAccountModel model)
        {
            ResponseModel response = new();
            var user = await _dbContext.Queryable<account>().Where(c => c.UserName == model.Username && c.PassWord == model.Password).FirstAsync();
            if(user == null)
            {
                response.Code = ResponseCode.Success;
            }
            else
            {
                response.Code = ResponseCode.Error;
            }
            return response;
        }
    }
}
