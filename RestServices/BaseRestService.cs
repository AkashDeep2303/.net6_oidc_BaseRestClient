namespace RestServices
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Client;
    using OIDC.Certificate.Service;
    using RestSharp;
    using RestSharp.Authenticators;
    using RestSharp.Authenticators.OAuth2;
    using System.Net;

    public class BaseRestService
    {
        private readonly IRestClient restClient;
        private readonly IConfidentialClientApplicationProvider confidentialClientApplicationProvider;

        private readonly Uri BaseUrl = new Uri("");
        private readonly IAuthenticator? Authenticator;
        public BaseRestService(IConfiguration configuration, IRestClient restClient, IConfidentialClientApplicationProvider confidentialClientApplicationProvider, string serviceName)
        {
            this.restClient = restClient;
            this.confidentialClientApplicationProvider = confidentialClientApplicationProvider;

            bool.TryParse(configuration[$"RestServices:{serviceName}:baseUri"], out var isOIDCEnabled);
            this.BaseUrl = new Uri(configuration[$"RestServices:{serviceName}:baseUrl"].ToString());

            var restOptions = new RestClientOptions(this.BaseUrl);

            if (isOIDCEnabled)
            {
                var result = this.GetConfidentialClientApplication(configuration, serviceName);
                this.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(result.IdToken, "Bearer");

                restOptions.Authenticator = this.Authenticator;
            }
            else
            {
                restOptions.UseDefaultCredentials = true;
            }

            this.restClient = new RestClient(restOptions);
            
        }

        private AuthenticationResult? GetConfidentialClientApplication(IConfiguration configuration, string serviceName)
        {
            AuthenticationResult result = null;
            var authority = configuration["OAuth:authority"].ToString();
            var clientId = configuration["OAuth:appIdentifier"].ToString();
            var clientSecret = configuration["OAuth:clientSecret"].ToString();
            var certificateName = configuration["OAuth:certificateName"].ToString();
            bool.TryParse(configuration[$"OAuth:useClientSecret"].ToString(), out var useClientSecret);
            var scope = $"api://{configuration[$"RestServices:{serviceName}:clientId"].ToString()}";
            try
            {
                var app = this.confidentialClientApplicationProvider.RetrieveConfidentialClientApplication(serviceName, clientId, certificateName, clientSecret, authority, useClientSecret);
                result = app.AcquireTokenForClient(new List<string>() { scope }).ExecuteAsync().GetAwaiter().GetResult();
            }
            catch(Exception ex)
            {
                throw new Exception($"Error occurred while retrieving IConfidentialClientApplication. {ex.Message}");
            }

            return result;
        }

        public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest httpRequest)
        {
            var response = this.restClient.ExecuteAsync<T>(httpRequest).GetAwaiter().GetResult();
            this.ValidateResponse<T>(response);
            return response;
        }

        public void ValidateResponse<T>(RestResponse<T> response)
        {
            if(response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case (HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized) :
                    throw new UnauthorizedAccessException();
                case (HttpStatusCode.BadRequest):
                    throw new Exception("Bad Request.");
                default:
                    throw new Exception("Error Occurred.");
            }
                 
        }
    }
}