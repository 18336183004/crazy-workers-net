using System.Net;
using System.Text;
using CW.Api.Controllers.BASE;
using CW.Api.IServices;
using CW.Api.Models;
using CW.Common.Attributes;
using CW.Common.BASE;
using CW.Common.JwtAuth;
using CW.Common.WeChat;
using CW.Framework.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Bmp;
using Newtonsoft.Json;
using System;
using CW.Api.Services;
using System.Xml;
using NLog.Fluent;
using System.IO;

namespace CW.Api.Controllers
{
    [Route("api/tokenpass/[controller]/[action]")]
    public class LoginController : BaseController
    {
        private readonly ILoginService _loginService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public LoginController(ILoginService loginService, IConfiguration configuration, IWebHostEnvironment env)
        {
            _loginService = loginService;
            _configuration = configuration;
            _env = env;
        }
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(SsoTokenInfoModel), 200)]
        public async Task<ResponseModel> UserInfo([FromBody] WeChatModel model)
        {
            // 替换为你的AppID和AppSecret  
            var appId = Define.APP_ID;
            var appSecret = Define.APP_SECRET;

            var weChatApiHelper = new WeChatApiHelper(appId, appSecret);
            var sessionInfo = await weChatApiHelper.GetSessionKeyAsync(model.Code);
            if (sessionInfo != null)
            {
                LoginModel login = new()
                {
                    OpenId = sessionInfo.openid,
                    InviterId = model.InviterId
                };

                var data = await Login(login);

                //创建用户
                return Success(data.Data);
            }
            return Success();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //#if !DEBUG
        //                [NonAction]
        //#endif
        [HttpPost]
        [ProducesResponseType(typeof(LoginModel), 200)]
        public async Task<ResponseModel> Login([FromBody] LoginModel model)
        {
            bool i = false;
            var user = await _loginService.GetUserinfo(model.OpenId);
            if (user == null)
            {
                var result1 = await _loginService.LoginAccount(model);
                if (result1?.Code != ResponseCode.Success)
                {
                    throw new UserOperationException("用户登录失败!");
                }
                else
                {
                    i = true;
                    user = await _loginService.GetUserinfo(model.OpenId);
                }
            }
            var result = await _loginService.Login(model, user);
            if (result.Code == ResponseCode.Success)
            {
                //生成JwtToken
                JwtSSOTokenModel tokenModel = new()
                {
                    UserId = user.Id.ToString(),
                    OpenId = user.OpenId,
                    //Account = user.Tel,
                    //Username = user.Name,
                };
                SsoTokenInfoModel ssoTokenInfo = new()
                {
                    SsoToken = SsoJwtHelper.IssueJwt(tokenModel, _configuration),
                    Day = i ? 15 : Convert.ToInt32(result.Data),
                    OpenId = model.OpenId,
                };
                return Success(ssoTokenInfo);
            }
            return ResponseModel.Success();
        }

        /// <summary>
        /// 获取地图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> Map()
        {
            return await _loginService.Map();
        }

        /// <summary>
        /// 每日总数记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(DailyRecordModel), 200)]
        public async Task<ResponseModel> DailyRecord()
        {
            return await _loginService.DailyRecord();
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> Get(string latitude, string longitude)
        {
            Url = @"http://api.map.baidu.com/reverse_geocoding/v3/?ak=" + ak + "&output=json&coordtype=bd09ll&location=" + latitude + "," + longitude;

            HttpWebRequest  request = (HttpWebRequest)WebRequest.Create(Url);
            request.KeepAlive = false;
            request.Method = "GET";
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var myResponseStream = response.GetResponseStream();
            var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            response.Close();
            request.Abort();

            return Success(retString);
        }

        //[HttpGet]
        //public ResponseModel Login1(string tel)
        //{

        //    return Success(UserPasswordCryptionHelper.EncryptPassword(tel));
        //}
        //[HttpGet]
        //public ResponseModel Login2(string tel, string iv)
        //{

        //    return Success(UserPasswordCryptionHelper.DecryptPassword1(tel, iv));
        //}
    }
}
