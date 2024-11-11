using System;
using System.Text;

namespace CW.Common.SMS
{
    /// <summary>
    /// 短信发送帮助类
    /// </summary>
    public class SMSHelper
    {
        /// <summary>
        /// 生成短信验证码
        /// </summary>
        /// <param name="digit">验证码位数</param>
        /// <returns></returns>
        public static string BuildIdentifyingCode(int digit = 4, bool isContainLetter = false)
        {
            string[] source = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string[] letter = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z" };
            StringBuilder code = new StringBuilder();
            Random rd = new Random();
            for (int i = 0; i < digit; i++)
            {
                if (isContainLetter)
                    code.Append(letter[rd.Next(0, letter.Length)]);
                else
                    code.Append(source[rd.Next(0, source.Length)]);
            }
            return code.ToString();
        }
    }
}
