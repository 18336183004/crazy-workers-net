using CW.Common.Attributes.ModelValidators;
using CW.Common.Extensions;
using CW.Common.Filters;
using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar.IOC;
using SqlSugar;
using Newtonsoft.Json.Serialization;
using CW.Common.Middlewares;

var builder = WebApplication.CreateBuilder(args);

//在ASP.NET Core应用程序中配置依赖注入容器，将 HttpContextAccessor 注册为一个服务
builder.Services.AddHttpContextAccessor();


// 加载appsettings.json配置
var configuration = builder.Configuration;

#region 服务注入
builder.Services.AddOptions();

builder.Services.AddControllers(c =>
{
    c.Filters.Add<ModelValidationAttribute>();
    c.Filters.Add(typeof(GlobalExceptionsFilter));
})
//.AddControllersAsServices()
.AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })// 取消默认驼峰
.AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

////服务层注入
builder.Services.AddAllServices();
#endregion

#region SqlSugar
builder.Services.AddSqlSugar(new IocConfig()
{
    ConnectionString = configuration.GetSection("ConnectionStrings").GetSection("DbConnect.Default.Server").Value,
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

#region Memory
//services.AddScoped<ICaching, MemoryCaching>();
//services.AddSingleton<IMemoryCache>(factory => new MemoryCache(new MemoryCacheOptions()));
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

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Logging.AddLog4Net("log4net.config");
// 添加CORS服务并允许所有来源
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() // 允许任何来源
                   .AllowAnyHeader() // 允许任何头
                   .AllowAnyMethod(); // 允许任何方法（GET, POST等）
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    #region 压缩
    app.UseResponseCompression();
    #endregion

    #region 请求响应日志
    app.UseReuestResponseLog();
    #endregion

    #region routing
    app.UseRouting();
    #endregion

    #region endpoint
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
    #endregion
}
//app.UseMiddleware<CapsuleToysWeb.ActionMonitorMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Account}/{action=Login}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");


app.Run();
