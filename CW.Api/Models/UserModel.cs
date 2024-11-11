using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace CW.Api.Models
{
    public class WeChatModel
    {
        [Required(ErrorMessage = "jscode不能为空!")]
        public string? Code { get; set; }
        /// <summary>
        /// 邀请人
        /// </summary>
        public string? InviterId { get; set; }
    }

    public class LoginModel
    {
        /// <summary>
        /// 微信openId
        /// </summary>
        [Required(ErrorMessage = "微信号不能为空!")]
        public string OpenId { get; set; }
        /// <summary>
        /// 邀请人
        /// </summary>
        public string? InviterId { get; set; }
    }

    public class LoginUserModel
    {
        /// <summary>
        /// 微信openId
        /// </summary>
        [Required(ErrorMessage = "微信号不能为空!")]
        public string? OpenId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空!")]
        public string? Name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        //[Required(ErrorMessage = "手机号不能为空!")]
        public string? Tel { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [Required(ErrorMessage = "头像不能为空!")]
        public string? Picture { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        //[Required(ErrorMessage = "城市不能为空!")]
        public string? City { get; set; }
    }
    public class SsoTokenInfoModel
    {
        /// <summary>
        /// ssoToken
        /// </summary>
        public string? SsoToken { get; set; }
        /// <summary>
        /// 天
        /// </summary>
        public int? Day { get; set; }

        public string OpenId { get; set; }
    }

    public class UserInfoModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string? Tel { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string? Picture { get; set; }
        /// <summary>
        /// 装扮Id
        /// </summary>
        public long? CollectId { get; set; }
        /// <summary>
        /// 装扮
        /// </summary>
        public string? Collect { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? FullBodyPhoto { get; set; }
        /// <summary>
        /// 职级Id
        /// </summary>
        public long? PositionLevelId { get; set; }
        /// <summary>
        /// 职级
        /// </summary>
        public string? PositionLevelName { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string? Area { get; set; }
        /// <summary>
        /// 上班天数
        /// </summary>
        public int Day { get; set; }
    }

    /// <summary>
    /// 地图信息
    /// </summary>
    public class DateModel
    {
        /// <summary>
        /// 地图Id
        /// </summary>
        //public string Id { get; set; }
        /// <summary>
        /// 难度等级 1低级 2中级 3高级 4困难 5地狱
        /// </summary>
        //public string DifficultyLevel { get; set; }
        /// <summary>
        /// 地图类型 1(4*7) 2(5*8) 3(6*12) 4(7*11)
        /// </summary>
        public string? MapDataBusTypes { get; set; }
        /// <summary>
        /// 地图数据
        /// </summary>
        public string? MapData { get; set; }
        /// <summary>
        /// 乘客排序颜色
        /// </summary>
        public string? MapDataRoloQueues { get; set; }
        /// <summary>
        /// 可坐的乘客颜色
        /// </summary>
        public string? MapDataGraySitDownColor { get; set; }
        /// <summary>
        /// 通关时间
        /// </summary>
        public string? MapDataCountDown { get; set; }
    }

    /// <summary>
    /// 玩家收藏
    /// </summary>
    public class UserCollectModel
    {
        /// <summary>
        /// 收藏Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 收藏名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 收藏图片
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// 全身照
        /// </summary>
        public string FullBodyPhoto { get; set; }
        /// <summary>
        /// 职级Id
        /// </summary>
        public long PositionLevelId { get; set; }
        /// <summary>
        /// 职级名称
        /// </summary>
        public string PositionLevelIName { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public string Condition { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否穿戴 0未穿戴 1穿戴
        /// </summary>
        public byte IsWear { get; set; }
        /// <summary>
        /// 是否持有 0未持有 1持有
        /// </summary>
        public byte IsHold { get; set; }
    }

    public class UserCollectData
    {
        /// <summary>
        /// 收藏列表
        /// </summary>
        public List<UserCollectModel>? UserCollects { get; set; }
        /// <summary>
        /// 持有数
        /// </summary>
        public int HoldNum { get; set; }
        /// <summary>
        ///总数
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// 玩家荣誉
    /// </summary>
    public class UserHonorModel
    {
        /// <summary>
        /// 荣誉Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 荣誉名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 荣誉图片
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否穿戴 0未穿戴 1穿戴
        /// </summary>
        public byte IsWear { get; set; }
        /// <summary>
        /// 是否持有 0未持有 1持有
        /// </summary>
        public byte IsHold { get; set; }
    }

    public class UserHonorData
    {
        public List<UserHonorModel>? List1 { get; set; }
        public List<UserHonorModel>? List2 { get; set; }
        public List<UserHonorModel>? List3 { get; set; }
        /// <summary>
        /// 穿戴荣誉
        /// </summary>
        public UserHonorModel? UserHonor { get; set; }
    }

    public class EndThisGameModel
    {
        /// <summary>
        /// 结果 0失败 1成功
        /// </summary>
        [Required(ErrorMessage = "类型不能为空!")]
        public byte Type { get; set; }
        /// <summary>
        /// 关卡类型 0正常关卡 1加班关卡
        /// </summary>
        [Required(ErrorMessage = "关卡类型不能为空!")]
        public byte Barrier { get; set; }

    }

    public class RecordModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 是否上班成功 0未成功 1成功
        /// </summary>
        public byte IsSuccess { get; set; }
    }


    /// <summary>
    /// 省份排行
    /// </summary>
    public class ProvinceRankingModel
    {
        /// <summary>
        /// 省份Id
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 上班人数
        /// </summary>
        public int? TotalNumber { get; set; }
        /// <summary>
        /// 出勤率
        /// </summary>
        public double? Attendance { get; set; }
    }

    public class ProvinceRankingData
    {
        public List<ProvinceRankingModel>? ProvinceRankings { get; set; }

        public long? UserProvinceId { get; set; }
    }

    public class PositionLevelRankingModel
    {
        /// <summary>
        /// 职级Id
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 职级名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int? TotalNumber { get; set; }
        /// <summary>
        /// 玩家信息
        /// </summary>
        public List<PositionLevelUserModel> List { get; set; }
    }

    public class PositionLevelUserModel
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string? Picture { get; set; }
        /// <summary>
        /// 装扮
        /// </summary>
        public long CollectId { get; set; }
        /// <summary>
        /// 装扮图片
        /// </summary>
        public string? CollectPicture { get; set; }
        /// <summary>
        /// 全身照
        /// </summary>
        public string? FullBodyPhoto { get; set; }
    }
    public class ToBeNotifiedModel
    {
        public PositionLevelModel? PositionLevel { get; set; }

        public List<CollectModel>? Collects { get; set; }

        public List<HonorModel>? Honors { get; set; }
    }

    public class PositionLevelModel
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Picture { get; set; }
    }
    public class CollectModel
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Picture { get; set; }
        public string? FullBodyPhoto { get; set; }
    }
    public class HonorModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Picture { get; set; }
    }

    public class ToBenotifiedModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 状态 0失去 1获得
        /// </summary>
        public byte State { get; set; }
        /// <summary>
        /// 类型 0职级 1装扮 2荣誉
        /// </summary>
        public byte Type { get; set; }
        /// <summary>
        /// 数据 json
        /// </summary>
        public string Data { get; set; }
    }

    public class EditToBenotifiedModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空!")]
        public long Id { get; set; }
    }
    public class AddToBenotifiedModel
    {
        /// <summary>
        /// 状态 0失去 1获得
        /// </summary>
        [Required(ErrorMessage = "状态不能为空!")]
        public byte State { get; set; }
        /// <summary>
        /// 类型 0职级 1装扮 2荣誉
        /// </summary>
        [Required(ErrorMessage = "类型不能为空!")]
        public byte Type { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空!")]
        public long Id { get; set; }
    }

    public class DailyRecordModel
    {
        public int Total { get; set; }
        public int CompletedQuantity { get; set; }
    }

    public class UserPropModel
    {
        /// <summary>
        /// 道具数量
        /// </summary>
        public int Prop1 { get; set; }
        /// <summary>
        /// 道具数量
        /// </summary>
        public int Prop2 { get; set; }
        /// <summary>
        /// 道具数量
        /// </summary>
        public int Prop3 { get; set; }
    }
    public class UserInfo
    {
        /// <summary>
        /// 职级Id
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 职级名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 总上班天数
        /// </summary>
        public int? WorkingDays { get; set; }
    }

    public class RewardModel
    {
        /// <summary>
        /// 装扮 0未获取 1已获取
        /// </summary>
        public int Reward { get; set; }
        /// <summary>
        /// 道具
        /// </summary>
        public int Reward1 { get; set; }
        /// <summary>
        /// 邀请数量
        /// </summary>
        public int Num { get; set; }
    }
}
