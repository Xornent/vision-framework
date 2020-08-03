using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;
using System.Diagnostics;

namespace Vision {

    public class Startup {
        public static string WebRoot = "";

        public Startup(IConfiguration configuration) {
            WebRoot = (string)configuration.GetValue(typeof(String), "URLS");
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. 
        // Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();

            services.AddDbContext<PageContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Vision")));
            services.AddDbContext<RecordContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Vision")));
            services.AddDbContext<CategoryContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Vision")));
            services.AddDbContext<UserContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Vision")));
        }

        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this
                // for production scenarios, see https://aka.ms/aspnetcore-hsts.

                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public static class Log {
        [Conditional("LOG")]
        public static void WriteLine(string format, params object[] args) {
            Debug.WriteLine(string.Format(format, args));
        }
    }
}