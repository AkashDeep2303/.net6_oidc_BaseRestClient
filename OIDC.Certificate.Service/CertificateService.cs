using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OIDC.Certificate.Service
{
    public class CertificateService
    {

        public static X509Certificate2 GetValidCertificate(X509Certificate2Collection certificates, string subjectName)
        {
            X509Certificate2 certificate = new X509Certificate2();
            foreach (X509Certificate2 cert in certificates)
            {
                var now = DateTime.Now;

                if(cert.Subject.Contains($"CN={subjectName}") && now <= cert.NotAfter && now>= cert.NotBefore)
                {
                    certificate = cert;
                    break;
                }
            }

            if(certificates == null)
            {
                throw new Exception("certificate not found.");
            }

            return certificate;
        }

    }
}
