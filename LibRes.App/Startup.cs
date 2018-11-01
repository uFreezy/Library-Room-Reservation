using System;
using LibRes.App.Data;
using LibRes.App.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
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
            var connectionString = "Server=.;Database=LibRes;User Id=UpdateMe;Password=A123456z";
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Build the intermediate service provider
            var serviceProvider = services.BuildServiceProvider();

            //resolve implementations
            LibResDbContext libResDbContext = serviceProvider
                .GetService<LibResDbContext>();
            UserManager<ApplicationUser> userManager = serviceProvider
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
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute(name: "spa-fallback", defaults: new { controller = "Home", action = "Error" });

            });
            
            //resolve implementations
            LibResDbContext libResDbContext = serviceProvider
                .GetService<LibResDbContext>();
            UserManager<ApplicationUser> userManager = serviceProvider
                .GetService<UserManager<ApplicationUser>>();

            libResDbContext.CreateSeedData(userManager);


        }
    }
}
