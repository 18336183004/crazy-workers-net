using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CW.Web.Controllers
{
    //[Authorize]
    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public BaseController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="content">提示信息</param>
        /// <param name="id">返回主键ID</param>
        /// <returns></returns>
        protected IActionResult AjaxResult(string state, string content, string id = "")
        {
            var obj = new
            {
                state,
                content,
                id
            };
            return Json(obj);
        }

        /// <summary>
        /// ajax请求结果
        /// </summary>
        /// <param name="data">layUI Table Data</param>
        /// <returns></returns>
        protected IActionResult AjaxResult(object data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            return Content(jsonData);
        }

    }
}
