using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Fhi.EikUtforsker.Helpers
{
    public static class DekryptHelper
    {
        public static byte[] DekrypterLmrEikNøkkel(string keyCipherValue, StoreName storeName, StoreLocation storeLocation, string thumbprint)
        {
            var forventetPrefix = "eik:";

            var certificate = CertificateHelper.GetCertificate(storeName, storeLocation, thumbprint);

            // Decode keyCipherValue from base64
            var keyEncryptedBytes = Convert.FromBase64String(keyCipherValue);

            // Decrypt AES key with certificate
            byte[] dekryptert = X509RsaHelper.DecryptWithPrivateX509(keyEncryptedBytes, certificate);

            var dekryptertString = Encoding.UTF8.GetString(dekryptert);

            if (!dekryptertString.StartsWith(forventetPrefix))
            {
                throw new Exception($"Symmetrisk nøkkel var ikke prefikset med \"{forventetPrefix}\"");
            }

            var base64 = dekryptertString.Substring(forventetPrefix.Length);
            var aesKey = Convert.FromBase64String(base64);

            if (aesKey.Length != AesGcmHelper.KEY_BYTES)
            {
                throw new Exception($"Wrong AES key size: {aesKey.Length} bytes. It should be {AesGcmHelper.KEY_BYTES} bytes");
            }
            return aesKey;
        }

        public static byte[] DekrypterNøkkel(string keyCipherValue, StoreName storeName, StoreLocation storeLocation, string thumbprint)
        {
            var certificate = CertificateHelper.GetCertificate(storeName, storeLocation, thumbprint);

            // Decode keyCipherValue from base64
            var keyEncryptedBytes = Convert.FromBase64String(keyCipherValue);

            // Decrypt AES key with certificate
            byte[] aesKey = X509RsaHelper.DecryptWithPrivateX509(keyEncryptedBytes, certificate);

            if (aesKey.Length != AesGcmHelper.KEY_BYTES)
            {
                throw new Exception($"Wrong AES key size: {aesKey.Length} bytes. It should be {AesGcmHelper.KEY_BYTES} bytes");
            }
            return aesKey;
        }

        public static string DekrypterBase64Cipher(string cipherData, byte[] aesKey)
        {
            // Decode cipherData from base64
            var dataEncryptedBytes = Convert.FromBase64String(cipherData);

            // Split the byte array into three parts: nonce, cipher and tag
            var (nonce, ciphertag) = ArrayHelper.Split(dataEncryptedBytes, AesGcmHelper.NONCE_BYTES);
            var tag = ArrayHelper.Sub(ciphertag, ciphertag.Length - AesGcmHelper.TAG_BYTES, AesGcmHelper.TAG_BYTES);
            var cipher = ArrayHelper.Sub(ciphertag, 0, ciphertag.Length - AesGcmHelper.TAG_BYTES);

            // Decrypt cipher using key 
            var data = AesGcmHelper.Decrypt(cipher: cipher, key: aesKey, nonce: nonce, tag: tag);
            var dataAsString = Encoding.UTF8.GetString(data);
            return dataAsString;
        }
    }
}
