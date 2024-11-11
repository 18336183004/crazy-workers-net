using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CW.Common.Extensions
{
    /// <summary>
    /// 服务层注入
    /// </summary>
    public static class ServiceInjectionExtension
    {
        private const string ServiceClassNameEnd = "Service";
        public static IServiceCollection AddAllServices(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            // 获取当前程序集 注入所有服务类
            //var assembly = Assembly.GetAssembly(typeof(ServiceInjectionExtensions));
            var assembly = Assembly.GetEntryAssembly();

            var types = assembly.GetTypes();

            var classList = types.Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && t.Name.EndsWith(ServiceClassNameEnd));

            if (classList?.Any() ?? false)
            {
                foreach (var type in classList)
                {
                    var interfaceList = type.GetInterfaces();

                    if (interfaceList?.Any() ?? false)
                    {
                        var inf = interfaceList.First();
                        switch (serviceLifetime)
                        {
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(inf, type);
                                break;
                            case ServiceLifetime.Scoped:
                                services.AddScoped(inf, type);
                                break;
                            case ServiceLifetime.Transient:
                                services.AddTransient(inf, type);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }           
            return services;
        }
    }
}
