using Api.AuthRequirement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    //this class is added in ep7
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //having below line make it return 401 instead 500 if the token is expired
            //for API, if Authorization (line 26) fails, then it will challenge you to authenticate. in that case, if we dont' have authenticate registered,
            //it will return 500. in this case, we rather fail the authenticate with CustomAuthenticationHandler, 
            //so it will return 401 which makes more sense
            //we can just completely remove below AddAuthentication, in that case if the token is expired or invalid, it will return 500
            services.AddAuthentication("DefaultAuth")           //added in ep8
                .AddScheme<AuthenticationSchemeOptions, CustomeAuthenticationHandler>("DefaultAuth", null);

            //services.AddAuthentication();

            services.AddAuthorization(config =>
            {
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                    .AddRequirements(new JwtRequirement())
                    .Build();

                config.DefaultPolicy = defaultAuthPolicy;
            });

            services.AddScoped<IAuthorizationHandler, JwtRequirementHandler>();

            services.AddHttpClient()
                .AddHttpContextAccessor();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
