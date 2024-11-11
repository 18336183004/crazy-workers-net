using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using static CW.Common.Swagger.CustomApiVersion;
using Microsoft.OpenApi.Models;
using CW.Common.BASE;


namespace CW.Common.Extensions
{ 
    /// <summary>
    /// Swagger配置
    /// </summary>
    public static class SwaggerConfigExtension
    {
        /// <summary>
        /// 配置Swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceName">服务名称</param>
        /// <param name="basePath">程序根目录</param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, string serviceName, string basePath)
        {
            services.AddSwaggerGen(c =>
            {
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        // {ApiName} 定义成全局变量
                        Title = $"{serviceName} 接口文档",
                        Description = $"{serviceName} " + version,
                        //TermsOfService = "None",
                        Contact = new OpenApiContact { Name = serviceName, Email = "IT@ranwen.com" }
                    });
                    // 按相对路径排序
                    //c.OrderActionsBy(o => o.RelativePath);
                });
                //解决相同类名会报错的问题
                c.CustomSchemaIds(type => type.FullName);
                //配置的xml文件名
                var xmlPath = Path.Combine(basePath, $"{serviceName}.xml");
                c.IncludeXmlComments(xmlPath, true);

                #region Token绑定到ConfigureServices
                //添加header验证信息
                //c.OperationFilter<SwaggerHeader>();

                // 发行人
                var IssuerName = Define.ISSUER;
                var securit = new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = IssuerName},
                            //Name = IssuerName
                        }, Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(securit);

                //方案名称“Mammoth”可自定义，上下一致即可
                c.AddSecurityDefinition(IssuerName, new OpenApiSecurityScheme
                {
                    Description = "请在下框中输入Bearer {token}",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                #endregion
            });

            return services;
        }
    }
}
