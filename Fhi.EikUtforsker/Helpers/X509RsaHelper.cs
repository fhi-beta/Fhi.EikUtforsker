using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Fhi.EikUtforsker.Helpers
{
    public static class X509RsaHelper
    {
        public static byte[] DecryptWithPrivateX509(byte[] dataEncryptedBytes, X509Certificate2 x509)
        {
            RSA privateKey = x509.GetRSAPrivateKey();
            byte[] decrypted = privateKey.Decrypt(dataEncryptedBytes, RSAEncryptionPadding.Pkcs1);
            return decrypted;
        }
    }
}
