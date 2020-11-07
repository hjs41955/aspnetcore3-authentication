using IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");    //added in ep17

            services.AddDbContext<AppDbContext>(config =>       //added for ep11. from this line to line #51 is same as in basic project
            {
                config.UseSqlServer(connectionString);          //added in ep17 and commented out below
                //config.UseInMemoryDatabase("Memory");
            });

            // AddIdentity registers the services
            services.AddIdentity<IdentityUser, IdentityRole>(config =>      //password requirement for sign-up
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()                   //store user login info into in-memory DB
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";               //name of the cookie
                config.LoginPath = "/Auth/Login";                           //if cookie isn't there or expired, use this end-point to log-in
                config.LogoutPath = "/Auth/Logout";                         //added in ep18
            });

            var assembly = typeof(Startup).Assembly.GetName().Name;         //added in ep17

            var filePath = Path.Combine(_env.ContentRootPath, "is_cert.pfx");   //added in ep18
            var certificate = new X509Certificate2(filePath, "test");   //added in ep18

            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()                  //added in ep11. adding this will allow IdentityServer to have access to above lines #36-44
                .AddConfigurationStore(options =>                   //below 10 lines are added in ep17 for SQL Server
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,  //used for config data such as clients, resources, and scopes
                        sql => sql.MigrationsAssembly(assembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,  //used for temporary operational data such as auth codes, refresh tokens
                        sql => sql.MigrationsAssembly(assembly));
                })
                .AddSigningCredential(certificate);                                     //added in ep18
                //.AddInMemoryApiResources(Configuration.GetApis())                     //commented out in ep17
                //.AddInMemoryIdentityResources(Configuration.GetIdentityResources())   //this line added for ep10, commented out in ep17
                //.AddInMemoryClients(Configuration.GetClients())                       //commented out in ep17
                //.AddDeveloperSigningCredential();                                     //commented out in ep18

            //services.AddAuthentication()
            //    .AddFacebook(config => {
            //        config.AppId = "3396617443742614";
            //        config.AppSecret = "secret";
            //    });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();

            //if(_env.IsDevelopment())
            //{
            //    app.UseCookiePolicy(new CookiePolicyOptions()
            //    {
            //         MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax
            //    });
            //}

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
