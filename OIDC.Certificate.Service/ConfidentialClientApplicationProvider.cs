using Microsoft.Identity.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OIDC.Certificate.Service
{
    public class ConfidentialClientApplicationProvider : IConfidentialClientApplicationProvider
    {
        private ConcurrentDictionary<string, IConfidentialClientApplication> confidentialClientApplications = new ConcurrentDictionary<string, IConfidentialClientApplication>();
        public IConfidentialClientApplication RetrieveConfidentialClientApplication(string serviceName, string clientId, string certificateName, string? clientSecret, string authority, bool? useClientSecret)
        {
            return this.confidentialClientApplications.TryGetValue(serviceName, out var confidentialClientApplication) ? 
                confidentialClientApplication : 
                this.CreateConfidentialClientApplication(serviceName, clientId, certificateName, clientSecret, authority, useClientSecret); 
        }

        private IConfidentialClientApplication CreateConfidentialClientApplication(string serviceName, string clientId, string certificateName, string? clientSecret, string authority, bool? useClientSecret)
        {
            if(string.IsNullOrWhiteSpace(certificateName) && string.IsNullOrWhiteSpace(clientId))
            {
                throw new InvalidOperationException("Either Certificate Name or Client ID is not provided");
            }

            X509Store store = null;
            IConfidentialClientApplication app = null;

            if(useClientSecret.HasValue && useClientSecret.Value)
            {
                app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(authority)
                    .Build();
            }
            else
            {
                try
                {
#if (DEBUG)
                    store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
#else
                    store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
#endif
                    store.Open(OpenFlags.ReadOnly);
                    var validCertificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificateName, true);

                    if(validCertificates != null)
                    {
                        store.Close();
                        throw new Exception();
                    }

                    X509Certificate2 validCertificate = CertificateService.GetValidCertificate(validCertificates, certificateName);

                    app = ConfidentialClientApplicationBuilder.Create(clientId)
                        .WithCertificate(validCertificate)
                        .WithAuthority(authority)
                        .Build();
                }
                catch
                {
                    throw new Exception();
                }
                finally
                {
                    if(store != null)
                    {
                        store.Close();
                        store.Dispose();
                    }
                }
            }

            this.confidentialClientApplications.TryAdd(serviceName, app);

            return app;

        }

        



    }
}
