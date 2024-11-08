using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http.Features.Authentication;
using System.Security.Claims;

namespace POC_Services.Middlewares
{
    public class Authentication1Middleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                var schemes = new List<string>{ "googleAuth" };

                foreach(var scheme in schemes)
                {
                    var result = await context.AuthenticateAsync(scheme);
                    if (result.Succeeded)
                    {
                        context.User = result.Principal;
                        break;
                    }
                    throw new UnauthorizedAccessException();
                }

                if (!context.User.Identity.IsAuthenticated)
                {
                    await context.ChallengeAsync("googleAuth");
                }

            }

            await next(context);

            // ----------------------------------- To authenticate without registration at build time -----------------------------------------------------
            //var client = new HttpClient();
            //var googleKeys = await client.GetFromJsonAsync<GoogleKeys>("https://www.googleapis.com/oauth2/v3/certs");

            //var handler = new JwtSecurityTokenHandler();
            //var tokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidIssuer = "https://accounts.google.com",
            //    ValidateAudience = true,
            //    ValidAudience = "52855828166-rf80dpnioksgh8ck9l3s5sdj92rs27qc.apps.googleusercontent.com",
            //    IssuerSigningKeys = googleKeys.Keys.Select(key =>
            //        new RsaSecurityKey(new RSAParameters
            //        {
            //            Modulus = Base64UrlEncoder.DecodeBytes(key.N),
            //            Exponent = Base64UrlEncoder.DecodeBytes(key.E)
            //        })
            //    )
            //};

            //SecurityToken validatedToken;
            //var principal = handler.ValidateToken(context.Request.Headers["Authorization"], tokenValidationParameters, out validatedToken);

            //// Call the next middleware in the pipeline
            //if(validatedToken != null)
            //{
            //    await next(context);
            //}
            //else
            //{
            //    throw new UnauthorizedAccessException();
            //}

        }
    }

    //public class GoogleKeys
    //{
    //    public List<JsonWebKey> Keys { get; set; }
    //}
}


