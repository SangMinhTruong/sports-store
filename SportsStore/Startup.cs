using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionIT.Mvc.Paging;
using SportsStore.Data;
using SportsStore.Helpers.Photos;
using SportsStore.Interfaces;
using SportsStore.Models;
using SportsStore.Models.Photos;
namespace SportsStore
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
                    .AddSessionStateTempDataProvider();

            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDefaultIdentity<ApplicationUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<StoreDbContext>();

            services.AddPaging(options =>
            {
                options.ViewName = "Bootstrap3";
                options.PageParameterName = "pageNumber";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy
                (
                    "AdminPolicy",
                    policy => policy.RequireRole("Admin")
                );
                options.AddPolicy
                (
                    "EmployeePolicy",
                    policy => policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin") ||
                        context.User.IsInRole("Employee")
                    )
                );
                options.AddPolicy
                (
                    "CustomerPolicy",
                    policy => policy.RequireAssertion(context =>
                        context.User.IsInRole("Cusomter") ||
                        (
                            !context.User.IsInRole("Employee") &&
                            !context.User.IsInRole("Admin")
                        )
                    )
                );
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(1);
            });

            services.AddScoped<IOrderRepository, EFOrderRepository>();
            services.AddScoped<IImportOrderRepository, EFImportOrderRepository>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Accounts/Login";
                options.LogoutPath = $"/Accounts/Logout";
                options.AccessDeniedPath = $"/Accounts/AccessDenied";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
