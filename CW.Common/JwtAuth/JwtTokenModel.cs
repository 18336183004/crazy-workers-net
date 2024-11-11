using System;
using System.Collections.Generic;
using System.Text;

namespace CW.Common.JwtAuth
{
    /// <summary>
    /// BNS令牌
    /// </summary>
    public class JwtBNSTokenModel
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        public string PlayerId { get; set; }
        /// <summary>
        /// 账号(手机号)
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 区服Id
        /// </summary>
        public string RegionId { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 密文
        /// </summary>
        public string Cipher { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expires { get; set; }
    }

    /// <summary>
    /// SSO令牌
    /// </summary>
    public class JwtSSOTokenModel
    {
        /// <summary>
        /// 玩家Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 玩家openId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 账号(手机号)
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密文
        /// </summary>
        public string Cipher { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expires { get; set; }
    }
}
