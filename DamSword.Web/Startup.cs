using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DamSword.Common;
using DamSword.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DamSword.Web
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IContainer ApplicationContainer { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework();
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<EntityContext>(options => options.UseSqlServer(Configuration.GetConnectionString(AppConfig.EntityContextConnectionStringName)));
            services.AddMvc();
            
            var builder = new ContainerBuilder();
            DependenciesConfig.Configure(builder);
            builder.Populate(services);

            ApplicationContainer = builder.Build();
            ServiceLocator.SetResolver(t => ApplicationContainer.Resolve<IServiceProvider>().GetService(t));
            
            return new AutofacServiceProvider(ApplicationContainer);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole();

            InitializeDatabase(app, env);
            app.UseDeveloperExceptionPage();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
        }

        private static void InitializeDatabase(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<EntityContext>();
                if (env.IsDevelopment())
                {
                    context.Database.EnsureDeleted();
                }

                context.Database.EnsureCreated();

                if (env.IsDevelopment())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
