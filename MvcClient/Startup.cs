using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config => {
                config.DefaultScheme = "Cookie";                //store the authentiation to cookie
                config.DefaultChallengeScheme = "oidc";         //if challenged (cookie is missing), get it from identity server using auth code flow for oidc
            })
                .AddCookie("Cookie")
                .AddOpenIdConnect("oidc", config => {
                    config.Authority = "https://localhost:44305/";
                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secret_mvc";
                    config.SaveTokens = true;
                    config.ResponseType = "code";
                    config.SignedOutCallbackPath = "/Home/Index";       //added in ep18

                    // configure cookie claim mapping                   //added in ep12. this allows you to manipulate what to include/remove from teh claim
                    config.ClaimActions.DeleteClaim("amr");
                    config.ClaimActions.DeleteClaim("s_hash");
                    config.ClaimActions.MapUniqueJsonKey("RawCoding.Grandma", "rc.garndma");

                    //// two trips to load claims in to the cookie
                    //// but the id token is smaller !
                    config.GetClaimsFromUserInfoEndpoint = true;        //added in ep12 (adding this makes the id-token smaller by not including the user info
                                                                        //by having AlwaysIncludeUserClaimsInIdToken = true on configuration.cs)

                    // configure scope                      //added in ep12. this shows we can clear all scope and add only ones we need
                    config.Scope.Clear();
                    config.Scope.Add("openid");
                    config.Scope.Add("rc.scope");           //added in ep12. this client is now requesting access to this scope which already should be registerd in configuration
                    config.Scope.Add("ApiOne");             //added in ep12
                    config.Scope.Add("ApiTwo");
                    config.Scope.Add("offline_access");     //added in ep13

                });

            services.AddHttpClient();

            services.AddControllersWithViews();
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
