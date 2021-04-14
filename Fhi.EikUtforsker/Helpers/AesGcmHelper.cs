using System.Security.Cryptography;

namespace Fhi.EikUtforsker.Helpers
{
    public static class AesGcmHelper
    {
        public const int TAG_BYTES = 16;
        public const int NONCE_BYTES = 12;
        public const int KEY_BYTES = 32;

        public static byte[] Encrypt(byte[] input, byte[] key, byte[] nonce)
        {
            byte[] tag = new byte[TAG_BYTES];
            byte[] cipher = new byte[input.Length];

            using var aesGsm = new AesGcm(key);
            aesGsm.Encrypt(nonce, input, cipher, tag);
            return ArrayHelper.Concat(nonce, ArrayHelper.Concat(tag, cipher));
        }

        public static byte[] Decrypt(byte[] cipher, byte[] key, byte[] nonce, byte[] tag)
        {
            byte[] decryptedData = new byte[cipher.Length];
            using var aesGsm = new AesGcm(key);
            aesGsm.Decrypt(nonce, cipher, tag, decryptedData);
            return decryptedData;
        }

        public static (byte[] key, byte[] nonce) GetKeyAndNonce()
        {
            byte[] key = new byte[KEY_BYTES];
            byte[] nonce = new byte[NONCE_BYTES];
            RandomNumberGenerator.Fill(key);
            RandomNumberGenerator.Fill(nonce);
            return (key, nonce);
        }
    }
}
