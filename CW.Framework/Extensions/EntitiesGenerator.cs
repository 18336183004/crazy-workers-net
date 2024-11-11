using SqlSugar;
using SqlSugar.IOC;
using System.IO;

namespace CW.Framework.Extensions
{
    public static class EntitiesGenerator
    {
        private static SqlSugarClient _db = DbScoped.Sugar;

        /// <summary>
        /// 生成数据库实体类
        /// </summary>
        public static void GenerateDbEntities(string serviceName = "CW.Api")
        {
            string path = Directory.GetCurrentDirectory();
            //path = path.Substring(0, path.Length - 15); //去掉 CW.Api
            path = path.Replace(serviceName, "");
            //_db.DbFirst
            //    .Where(db => db.StartsWith("Y"))
            //    .IsCreateDefaultValue()
            //    .IsCreateAttribute()
            //    .CreateClassFile(path + @"CW.Framework\Entities", "CW.Framework.Entities");
            GenerateDbEntitiesByCustom(path + @"CW.Framework\Entities", "CW.Framework.Entities", null, "", true);
        }
        /// <summary>
        /// 自定义生成数据库实体类
        /// </summary>
        /// <param name="path">生成路径(默认)</param>
        /// <param name="nameSpace">命名空间(默认)</param>
        /// <param name="tablesName">指定表名</param>
        /// <param name="interfaceName">继承接口/类</param>
        /// <param name="isSerializable">是否序列化(默认否)</param>
        /// <param name="tableNameStartWith">以[当前字符串]开头的表</param>
        public static void GenerateDbEntitiesByCustom(string path, string nameSpace, string[] tablesName = null,
            string interfaceName = null, bool isSerializable = false, string tableNameStartWith = null)
        {
            path = !string.IsNullOrEmpty(path) ? path : Directory.GetCurrentDirectory() + @"/Entities";
            nameSpace = !string.IsNullOrEmpty(nameSpace) ? nameSpace : "SqlSugarDemo.Entities";

            if (tablesName != null && tablesName.Length > 0)
            {
                _db.DbFirst.Where(tablesName).IsCreateDefaultValue().IsCreateAttribute()
                    .SettingClassTemplate(p => p = @"
{using}

namespace {Namespace}
{
    {ClassDescription}{SugarTable}" + (isSerializable ? "\r\n    [Serializable]" : "") + @"
    public partial class {ClassName}" +
                                                   (string.IsNullOrEmpty(interfaceName)
                                                       ? ":BaseEntity"
                                                       : (" :BaseEntity, " + interfaceName)) + @"
    {
        public {ClassName}()
        {
{Constructor}
        }
{PropertyName}
    }
}
")
                    .SettingPropertyTemplate(p => p = @"
        {SugarColumn}
        public {PropertyType} {PropertyName}{get;set;}")
                    //.SettingPropertyDescriptionTemplate(p => p = "          private {PropertyType} _{PropertyName};\r\n" + p)
                    //.SettingConstructorTemplate(p => p = "              this._{PropertyName} ={DefaultValue};")
                    .CreateClassFile(path, nameSpace);
            }
            else
            {
                var generate = _db.DbFirst;
                if (!string.IsNullOrEmpty(tableNameStartWith))
                {
                    generate = generate.Where(t => t.StartsWith(tableNameStartWith));
                }
                generate.IsCreateAttribute().IsCreateDefaultValue()
                    .SettingClassTemplate(p => p = @"
{using}

namespace {Namespace}
{
    {ClassDescription}{SugarTable}" + (isSerializable ? "\r\n    [Serializable]" : "") + @"
    public partial class {ClassName}" +
                                                   (string.IsNullOrEmpty(interfaceName)
                                                       ? ":BaseEntity"
                                                       : (" :BaseEntity, " + interfaceName)) + @"
    {
        public {ClassName}()
        {
{Constructor}
        }
{PropertyName}
    }
}
")
                    .SettingPropertyTemplate(p => p = @"
        {SugarColumn}
        public {PropertyType} {PropertyName}{get;set;}")
                    //.SettingPropertyDescriptionTemplate(p => p = "          private {PropertyType} _{PropertyName};\r\n" + p)
                    //.SettingConstructorTemplate(p => p = "              this._{PropertyName} ={DefaultValue};")
                    .CreateClassFile(path, nameSpace);
            }
        }
    }
}
