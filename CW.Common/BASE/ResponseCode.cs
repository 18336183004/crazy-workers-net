using System.ComponentModel;

namespace CW.Common.BASE
{
    /// <summary>
    /// 系统响应
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 200,
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Error = 400,
        /// <summary>
        /// 授权登录
        /// </summary>
        [Description("授权登录")]
        AuthorizedLogin = 500,
        /// <summary>
        /// token过期
        /// </summary>
        [Description("无效令牌")]
        InvalidToken = 600,
        /// <summary>
        /// token过期
        /// </summary>
        [Description("令牌过期")]
        ExpirationToken = 601,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("警告")]
        Warn = 602,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 900
    }
}
