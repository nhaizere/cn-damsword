using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DamSword.Common;
using DamSword.Data;
using DamSword.Web.DatabaseInitializers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole();

            ConfigureDatabase(app, HostingEnvironment);
            app.UseDeveloperExceptionPage();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
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
                    serviceScope.ServiceProvider.GetService<IDatabaseInitializer>().Initialize(context);
                }
                else if (env.IsDevelopment())
                {
                    context.Database.Migrate();
                    serviceScope.ServiceProvider.GetService<IDatabaseInitializer>().Initialize(context);
                }
            }
        }
    }
}
