using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
<<<<<<< Updated upstream:C300Project/Startup.cs
=======
//using fyp.Models;
>>>>>>> Stashed changes:C300 project/Startup.cs

namespace fyp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            /*services.AddDbContext<Models.AppDbContext>(
                options =>
                    options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection")));
<<<<<<< Updated upstream:C300Project/Startup.cs
            
=======
            */
>>>>>>> Stashed changes:C300 project/Startup.cs
            services
               .AddAuthentication("UserSecurity")
               .AddCookie("UserSecurity",
                   options =>
                   {
                       options.LoginPath = "/Account/Login/";
                       options.AccessDeniedPath = "/Account/Forbidden/";
                   });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

