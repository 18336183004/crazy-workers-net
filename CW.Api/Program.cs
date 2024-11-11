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
    //��ȡ����
    IConfiguration configuration = builder.Configuration;
    //��������
    string serviceName = "CW.Api";

    //��ȡ����
    configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

    //// ���CORS��������������Դ
    //builder.Services.AddCors(options =>
    //{
    //    options.AddPolicy(name: "AllowAllOrigins",
    //        builder =>
    //        {
    //            builder.AllowAnyOrigin() // �����κ���Դ
    //                   .AllowAnyHeader() // �����κ�ͷ
    //                   .AllowAnyMethod(); // �����κη�����GET, POST�ȣ�
    //        });
    //});
    // ���CORS�������ò���  
    //builder.Services.AddCors(options =>
    //{
    //    options.AddPolicy(name: "AllowSpecificOrigin",
    //        builder =>
    //        {
    //            builder.AllowAnyOrigin() // �����κ���Դ
    //                   //.AllowAnyHeader() // �����κ�ͷ
    //                   .WithHeaders("Content-Type", "Authorization", "X-Custom-Token") // ֻ������Щͷ  
    //                   .AllowAnyMethod(); // �����κη�����GET, POST�ȣ�  
    //        });
    //});

    #region Nlog
    // Setup NLog for Dependency injection
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    builder.Host.UseNLog();
    #endregion

    #region ���������ļ�
    builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        //����appsetting.json
        config.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  //.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
    });
    #endregion

    #region ���� Services

    #region ����ע��
    builder.Services.AddOptions();

    // Add services to the container.
    builder.Services
        .AddControllers(c =>
        {
            c.Filters.Add<ModelValidationAttribute>();
            c.Filters.Add(typeof(GlobalExceptionsFilter));
        })
        .AddControllersAsServices()
        .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })// ȡ��Ĭ���շ�
        .AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); }); //Newtonsoft.Json

    //�������
    builder.Services.AddHealthChecks();

    // HttpContext
    builder.Services.AddHttpContextAccessor();

    // HttpClient & Helper & Refit
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<HttpClientHelper>();

    // �����ע��
    builder.Services.AddAllServices();

    #endregion

    #region SqlSugar
    builder.Services.AddSqlSugar(new IocConfig()
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection"),
        //ConnectionString = "PORT=3306;DATABASE=crazy_workers;HOST=8.140.184.170;PASSWORD=e055c42145dd4a53;USER ID=root",
        DbType = IocDbType.MySql,
        IsAutoCloseConnection = true,//�Զ��ͷ�
    }); //�����ʹ�List<IocConfig>
    builder.Services.ConfigurationSugar(db =>
    {
        db.CurrentConnectionConfig.InitKeyType = InitKeyType.Attribute;
        db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
        {
            IsAutoRemoveDataCache = true
        };
        db.Aop.OnLogExecuting = (sql, parameters) => { Console.WriteLine($"��SqlSugar - {DateTime.Now:yyyy-MM-dd HH:mm:ss}����" + sql); };
    });
    #endregion

    #region ����
    builder.Services.AddSingleton<IMemoryCache>(factory => new MemoryCache(new MemoryCacheOptions()));
    #endregion

    #region  ��Ӧ���ѹ��
    //����ѹ��
    builder.Services.AddResponseCompression(options =>
    {
        options.Providers.Add<BrotliCompressionProvider>(); //brѹ��
        options.Providers.Add<GzipCompressionProvider>(); //gzip ѹ��
    });
    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest; //ѹ��Ӧ�þ�����ɣ���ʹ���ɵ����δ����ѷ�ʽѹ����
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest; //ѹ��Ӧ�þ�����ɣ���ʹ���ɵ����δ����ѷ�ʽѹ����
    });
    #endregion

    #region IpRateLimitOptions
    //��Ҫ�洢���ʺ�ip����
    //����������
    builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
    //������ʱ���͹���
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    #endregion

    #region ͳһ��֤
    builder.Services.AddJwtAuth();
    #endregion

    #region swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwagger(serviceName, AppContext.BaseDirectory);
    #endregion

    #endregion

    WebApplication app = builder.Build();

    #region ���� Configure

    #region Swagger�ĵ�
    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c => {c.Configure(serviceName);});
    #endregion

    app.UseMiddleware<CW.Api.windows.ActionMonitorMiddleware>();
    app.UseCors(); // Ӧ��CORS����  
    // ʹ��CORS�м����ָ��������  
    //app.UseCors("AllowSpecificOrigin");

    #region ѹ��
    app.UseResponseCompression();
    #endregion

    #region ������Ӧ��־
    app.UseReuestResponseLog();
    #endregion

    #region У��
    app.UseAuthentication();
    app.UseJwtTokenAuthForApi();//TokenУ��
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

    NLogHelper.ErrorLog("������Ŀʧ��!", ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}