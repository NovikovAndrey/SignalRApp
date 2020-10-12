using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalRServer.Extension;
using SignalRServer.Hubs;
using SignalRServer.Interfaces;
using SignalRServer.Services;

namespace SignalRServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
               .AddJsonOptions(options => {
                    options.JsonSerializerOptions.WriteIndented = true;
                });
            //services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddSignalR();
            //services.AddSingleton<SendMessageService>();

            services.AddSendMessageService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");

            //app.Run(SendMessageService);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SendHub>("/chat");
            });

            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<SendHub>("/nlo");
            //});
        }
    }
}
