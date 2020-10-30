using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace ApiOne
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;        //this will show the actual error if there is any when calling the Identity Server

            services.AddAuthentication("Bearer")        //authentication is done using Open-ID token using the identity server
                .AddJwtBearer("Bearer", config =>
                {
                    config.Authority = "https://localhost:44305";   //identity server address

                    config.Audience = "ApiOne";         //the token is for 'ApiOne' API

                    config.RequireHttpsMetadata = false;
                });

            //services.AddCors(confg =>
            //    confg.AddPolicy("AllowAll",
            //        p => p.AllowAnyOrigin()
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseCors("AllowAll");

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
