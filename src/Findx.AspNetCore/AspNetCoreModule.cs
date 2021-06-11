﻿using Findx.AspNetCore.Mvc;
using Findx.AspNetCore.Upload;
using Findx.DependencyInjection;
using Findx.Extensions;
using Findx.Modularity;
using Findx.Security;
using Findx.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;
using System.Security.Principal;

namespace Findx.AspNetCore
{
    [Description("Findx-AspNetCore模块")]
    public class AspNetCoreModule : FindxModule
    {
        public override ModuleLevel Level => ModuleLevel.Application;

        public override int Order => 10;

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddHttpContextAccessor();
            services.AddTransient<IPrincipal>(provider =>
            {
                IHttpContextAccessor accessor = provider.GetService<IHttpContextAccessor>();
                return accessor?.HttpContext?.User;
            });
            // 用户，依赖IPrincipal
            services.AddSingleton<ICurrentUser, CurrentUser>();

            services.TryAddSingleton<IScopedServiceResolver, HttpContextServiceScopeResolver>();
            services.Replace<ICancellationTokenProvider, HttpContextCancellationTokenProvider>(ServiceLifetime.Singleton);
            services.Replace<IHybridServiceScopeFactory, HttpContextServiceScopeFactory>(ServiceLifetime.Singleton);

            // API资源
            services.AddSingleton<IApiInterfaceService, DefaultApiInterfaceService>();
            // 文件上传
            services.AddSingleton<IFileUploadService, DefaultFileUploadService>();

            // 关闭模型自动化验证,实现自控
            services.Configure<ApiBehaviorOptions>(opts => opts.SuppressModelStateInvalidFilter = true);

            return services;
        }
    }
}
