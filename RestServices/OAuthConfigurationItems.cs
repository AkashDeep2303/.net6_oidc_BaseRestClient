using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServices
{
    internal class OAuthConfigurationItems
    {
        public string SchemeName { get; set; }
        public string Authority { get; set; }
        public string AppIdentifier { get; set; }
        public string NameClaimType { get; set; } 
        public string RoleClaimType { get; set; }
        public string ValidIssuer { get; set; }
        public string ClientSecret { get; set; }
        public string CertificateName { get; set; }
        public bool UseClientSecret { get; set; }
        public string DefaultAuthenticateScheme { get; set; }
        public int KeyRefreshInterval { get; set; } = 21600;
    }
}
