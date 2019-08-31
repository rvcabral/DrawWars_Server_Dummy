using DrawWars.Api.GameManager;
using DrawWars.Api.Hubs;
using DrawWars.Aws.Extensions;
using DrawWars.Data;
using DrawWars.Data.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrawWars.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            CoreManager.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigureDataServices(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();
            services.UseAwsManager(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();


            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<DrawWarsHub>("/Server");
            });
        }

        #region Private Utils

        private void ConfigureDataServices(IServiceCollection services)
        {
            services.AddTransient<IDrawingRepository, DrawingRepository>();
            services.AddTransient<IGameRoomRepository, GameRoomRepository>();
            services.AddTransient<IPlayerRepository, PlayerRepository>();
            services.AddTransient<IPlayerScoreRepository, PlayerScoreRepository>();
            services.AddTransient<IThemeRepository, ThemeRepository>();
        }

        #endregion
    }
}
