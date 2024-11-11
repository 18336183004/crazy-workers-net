using Microsoft.AspNetCore.Mvc;

namespace CW.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        }

        /// <summary>
        /// 框架和左侧菜单
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 无权限提示
        /// </summary>
        /// <returns></returns>
        public IActionResult NoAuthorization()
        {
            ViewData["msg"] = "您没有当前操作的权限！";
            return View();
        }

        /// <summary>
        /// 控制台
        /// </summary>
        /// <returns></returns>
        public IActionResult Console()
        {
            return View();
        }
    }
}