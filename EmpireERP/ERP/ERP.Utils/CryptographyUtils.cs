using System.Security.Cryptography;
using System.Text;

namespace ERP.Utils
{
    public static class CryptographyUtils
    {
        /// <summary>
        /// Вычисление значения хэш-функции
        /// </summary>
        /// <param name="text">исходный текст</param>
        /// <returns>вычисленное значение хэш-функции</returns>
        public static string ComputeHash(string text)
        {
            SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();
            byte[] hashByte = sha512.ComputeHash(Encoding.UTF8.GetBytes(text));

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashByte)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }
    }
}
