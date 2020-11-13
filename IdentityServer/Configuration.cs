﻿using IdentityModel;
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
                new IdentityResource                //added in ep12. this needs to be added in order to show them in the id-token and let IdentityServer know about the claims
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "rc.garndma"
                    }
                }
            };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {
                new ApiResource("ApiOne"),      //this is the API that is availble to be accessed via the token provided by this identity server (ex: Graph API on Azure App Registration)
                //new ApiResource("ApiTwo"),
                new ApiResource("ApiTwo", new string[] { "rc.api.garndma" }),   //added in ep12. this will add the claim in access token
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
                    RequirePkce = true,                     //added in ep20 (will configure IS to use PKCE flow to generate auth code. advised to use this for native app and java scripts

                    RedirectUris = { "https://localhost:44322/signin-oidc" },       //unlike ClientCredential type, auth code flow requires redirect URI
                    PostLogoutRedirectUris = { "https://localhost:44322/Home/Index" },  //added in ep18

                    AllowedScopes = {
                        "ApiOne",
                        "ApiTwo",
                        IdentityServerConstants.StandardScopes.OpenId,      //unlike OAuth, this is pure OpenID and requires this scope
                        IdentityServerConstants.StandardScopes.Profile,
                        "rc.scope",                                         //added in ep12 to register this custom scope accessible by the mvcClient
                    },

                    // puts all the claims in the id token
                    AlwaysIncludeUserClaimsInIdToken = true,        //added in ep12. this would add the user claims to the ID Token (name and preferred user name will be added)
                    AllowOfflineAccess = true,                      //added in ep13 to allow refresh token
                    RequireConsent = false,         //this will make the consent screen not appear (consent displayed on Ideneity Server after login and before granting code)
                },

                new Client {                            //added in ep14
                    ClientId = "client_id_js",

                    AllowedGrantTypes = GrantTypes.Code,        //added in ep20 (the flow is still implicit, but we will receive the code back in url)
                    //AllowedGrantTypes = GrantTypes.Implicit,  //removed in ep20
                    RequirePkce = true,                         //added in ep20
                    RequireClientSecret = false,                //added in ep20

                    RedirectUris = { "https://localhost:44345/home/signin" },
                    PostLogoutRedirectUris = { "https://localhost:44345/Home/Index" },      //added in ep18
                    AllowedCorsOrigins = { "https://localhost:44345" },             //added in ep15 (to allow .js to call IdentityServer's end points

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                        "ApiTwo",   //added in ep16
                        "rc.scope", //added in ep16
                    },

                    AccessTokenLifetime = 1,        //added in ep16. this is for testing to make the Access Token to be invalid in 1 second to show how it automatically regenerate a token

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },

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

                new Client {
                    ClientId = "wpf",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "http://localhost/sample-wpf-app" },
                    AllowedCorsOrigins = { "http://localhost" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },
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
