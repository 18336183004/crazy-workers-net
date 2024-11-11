using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CW.Common.Cryptions
{
    public class Md5Helper
    {
        /// <summary>
        /// MD5(32位加密小写)
        /// </summary>
        /// <param name="inputStr">需要加密的字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string ToMd5(string inputStr)
        {
            return ToMd5(inputStr, Encoding.UTF8);
        }

        /// <summary>
        /// MD5(32位加密小写)
        /// </summary>
        /// <param name="inputStr">需要加密的字符串</param>
        /// <param name="e">字符集</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string ToMd5(string inputStr, Encoding e)
        {
            string pwd = string.Empty;
            //实例化一个md5对像
            var md5 = MD5.Create();
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(e.GetBytes(inputStr));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x").PadLeft(2, '0');
            }
            return pwd;
        }
    }
}
