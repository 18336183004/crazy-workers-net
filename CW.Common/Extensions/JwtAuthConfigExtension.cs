using System;
using System.Text;
using System.Threading.Tasks;
using CW.Common.BASE;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CW.Common.Extensions
{
    public static class JwtAuthConfigExtension
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services)
        {

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Define.SSO_TOKEN_SECRET)),
                    ValidIssuer = Define.ISSUER,
                    ValidateIssuer = true,
                    ValidAudience = Define.AUDIENCE,
                    ValidateAudience = true,

                    //ValidateLifetime = true, //默认true
                    ClockSkew = TimeSpan.FromSeconds(30), //默认300
                    //RequireExpirationTime = true,  //默认true
                };
                option.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // 如果过期，则把<是否过期>添加到，返回头信息中
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            //services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            //services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            //services.AddAuthorization();
            return services;
        }
    }
}
