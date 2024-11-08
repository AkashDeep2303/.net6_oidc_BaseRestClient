using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace POC_Services.OAuth
{
    internal class OAuthConfigurationItems
    {
        public string SchemeName { get; set; }
        public string Authority { get; set; }
        public string AppIdentifier { get; set; }
        public string NameClaimType { get; set; } = ClaimTypes.Name;
        public string RoleClaimType { get; set; } = ClaimTypes.Name;
        public string ValidIssuer { get; set; }
        public string ClientSecret { get; set; }
        public string CertificateName { get; set; }
        public bool UseClientSecret { get; set; }
        public string DefaultAuthenticateScheme { get; set; } = JwtBearerDefaults.AuthenticationScheme;
        public int KeyRefreshInterval { get; set; } = 21600;
    }
}
