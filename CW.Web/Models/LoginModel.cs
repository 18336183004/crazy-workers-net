using System.ComponentModel.DataAnnotations;

namespace CW.Web.Models
{
    public class LoginAccountModel
    {
        [Required(ErrorMessage = "账号不能为空!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "密码不能为空!")]
        public string Password { get; set; }
    }
}
