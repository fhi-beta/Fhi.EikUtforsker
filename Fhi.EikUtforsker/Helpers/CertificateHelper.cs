using System;
using System.Security.Cryptography.X509Certificates;

namespace Fhi.EikUtforsker.Helpers
{
    public static class CertificateHelper
    {
        public static X509Certificate2 GetCertificate(StoreName storeName, StoreLocation storeLocation, string thumbprint)
        {
            using (var store = new X509Store(storeName, storeLocation))
            {
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
                if (certificates.Count == 0)
                {
                    throw new Exception($"Finner ikke certificate med thumbprint {thumbprint}");
                }
                store.Close();
                return certificates[0];
            }
        }
    }
}
