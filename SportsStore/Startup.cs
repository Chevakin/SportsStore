using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options => options.Serve
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddHttpContextAccessor();

            services.AddTransient<IProductRepository, EFProductRepository>();
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp.GetService<IHttpContextAccessor>()));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IOrderRepository, EFOrderRepository>();
;        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: null,
                   pattern: "{area:exists}/{controller=Admin}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "{category}/Page{productPage:int}",
                    defaults: new { controller = "Product", action = "List" });

                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "Page{productPage:int}",
                    defaults: new { controller = "Product", action = "List",
                    productPage = 1});

                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "{category}",
                    defaults: new { controller = "Product", action = "List",
                        productPage = 1
                    });

                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "",
                    defaults: new { controller = "Product", action = "List",
                        productPage = 1
                    });

                endpoints.MapControllerRoute(
                    name: null,
                    pattern: "{controller}/{action}/{id?}" 
                    );

                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });

            SeedData.EnsurePopulated(app);
            IdentitySeedData.EnsurePopulated(app);
        }
    }
}
