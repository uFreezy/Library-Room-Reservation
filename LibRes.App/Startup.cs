﻿using System;
using LibRes.App.Data;
using LibRes.App.DbModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibRes.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            const string connectionString =
                "Server=tcp:librestest.database.windows.net,1433;Initial Catalog=LibResTest;Persist Security Info=False;User ID=libresmanager;Password=Libres123456!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            services
                .AddDbContext<LibResDbContext>(o =>
                    o.UseSqlServer(connectionString));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddIdentity<ApplicationUser, IdentityRole>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<LibResDbContext>()
                .AddDefaultTokenProviders();
            services.AddMvc();


            services.ConfigureApplicationCookie(options => options.LoginPath = "/login");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Build the intermediate service provider
            var serviceProvider = services.BuildServiceProvider();

            //resolve implementations
            var libResDbContext = serviceProvider
                .GetService<LibResDbContext>();
            var userManager = serviceProvider
                .GetService<UserManager<ApplicationUser>>();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute("spa-fallback", new {controller = "Home", action = "Error"});
            });


            //resolve implementations
            var libResDbContext = serviceProvider
                .GetService<LibResDbContext>();
            var userManager = serviceProvider
                .GetService<UserManager<ApplicationUser>>();

            libResDbContext.CreateSeedData(userManager);
        }
    }
}