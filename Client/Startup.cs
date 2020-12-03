using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Client
{
    //this class is added in ep6
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config => {
                // We check the cookie to confirm that we are authenticated
                //DefaultAuthenticateScheme. if specified, AuthenticateAsync() will use this scheme, and also the AuthenticationMiddleware added by 
                //UseAuthentication() will use this scheme to set context.User automatically.
                config.DefaultAuthenticateScheme = "ClientCookie";
                // When we sign in we will deal out a cookie
                //DefaultSignInScheme is used by SignInAsync() and also by all of the remote auth schemes like Google/Facebook/OIDC/OAuth, typically this would be set to a cookie.
                config.DefaultSignInScheme = "ClientCookie";
                // use this to check if we are allowed to do something.
                //DefaultChallengeScheme if specified, ChallengeAsync() will use this scheme, [Authorize] with policies that don't specify schemes will also use this
                config.DefaultChallengeScheme = "OurServer";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer", config => {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.CallbackPath = "/oauth/callback";
                    config.AuthorizationEndpoint = "https://localhost:44382/oauth/authorize";
                    config.TokenEndpoint = "https://localhost:44382/oauth/token";
                    config.SaveTokens = true;

                    config.Events = new OAuthEvents()       //this part is explained in ep06 at 47min of video
                    {                                       //whenever token is recieved from the server, this event is triggered to read the payload and parse out
                        OnCreatingTicket = context =>       //the claims and add those claims to requestContext.User.Identity
                        {
                            var accessToken = context.AccessToken;
                            var base64payload = accessToken.Split('.')[1];
                            var bytes = Convert.FromBase64String(base64payload);
                            var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                            foreach (var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient();               //added in ep7

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
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
