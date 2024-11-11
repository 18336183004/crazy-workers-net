using Autofac.Extensions.DependencyInjection;
using CW.Common.Attributes.ModelValidators;
using CW.Common.Filters;
using CW.Common.Logs;
using CW.Common.Network;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using SqlSugar;
using System.IO.Compression;
using CW.Common.Extensions;
using SqlSugar.IOC;
using AspNetCoreRateLimit;
using CW.Common.Middlewares;


try
{
    //builder
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    //获取配置
    IConfiguration configuration = builder.Configuration;
    //服务名称
    string serviceName = "CW.Api";

    //获取配置
    configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

    //// 添加CORS服务并允许所有来源
    //builder.Services.AddCors(options =>
    //{
    //    options.AddPolicy(name: "AllowAllOrigins",
    //        builder =>
    //        {
    //            builder.AllowAnyOrigin() // 允许任何来源
    //                   .AllowAnyHeader() // 允许任何头
    //                   .AllowAnyMethod(); // 允许任何方法（GET, POST等）
    //        });
    //});
    // 添加CORS服务并配置策略  
    //builder.Services.AddCors(options =>
    //{
    //    options.AddPolicy(name: "AllowSpecificOrigin",
    //        builder =>
    //        {
    //            builder.AllowAnyOrigin() // 允许任何来源
    //                   //.AllowAnyHeader() // 允许任何头
    //                   .WithHeaders("Content-Type", "Authorization", "X-Custom-Token") // 只允许这些头  
    //                   .AllowAnyMethod(); // 允许任何方法（GET, POST等）  
    //        });
    //});

    #region Nlog
    // Setup NLog for Dependency injection
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    builder.Host.UseNLog();
    #endregion

    #region 载入配置文件
    builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        //设置appsetting.json
        config.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  //.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
    });
    #endregion

    #region 服务 Services

    #region 服务注入
    builder.Services.AddOptions();

    // Add services to the container.
    builder.Services
        .AddControllers(c =>
        {
            c.Filters.Add<ModelValidationAttribute>();
            c.Filters.Add(typeof(GlobalExceptionsFilter));
        })
        .AddControllersAsServices()
        .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })// 取消默认驼峰
        .AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); }); //Newtonsoft.Json

    //健康检查
    builder.Services.AddHealthChecks();

    // HttpContext
    builder.Services.AddHttpContextAccessor();

    // HttpClient & Helper & Refit
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<HttpClientHelper>();

    // 服务层注入
    builder.Services.AddAllServices();

    #endregion

    #region SqlSugar
    builder.Services.AddSqlSugar(new IocConfig()
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection"),
        //ConnectionString = "PORT=3306;DATABASE=crazy_workers;HOST=8.140.184.170;PASSWORD=e055c42145dd4a53;USER ID=root",
        DbType = IocDbType.MySql,
        IsAutoCloseConnection = true,//自动释放
    }); //多个库就传List<IocConfig>
    builder.Services.ConfigurationSugar(db =>
    {
        db.CurrentConnectionConfig.InitKeyType = InitKeyType.Attribute;
        db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
        {
            IsAutoRemoveDataCache = true
        };
        db.Aop.OnLogExecuting = (sql, parameters) => { Console.WriteLine($"【SqlSugar - {DateTime.Now:yyyy-MM-dd HH:mm:ss}】：" + sql); };
    });
    #endregion

    #region 缓存
    builder.Services.AddSingleton<IMemoryCache>(factory => new MemoryCache(new MemoryCacheOptions()));
    #endregion

    #region  响应结果压缩
    //返回压缩
    builder.Services.AddResponseCompression(options =>
    {
        options.Providers.Add<BrotliCompressionProvider>(); //br压缩
        options.Providers.Add<GzipCompressionProvider>(); //gzip 压缩
    });
    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest; //压缩应该尽快完成，即使生成的输出未以最佳方式压缩。
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest; //压缩应该尽快完成，即使生成的输出未以最佳方式压缩。
    });
    #endregion

    #region IpRateLimitOptions
    //需要存储速率和ip规则
    //加载配置项
    builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
    //单机计时器和规则
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    #endregion

    #region 统一认证
    builder.Services.AddJwtAuth();
    #endregion

    #region swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwagger(serviceName, AppContext.BaseDirectory);
    #endregion

    #endregion

    WebApplication app = builder.Build();

    #region 配置 Configure

    #region Swagger文档
    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c => {c.Configure(serviceName);});
    #endregion

    app.UseMiddleware<CW.Api.windows.ActionMonitorMiddleware>();
    app.UseCors(); // 应用CORS策略  
    // 使用CORS中间件并指定策略名  
    //app.UseCors("AllowSpecificOrigin");

    #region 压缩
    app.UseResponseCompression();
    #endregion

    #region 请求响应日志
    app.UseReuestResponseLog();
    #endregion

    #region 校验
    app.UseAuthentication();
    app.UseJwtTokenAuthForApi();//Token校验
    #endregion

    #region Map
    app.MapControllers();
    app.MapHealthChecks("/api/healthcheck");
    #endregion

    #endregion
   
    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    app.Run();

}
catch (Exception ex)
{

    NLogHelper.ErrorLog("启动项目失败!", ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}