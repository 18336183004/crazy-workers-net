using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CW.Common.Attributes;
using CW.Common.BASE;
using CW.Common.Converters;
using CW.Common.Cryptions;
using CW.Common.Logs;
using System.Security.Principal;

namespace CW.Common.JwtAuth
{
    public class SsoJwtHelper
    {
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string IssueJwt(JwtSSOTokenModel tokenModel, IConfiguration configuration)
        {
            try
            {
                var claims = new List<Claim>
                {
                 /*
                   1、这里将用户的部分信息，比如 uid 存到了Claim 中，如果你想知道如何在其他地方将这个 uid从 Token 中取出来，请看下边的SerializeJwt()方法的调用。
                   2、你也可以研究下 HttpContext.User.Claims
                 */
                 new Claim("UserId", tokenModel.UserId),
                 new Claim("OpenId", tokenModel.OpenId),
                //new Claim("Account", tokenModel.Account),
                //new Claim("Username", tokenModel.Username),
                
                //new Claim("Cipher", AESEncryptHelper.RijndaelEncrypt(tokenModel.OpenId, configuration.GetSection("SSOToken").GetSection("SSOToken.AESSecret").Value)),
                new Claim("Cipher", AESEncryptHelper.RijndaelEncrypt(tokenModel.OpenId, "ranwen2020wxjndaelzekf884knywill")),
                // 过期时间可自定义，注意JWT有自己的缓冲过期时间
                //new Claim("Expires", DateTime.Now.AddSeconds(configuration.GetSection("SSOToken").GetSection("SSOToken.Expires").Value.ToInt())
                        //.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo)),

                 new Claim("Expires", DateTime.Now.AddSeconds(86400)
                        .ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo)),

                //new Claim(JwtRegisteredClaimNames.Iat,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                //new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                //new Claim(JwtRegisteredClaimNames.Iss,Define.ISSUER),
                //new Claim(JwtRegisteredClaimNames.Aud,Define.AUDIENCE),
                // 为了解决一个用户多个角色(比如：Admin,System)，用下边的方法
                //new Claim(ClaimTypes.Role,tokenModel.Role)
               };
                // 可以将一个用户的多个角色全部赋予
                //if(tokenModel.Role != null)
                //    claims.AddRange(tokenModel.Role?.Split('|').Select(s => new Claim(ClaimTypes.Role, s)));

                //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Define.SSO_TOKEN_SECRET));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwt = new JwtSecurityToken(
                    issuer: Define.ISSUER,
                    audience: Define.AUDIENCE,
                    claims: claims,
                    signingCredentials: creds,
                    //expires: DateTime.Now.AddSeconds(configuration.GetSection("SSOToken").GetSection("SSOToken.Expires").Value.ToInt() + 60) //冗余60秒
                    expires: DateTime.Now.AddSeconds(86400 + 60) //冗余60秒
                    );

                var jwtHandler = new JwtSecurityTokenHandler();
                var encodedJwt = jwtHandler.WriteToken(jwt);

                return encodedJwt;
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 生成SSOToken失败，{ex.Message}", ex);
                throw new UserOperationException("生成Token失败!");
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static JwtSSOTokenModel SerializeJwt(string jwtStr)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();

                // 增加日志记录JWT字符串（注意：在实际应用中，避免记录敏感信息）
                NLogHelper.DebugLog($"Attempting to parse JWT: {jwtStr}");

                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

                // 从Payload中提取数据
                jwtToken.Payload.TryGetValue("UserId", out object userId);
                jwtToken.Payload.TryGetValue("OpenId", out object openId);
                jwtToken.Payload.TryGetValue("Cipher", out object cipher);
                jwtToken.Payload.TryGetValue("Expires", out object expires);

                var jwt = new JwtSSOTokenModel
                {
                    UserId = userId?.ToString() ?? string.Empty,
                    OpenId = openId?.ToString() ?? string.Empty,
                    Cipher = cipher?.ToString() ?? string.Empty,
                    Expires = expires != null ? expires.ToDate() : DateTime.MinValue,
                };

                return jwt;
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 解析Token失败，{ex.Message}", ex);
                throw new UserOperationException("解析Token失败，请检查JWT格式！");
            }
        }

        //public static JwtSSOTokenModel SerializeJwt(string jwtStr)
        //{
        //    try
        //    {
        //        var jwtHandler = new JwtSecurityTokenHandler();

        //        JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

        //        jwtToken.Payload.TryGetValue("UserId", out object userId);
        //        jwtToken.Payload.TryGetValue("OpenId", out object openId);
        //        //jwtToken.Payload.TryGetValue("Account", out object account);
        //        //jwtToken.Payload.TryGetValue("Username", out object username);

        //        jwtToken.Payload.TryGetValue("Cipher", out object cipher);
        //        jwtToken.Payload.TryGetValue("Expires", out object expires);
        //        var jwt = new JwtSSOTokenModel
        //        {
        //            UserId = userId != null ? userId.ToString() : string.Empty,
        //            OpenId = openId != null ? openId.ToString() : string.Empty,
        //            //Account = account != null ? account.ToString() : string.Empty,
        //            //Username = username != null ? username.ToString() : string.Empty,

        //            Cipher = cipher != null ? cipher.ToString() : string.Empty,
        //            Expires = expires != null ? expires.ToDate() : DateTime.MinValue,
        //        };
        //        return jwt;
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 解析Token失败，{ex.Message}", ex);
        //        throw new UserOperationException("解析Token失败!");
        //    }

        //}

        /// <summary>
        /// 校验JWTToken
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static bool ValidateJwtToken(string jwtStr)
        {
            try
            {
                JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
                jwtHandler.ValidateToken(jwtStr, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Define.SSO_TOKEN_SECRET)),
                    ValidIssuer = Define.ISSUER,
                    ValidateIssuer = true,
                    ValidAudience = Define.AUDIENCE,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.FromSeconds(30), //默认300
                }, out _);

                return true;
            }
            catch (Exception ex)
            {
                NLogHelper.ErrorLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 校验JWTToken失败，{ex.Message}", ex);
                return false;
            }
        }
    }
}