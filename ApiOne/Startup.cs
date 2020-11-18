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
                    //config.Authority = "https://localhost:44305";   //identity server address
                    config.Authority = "http://192.168.1.107:5000";     //added in ep22 (replaced above line)

                    config.Audience = "ApiOne";         //the token is for 'ApiOne' API

                    config.RequireHttpsMetadata = false;
                });

            services.AddCors(confg =>                           //added in ep15 to allow the Javascript client to call this API
                confg.AddPolicy("AllowAll",
                    p => p.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");                            //added in ep15

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
