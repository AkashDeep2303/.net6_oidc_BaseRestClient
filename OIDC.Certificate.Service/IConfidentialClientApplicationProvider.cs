using Microsoft.Identity.Client;

namespace OIDC.Certificate.Service
{
    /// <summary>
    /// Confidential Client Application Provider for OIDC M2M Service.
    /// </summary>
    public interface IConfidentialClientApplicationProvider
    {
        IConfidentialClientApplication RetrieveConfidentialClientApplication(string serviceName, string clientId, string certificateName, string? clientSecret, string authority, bool? useClientSecret);
    }
}
