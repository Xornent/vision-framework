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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;

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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option => {
                    option.AccessDeniedPath = "/Account/Login"; 
                    option.LoginPath = "/Account/Login";
                    option.Cookie.Name = "user-identity";
                    option.Cookie.HttpOnly = true;

                    // 注意，我们默认采用 HTTPS 协议，设置本项表示验证用户的 Cookie 在
                    // 非 HTTPS 下不会发送，如果要适配 HTTP 协议，修改未 SameAsRequest.

                    option.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                });
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

            CookiePolicyOptions cookieOptions = new CookiePolicyOptions();
            cookieOptions.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            cookieOptions.Secure = CookieSecurePolicy.Always;

            app.UseCookiePolicy(cookieOptions);
            app.UseAuthentication();
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
