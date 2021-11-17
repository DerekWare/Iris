using System;
using System.Security.Cryptography;

namespace DerekWare.Security
{
    public enum TransformType
    {
        Decrypt,
        Encrypt
    }

    public static class Transform
    {
        public static ICryptoTransform CreateTransform(this SymmetricAlgorithm encryptor, TransformType type)
        {
            switch(type)
            {
                case TransformType.Decrypt:
                    return encryptor.CreateDecryptor();

                case TransformType.Encrypt:
                    return encryptor.CreateEncryptor();

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
