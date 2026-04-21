using System;
using System.Security.Cryptography;
using System.Text;
using RETAIL.BASE.NEG.Interfaces;

namespace RETAIL.BASE.NEG.Helpers
{
    public class CryptographerSHA512B : ICryptographerB
    {
        /// <summary>
        /// Compara el hash calculado del input con el hash almacenado utilizando el algoritmo indicado.
        /// </summary>
        /// <param name="input">Texto a hashear (por ejemplo, el password en texto plano)</param>
        /// <param name="hash">Hash almacenado para comparación (se supone en formato hexadecimal)</param>
        /// <returns>true si los hashes coinciden; false en caso contrario</returns>
        public bool CompareHash(string input, string hash)
        {
            // Crear la instancia del algoritmo de hash.
            HashAlgorithm hashAlgorithm;

            // Usamos SHA512CryptoServiceProvider directamente para que coincida con el cálculo original.

            hashAlgorithm = SHA512.Create();    //new SHA512CryptoServiceProvider();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = hashAlgorithm.ComputeHash(inputBytes);

            // Convertir el hash a Base64 para que coincida con el formato de storedHash
            string computedHash = Convert.ToBase64String(hashBytes);

            return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Crea un hash del texto proporcionado utilizando el algoritmo especificado.
        /// </summary>
        /// <param name="algorithmName">Name del algoritmo (ej. "SHA512")</param>
        /// <param name="plainText">Texto a hashear</param>
        /// <returns>Hash en formato Base64</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public string CreateHash(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Convertir la cadena en bytes utilizando UTF8 (asegúrate que sea la misma codificación usada originalmente)
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Crear la instancia del algoritmo de hash.
            HashAlgorithm hashAlgorithm;

            // Se utiliza directamente SHA512CryptoServiceProvider para que el resultado sea idéntico
            hashAlgorithm = SHA512.Create();    //new SHA512CryptoServiceProvider();

            // Calcular el hash y convertirlo a Base64
            using (hashAlgorithm)
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }


        public static bool VerifyPassword(string password, string storedHashBase64)
        {
            // Decodificar el hash almacenado en Base64
            byte[] storedHashBytes = Convert.FromBase64String(storedHashBase64);

            // Extraer el salt (primeros 16 bytes)
            byte[] salt = new byte[16];
            Array.Copy(storedHashBytes, 0, salt, 0, 16);

            // Extraer el hash real (64 bytes siguientes)
            byte[] storedHash = new byte[64];
            Array.Copy(storedHashBytes, 16, storedHash, 0, 64);

            // Convertir la contraseña en bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Concatenar salt + password
            byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);

            // Calcular el hash usando SHA-512
            using (SHA512 sha512 = SHA512.Create()) //new SHA512CryptoServiceProvider())
            {
                byte[] computedHash = sha512.ComputeHash(saltedPassword);
                Console.WriteLine(Convert.ToBase64String(computedHash));
                // Comparar el hash calculado con el almacenado de forma segura
                return CompareHashes(computedHash, storedHash);
            }

        }

        private static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;

            bool areEqual = true;
            for (int i = 0; i < hash1.Length; i++)
            {
                areEqual &= (hash1[i] == hash2[i]); // Comparación en tiempo constante
            }
            return areEqual;
        }
    }
}
