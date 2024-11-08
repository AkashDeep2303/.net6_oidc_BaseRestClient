using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;

namespace POC_Services.OAuth
{
    internal class OidcKeyResolver
    {
        private readonly ConfigurationManager<OpenIdConnectConfiguration> configurationManager;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

//------------------------------Using Well-known/configuration endpoint -------------------------------------------------------------------------------
        internal OidcKeyResolver(OAuthConfigurationItems oAuthConfigurationItems, IConfiguration configuration)
        {
            string wellKnownEndpoint = $"{oAuthConfigurationItems.Authority}/.well-known/openid-configuration";
            string googleJWKsURI = $"{configuration["GoogleJwksUri"]}";
            this._configuration = configuration;
            this._httpClient = new HttpClient();

            configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(wellKnownEndpoint, new OpenIdConnectConfigurationRetriever())
            {
                AutomaticRefreshInterval = new TimeSpan(0, oAuthConfigurationItems.KeyRefreshInterval, 0),
            };
        }

        internal ICollection<SecurityKey> Resolve(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters)
        {
            OpenIdConnectConfiguration openIdConnect = configurationManager.GetConfigurationAsync().GetAwaiter().GetResult();

            return openIdConnect.SigningKeys;
        }

// --------------------------------------------Using Google JWKs URI ------------------------------------------------------------------------------

        //internal IEnumerable<SecurityKey> ResolveGoogleToken(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters)
        //{
        //    var googleJwksUri = _configuration["GoogleJwksUri"]; // Replace with your Google JWKs URI https://www.googleapis.com/oauth2/v3/certs

        //    try
        //    {
        //        var response = _httpClient.GetAsync(googleJwksUri).GetAwaiter().GetResult();
        //        response.EnsureSuccessStatusCode();

        //        var jwkSet = JsonDocument.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult()).RootElement.Deserialize<JsonWebKeySet>();
        //        var keys = new List<SecurityKey>();
        //        foreach (var element in jwkSet.Keys)
        //        {
        //            var rsaKey = new RsaSecurityKey(new RSAParameters
        //            {
        //                Modulus = Base64UrlEncoder.DecodeBytes(element.N),
        //                Exponent = Base64UrlEncoder.DecodeBytes(element.E)
        //            })
        //            {
        //                KeyId = element.Kid
        //            };
        //            keys.Add(rsaKey);
        //        }
        //        return keys;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error or handle it appropriately
        //        Console.Error.WriteLine($"Error retrieving Google public keys: {ex.Message}");
        //        throw; // Re-throw the exception to propagate it to the caller
        //    }
        //}
    }

    //public class GoogleKeys
    //{
    //    public List<JsonWebKey> Keys { get; set; }
    //}
}
