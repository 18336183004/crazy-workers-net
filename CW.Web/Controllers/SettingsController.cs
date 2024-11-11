using Microsoft.AspNetCore.Mvc;

namespace CW.Web.Controllers
{
    /// <summary>
    /// 配置管理
    /// </summary>
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;

        public SettingsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


    }
}
