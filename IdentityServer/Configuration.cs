using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),     //server can return OpenID token
                new IdentityResources.Profile(),    //server can provide user info (profile) via userinfo end point (Access Token adds this scope)
                //new IdentityResource
                //{
                //    Name = "rc.scope",
                //    UserClaims =
                //    {
                //        "rc.garndma"
                //    }
                //}
            };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {
                new ApiResource("ApiOne"),      //this is the API that is availble to be accessed via the token provided by this identity server (ex: Graph API on Azure App Registration)
                new ApiResource("ApiTwo"),
                //new ApiResource("ApiTwo", new string[] { "rc.api.garndma" }),
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client {
                    ClientId = "client_id",         //this is just like the app registration (ex: AM Tool) 
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,   //from Azure --> clientCredential = application, Code = delegate

                    AllowedScopes = { "ApiOne" }    //ex: AM Tool is registered to be able to call the Graph API
                },
                new Client {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    //RequirePkce = true,

                    RedirectUris = { "https://localhost:44322/signin-oidc" },       //unlike ClientCredential type, auth code flow requires redirect URI
                    //PostLogoutRedirectUris = { "https://localhost:44322/Home/Index" },

                    AllowedScopes = {
                        "ApiOne",
                        "ApiTwo",
                        IdentityServerConstants.StandardScopes.OpenId,      //unlike OAuth, this is pure OpenID and requires this scope
                        IdentityServerConstants.StandardScopes.Profile,
                        //"rc.scope",
                    },

                    // puts all the claims in the id token
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    RequireConsent = false,         //this will make the consent screen not appear (consent displayed on Ideneity Server after login and before granting code)
                },

                //new Client {
                //    ClientId = "client_id_js",

                //    AllowedGrantTypes = GrantTypes.Code,
                //    RequirePkce = true,
                //    RequireClientSecret = false,

                //    RedirectUris = { "https://localhost:44345/home/signin" },
                //    PostLogoutRedirectUris = { "https://localhost:44345/Home/Index" },
                //    AllowedCorsOrigins = { "https://localhost:44345" },

                //    AllowedScopes = {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        "ApiOne",
                //        "ApiTwo",
                //        "rc.scope",
                //    },

                //    AccessTokenLifetime = 1,

                //    AllowAccessTokensViaBrowser = true,
                //    RequireConsent = false,
                //},

                //new Client {
                //    ClientId = "angular",

                //    AllowedGrantTypes = GrantTypes.Code,
                //    RequirePkce = true,
                //    RequireClientSecret = false,

                //    RedirectUris = { "http://localhost:4200" },
                //    PostLogoutRedirectUris = { "http://localhost:4200" },
                //    AllowedCorsOrigins = { "http://localhost:4200" },

                //    AllowedScopes = {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        "ApiOne",
                //    },

                //    AllowAccessTokensViaBrowser = true,
                //    RequireConsent = false,
                //},

                //new Client {
                //    ClientId = "wpf",

                //    AllowedGrantTypes = GrantTypes.Code,
                //    RequirePkce = true,
                //    RequireClientSecret = false,

                //    RedirectUris = { "http://localhost/sample-wpf-app" },
                //    AllowedCorsOrigins = { "http://localhost" },

                //    AllowedScopes = {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        "ApiOne",
                //    },

                //    AllowAccessTokensViaBrowser = true,
                //    RequireConsent = false,
                //},
                //new Client {
                //    ClientId = "xamarin",

                //    AllowedGrantTypes = GrantTypes.Code,
                //    RequirePkce = true,
                //    RequireClientSecret = false,

                //    RedirectUris = { "xamarinformsclients://callback" },

                //    AllowedScopes = {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        "ApiOne",
                //    },

                //    AllowAccessTokensViaBrowser = true,
                //    RequireConsent = false,
                //},
		//new Client {
  //                  ClientId = "flutter",

  //                  AllowedGrantTypes = GrantTypes.Code,
  //                  RequirePkce = true,
  //                  RequireClientSecret = false,

  //                  RedirectUris = { "http://localhost:4000/" },
  //                  AllowedCorsOrigins = { "http://localhost:4000" },

  //                  AllowedScopes = {
  //                      IdentityServerConstants.StandardScopes.OpenId,
  //                      "ApiOne",
  //                  },

  //                  AllowAccessTokensViaBrowser = true,
  //                  RequireConsent = false,
  //              },

            };
    }
}
