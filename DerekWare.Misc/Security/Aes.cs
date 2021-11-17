using System.Security.Cryptography;

namespace DerekWare.Security
{
    public static class AES
    {
        public static byte[] Decrypt(byte[] source, byte[] key, byte[] iv)
        {
            return Transform(source, key, iv, TransformType.Decrypt);
        }

        public static byte[] Encrypt(byte[] source, byte[] key, byte[] iv)
        {
            return Transform(source, key, iv, TransformType.Encrypt);
        }

        public static byte[] Transform(byte[] source, byte[] key, byte[] iv, TransformType type)
        {
            using(var encryptor = Aes.Create())
            {
                encryptor.Mode = CipherMode.CBC;
                encryptor.Key = key;
                encryptor.IV = iv;

                using(var transform = encryptor.CreateTransform(type))
                {
                    return transform.TransformFinalBlock(source, 0, source.Length);
                }
            }
        }
    }
}
