using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DamSword.Common;
using DamSword.Data;
using DamSword.Data.Entities;
using DamSword.Data.Repositories;
using DamSword.Services;
using DamSword.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DamSword.Web
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public IContainer ApplicationContainer { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            HostingEnvironment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework();
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<EntityContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString(AppConfig.EntityContextConnectionStringName), contextOptions => contextOptions.MigrationsAssembly("DamSword.Web")));

            services.AddMvc();
            
            var builder = new ContainerBuilder();
            DependenciesConfig.Configure(builder, !HostingEnvironment.IsDevelopment());
            builder.Populate(services);

            ApplicationContainer = builder.Build();
            ServiceLocator.SetResolver(t => ApplicationContainer.Resolve<IServiceProvider>().GetService(t));
            
            return new AutofacServiceProvider(ApplicationContainer);
        }
        
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime, IHttpContextAccessor contextAccessor)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();

            ConfigureScopeContainer(app, contextAccessor);
            ConfigureScopes(app);
            ConfigureDatabase(app, HostingEnvironment);
            
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=86400");
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Dashboard}/{action=Details}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
        }

        private static void ConfigureScopeContainer(IApplicationBuilder app, IHttpContextAccessor contextAccessor)
        {
            const string scopeContainerContextItemName = "SOD_SCOPE_CONTAINER";

            ScopeContainer.SetScopeStackResolver(() => (ScopeContainer)contextAccessor.HttpContext.Items[scopeContainerContextItemName]);
            app.Use(async (context, next) =>
            {
                context.Items[scopeContainerContextItemName] = new ScopeContainer();
                await next();
            });
        }

        private static void ConfigureScopes(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var remoteIpAddress = context.GetRemoteIpAddress();
                var sessionHash = ServiceLocator.Resolve<IAuthenticationService>().GetCurrentSessionHash(context);
                var sessionInfo = ServiceLocator.Resolve<ISessionService>().GetSession(sessionHash, remoteIpAddress);

                User user = null;
                Session session = null;
                if (sessionInfo != null)
                {
                    user = ServiceLocator.Resolve<IUserRepository>().GetById(sessionInfo.UserId);
                    session = ServiceLocator.Resolve<ISessionRepository>().GetById(sessionInfo.Id);
                }

                SessionScope.Begin(session);
                UserScope.Begin(user);
                await next();
            });
        }

        private static void ConfigureDatabase(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<EntityContext>();
                var isDbExist = ((RelationalDatabaseCreator)context.GetService<IDatabaseCreator>()).Exists();

                if (!isDbExist)
                {
                    context.Database.EnsureCreated();
                }
                else if (env.IsDevelopment())
                {
                    context.Database.EnsureCreated();
                    context.Database.Migrate();
                }
            }
        }
    }
}
