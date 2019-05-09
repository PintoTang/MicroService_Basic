
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Core.Lib;
using Redis.Lib;
using Core.Lib.ErrorCode;
using Basic.Lib.Commons;
using Basic.Lib.Serialize;

namespace Basic.Lib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 框架基础
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFeng(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();
            services.AddSingleton<RedisClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUser, HttpContextUser>();
            services.AddSingleton<IRequestScopedDataRepository, HttpDataRepository>();
            services.AddSingleton<IJsonHelper, DefaultJsonHelper>();
            services.AddSingleton<IErrorCode, EmptyErrorCode>();
            return services;
        }
        /// <summary>
        /// 跨域服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
            return services;
        }
    }
}
