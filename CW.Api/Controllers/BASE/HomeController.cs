using CW.Framework.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CW.Api.Controllers.BASE
{
    /// <summary>
    /// 主页
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// 健康监测
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult HealthCheck()
        {
            return Ok("CW Api Service Is Running");
        }

        /// <summary>
        /// 生成实体类(本地环境使用)
        /// </summary>
#if !DEBUG
        [NonAction]
#endif
        [HttpGet]
        public void GenerateEntities()
        {
            EntitiesGenerator.GenerateDbEntities();
        }

        /// <summary>
        /// 自定义生成实体(本地环境使用)
        /// </summary>
        /// <param name="model"></param>
#if !DEBUG
        [NonAction]
#endif
        [HttpPost]
        public void GenerateEntitiesByCustom(GenerateDbEntitiesModel model)
        {
            EntitiesGenerator.GenerateDbEntitiesByCustom(model.Path, model.Namespace, model.TablesName, model.InterfaceName, model.IsSerializable);
        }
    }

    public class GenerateDbEntitiesModel
    {
        /// <summary>
        /// 生成路径（默认）
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 命名空间（默认）
        /// </summary>
        public string Namespace { get; set; } = string.Empty;
        /// <summary>
        /// 指定表名
        /// </summary>
        public string[] TablesName { get; set; } = null;
        /// <summary>
        /// 实现接口
        /// </summary>
        public string InterfaceName { get; set; } = string.Empty;
        /// <summary>
        /// 是否序列化（默认是）
        /// </summary>
        public bool IsSerializable { get; set; } = true;
    }
}
