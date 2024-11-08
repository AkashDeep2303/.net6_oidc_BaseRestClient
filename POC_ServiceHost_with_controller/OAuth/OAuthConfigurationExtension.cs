using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using POC_Services.Middlewares;

namespace POC_Services.OAuth
{
    public static class OAuthConfigurationExtension
    {
        public static async Task<IServiceCollection> AddOAuthAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var oidcEnabled = configuration.GetSection("oidcEnabled").Get<bool>();
            var oidcConfig = configuration.GetSection("OAuth").Get<List<OAuthConfigurationItems>>();

            if (oidcEnabled)
            {
                AuthenticationBuilder authenticationBuilder = services.AddAuthentication(options =>
                {
                    if (configuration.GetSection("is_S256_Scheme").Get<bool>())
                    {
                        options.DefaultAuthenticateScheme = "S256";   // depends on Issuer/Authority
                        options.DefaultChallengeScheme = "S256";
                    }
                    else
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;   // depends on Issuer/Authority
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }
                });

                foreach (var item in oidcConfig)
                {
                    OidcKeyResolver keyResolver = new OidcKeyResolver(item, configuration);

                    _ = authenticationBuilder.AddJwtBearer(item.SchemeName, options =>
                    {
                        
                        options.Authority = String.IsNullOrEmpty(item.ValidIssuer) ? item.Authority : item.ValidIssuer;
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidAudience = item.AppIdentifier,
                            ValidateAudience = true,

                            ValidIssuer = String.IsNullOrEmpty(item.ValidIssuer) ? item.Authority : item.ValidIssuer,
                            ValidateIssuer = true,

                            ValidateIssuerSigningKey = true,

                            NameClaimType = item.NameClaimType,
                            RoleClaimType = item.RoleClaimType,

                            IssuerSigningKeyResolver = keyResolver.Resolve,

                        };
                    });
                }

                //Set default policy to check all specified schemes
                _ = services.AddAuthorization(options =>
                {
                    var authorizationPolicyBuilder = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(oidcConfig.Select(c => c.SchemeName).ToArray());

                    options.DefaultPolicy = authorizationPolicyBuilder.Build();

                });

            }
            else
            {
                services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
            }

            return services;

        }

        //// Method to fetch signing keys
        //private static async Task<IEnumerable<SecurityKey>> GetGoogleSigningKeysAsync()
        //{
        //    const string googleJwksUrl = "https://www.googleapis.com/oauth2/v3/certs";

        //    using (var httpClient = new HttpClient())
        //    {
        //        var response = await httpClient.GetStringAsync(googleJwksUrl);
        //        var jwks = JsonDocument.Parse(response);

        //        // Extract keys
        //        var keys = new List<SecurityKey>();
        //        foreach (var element in jwks.RootElement.GetProperty("keys").EnumerateArray())
        //        {
        //            var key = new JsonWebKey
        //            {
        //                Kid = element.GetProperty("kid").GetString(),
        //                Kty = element.GetProperty("kty").GetString(),
        //                Alg = element.GetProperty("alg").GetString(),
        //                N = element.GetProperty("n").GetString(),
        //                E = element.GetProperty("e").GetString(),
        //            };
        //            keys.Add(key);
        //        }
        //        return keys;
        //    }

        //}

    }
}
