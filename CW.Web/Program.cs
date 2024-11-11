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

//��ASP.NET CoreӦ�ó�������������ע���������� HttpContextAccessor ע��Ϊһ������
builder.Services.AddHttpContextAccessor();


// ����appsettings.json����
var configuration = builder.Configuration;

#region ����ע��
builder.Services.AddOptions();

builder.Services.AddControllers(c =>
{
    c.Filters.Add<ModelValidationAttribute>();
    c.Filters.Add(typeof(GlobalExceptionsFilter));
})
//.AddControllersAsServices()
.AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })// ȡ��Ĭ���շ�
.AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

////�����ע��
builder.Services.AddAllServices();
#endregion

#region SqlSugar
builder.Services.AddSqlSugar(new IocConfig()
{
    ConnectionString = configuration.GetSection("ConnectionStrings").GetSection("DbConnect.Default.Server").Value,
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

#region Memory
//services.AddScoped<ICaching, MemoryCaching>();
//services.AddSingleton<IMemoryCache>(factory => new MemoryCache(new MemoryCacheOptions()));
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

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Logging.AddLog4Net("log4net.config");
// ���CORS��������������Դ
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin() // �����κ���Դ
                   .AllowAnyHeader() // �����κ�ͷ
                   .AllowAnyMethod(); // �����κη�����GET, POST�ȣ�
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    #region ѹ��
    app.UseResponseCompression();
    #endregion

    #region ������Ӧ��־
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
