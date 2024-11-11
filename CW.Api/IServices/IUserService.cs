using CW.Api.Models;
using CW.Common.BASE;

namespace CW.Api.IServices
{
    public interface IUserService
    {
        /// <summary>
        /// 用户授权
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> Authorisation(LoginUserModel model);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> GetUserInfo();

        /// <summary>
        /// 获取玩家收藏列表
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> GetUserCollectList();

        /// <summary>
        /// 玩家穿戴收藏装扮
        /// </summary>
        /// <param name="collectId"></param>
        /// <returns></returns>
        public Task<ResponseModel> UserWearCollect(int collectId);

        /// <summary>
        /// 获取玩家荣誉表
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> GetUserHonorList();

        /// <summary>
        /// 玩家穿戴荣誉
        /// </summary>
        /// <param name="honorId"></param>
        /// <returns></returns>
        public Task<ResponseModel> UserWearHonor(int honorId);

        /// <summary>
        /// 添加游玩记录
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> AddRecord();

        /// <summary>
        /// 结束本局游戏
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> EndThisGame(EndThisGameModel model);

        /// <summary>
        /// 获取省份排行
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> GetProvinceRanking();

        /// <summary>
        /// 查询邀请申请
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> InvitationRecord();
        /// <summary>
        /// 领取邀请奖励
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> ReceiveInvitation();

        /// <summary>
        /// 查询职级排行
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> GetPositionLevelRanking();

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> GetToBenotifiedList();

        /// <summary>
        /// 修改消息通知为已读
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel>  EditToBenotified(EditToBenotifiedModel model);

        /// <summary>
        /// 新增消息通知
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<ResponseModel> AddToBenotified(AddToBenotifiedModel model);

        /// <summary>
        /// 获取玩家道具
        /// </summary>
        /// <returns></returns>
        public Task<ResponseModel> GetUserProp();
    }
}
