using System.ComponentModel.DataAnnotations;
using CW.Api.Controllers.BASE;
using CW.Api.IServices;
using CW.Api.Models;
using CW.Common.BASE;
using CW.Framework.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CW.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// 用户授权
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> Authorisation([FromBody] LoginUserModel model)
        {
            return await _userService.Authorisation(model);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserInfoModel), 200)]
        public async Task<ResponseModel> GetUserInfo()
        {
            return await _userService.GetUserInfo();
        }

        /// <summary>
        /// 获取玩家收藏列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserCollectData), 200)]
        public async Task<ResponseModel> GetUserCollectList()
        {
            return await _userService.GetUserCollectList();
        }

        /// <summary>
        /// 玩家穿戴收藏装扮
        /// </summary>
        /// <param name="collectId">收藏装扮Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> UserWearCollect([Required(ErrorMessage = "装扮不能为空!")][FromQuery] int collectId)
        {
            return await _userService.UserWearCollect(collectId);
        }

        /// <summary>
        /// 获取玩家荣誉表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserHonorData), 200)]
        public async Task<ResponseModel> GetUserHonorList()
        {
            return await _userService.GetUserHonorList();
        }

        /// <summary>
        /// 玩家穿戴荣誉
        /// </summary>
        /// <param name="honorId">荣誉Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> UserWearHonor([Required(ErrorMessage = "荣誉不能为空!")][FromQuery] int honorId)
        {
            return await _userService.UserWearHonor(honorId);
        }

        /// <summary>
        /// 添加登录游玩记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> AddRecord()
        {
            return await _userService.AddRecord();
        }

        /// <summary>
        /// 结束本局游戏
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserInfo), 200)]
        public async Task<ResponseModel> EndThisGame([FromBody] EndThisGameModel model)
        {
            return await _userService.EndThisGame(model);
        }

        /// <summary>
        /// 获取省份排行
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProvinceRankingData), 200)]
        public async Task<ResponseModel> GetProvinceRanking()
        {
            return await _userService.GetProvinceRanking();
        }

        /// <summary>
        /// 查询邀请申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(RewardModel), 200)]
        public async Task<ResponseModel> InvitationRecord()
        {
            return await _userService.InvitationRecord();
        }

        /// <summary>
        /// 领取邀请奖励
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> ReceiveInvitation()
        {
            return await _userService.ReceiveInvitation();
        }

        /// <summary>
        /// 查询职级排行
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(PositionLevelRankingModel), 200)]
        public async Task<ResponseModel> GetPositionLevelRanking()
        {
            return await _userService.GetPositionLevelRanking();
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ToBenotifiedModel), 200)]
        public async Task<ResponseModel> GetToBenotifiedList()
        {
            return await _userService.GetToBenotifiedList();
        }

        /// <summary>
        /// 修改消息通知为已读
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> EditToBenotified([FromBody] EditToBenotifiedModel model)
        {
            return await _userService.EditToBenotified(model);
        }

        /// <summary>
        /// 新增消息通知
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(AddToBenotifiedModel), 200)]
        public async Task<ResponseModel> AddToBenotified([FromBody] AddToBenotifiedModel model)
        {
            return await _userService.AddToBenotified(model);
        }

        /// <summary>
        /// 获取玩家道具
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserPropModel), 200)]
        public async Task<ResponseModel> GetUserProp()
        {
            return await _userService.GetUserProp();
        }
    }
}
