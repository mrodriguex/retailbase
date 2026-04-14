// using System;
// using System.Security.Cryptography;
// using System.Text;

// namespace HARD.CORE.OBJ
// {
//     public static class CryptographerB
//     {
//         /// <summary>
//         /// Compara el hash calculado del input con el hash almacenado utilizando el algoritmo indicado.
//         /// </summary>
//         /// <param name="algorithmName">Nombre del algoritmo (ej. "SHA512")</param>
//         /// <param name="input">Texto a hashear (por ejemplo, el password en texto plano)</param>
//         /// <param name="storedHash">Hash almacenado para comparación (se asume en formato hexadecimal)</param>
//         /// <returns>true si los hashes coinciden; false en caso contrario</returns>
//         public static bool CompareHash(string algorithmName, string input, string storedHash)
//         {
//             // Crear la instancia del algoritmo de hash.
//             HashAlgorithm hashAlgorithm;
//             if (string.Equals(algorithmName, "SHA512CryptoServiceProvider", StringComparison.OrdinalIgnoreCase))
//             {
//                 // Usamos SHA512CryptoServiceProvider directamente para que coincida con el cálculo original.
//                 hashAlgorithm = new SHA512CryptoServiceProvider();
//                 byte[] inputBytes = Encoding.UTF8.GetBytes(input);
//                 byte[] hashBytes = hashAlgorithm.ComputeHash(inputBytes);

//                 // Convertir el hash a Base64 para que coincida con el formato de storedHash
//                 string computedHash = Convert.ToBase64String(hashBytes);

//                 return string.Equals(computedHash, storedHash, StringComparison.OrdinalIgnoreCase);
//             }
//             else
//             {
//                 hashAlgorithm = HashAlgorithm.Create(algorithmName)
//                     ?? throw new ArgumentException("Algoritmo no válido", nameof(algorithmName));
//             }

//             return (false);
//         }

//         public static string CreateHash(string algorithmName, string plainText)
//         {
//             if (plainText == null)
//                 throw new ArgumentNullException(nameof(plainText));

//             // Convertir la cadena en bytes utilizando UTF8 (asegúrate que sea la misma codificación usada originalmente)
//             byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

//             // Crear la instancia del algoritmo de hash.
//             HashAlgorithm hashAlgorithm;
//             if (string.Equals(algorithmName, "SHA512CryptoServiceProvider", StringComparison.OrdinalIgnoreCase))
//             {
//                 // Se utiliza directamente SHA512CryptoServiceProvider para que el resultado sea idéntico
//                 hashAlgorithm = new SHA512CryptoServiceProvider();
//             }
//             else
//             {
//                 hashAlgorithm = HashAlgorithm.Create(algorithmName)
//                     ?? throw new ArgumentException("Algoritmo no válido", nameof(algorithmName));
//             }

//             // Calcular el hash y convertirlo a Base64
//             using (hashAlgorithm)
//             {
//                 byte[] hashBytes = hashAlgorithm.ComputeHash(plainTextBytes);
//                 return Convert.ToBase64String(hashBytes);
//             }
//         }
//         public static bool VerifyPassword(string password, string storedHashBase64)
//         {
//             // Decodificar el hash almacenado en Base64
//             byte[] storedHashBytes = Convert.FromBase64String(storedHashBase64);

//             // Extraer el salt (primeros 16 bytes)
//             byte[] salt = new byte[16];
//             Array.Copy(storedHashBytes, 0, salt, 0, 16);

//             // Extraer el hash real (64 bytes siguientes)
//             byte[] storedHash = new byte[64];
//             Array.Copy(storedHashBytes, 16, storedHash, 0, 64);

//             // Convertir la contraseña en bytes
//             byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

//             // Concatenar salt + password
//             byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];
//             Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
//             Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);

//             // Calcular el hash usando SHA-512
//             using (SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider())
//             {
//                 byte[] computedHash = sha512.ComputeHash(saltedPassword);
//                 Console.WriteLine(Convert.ToBase64String(computedHash));
//                 // Comparar el hash calculado con el almacenado de forma segura
//                 return CompareHashes(computedHash, storedHash);
//             }
//         }

//         private static bool CompareHashes(byte[] hash1, byte[] hash2)
//         {
//             if (hash1.Length != hash2.Length)
//                 return false;

//             bool areEqual = true;
//             for (int i = 0; i < hash1.Length; i++)
//             {
//                 areEqual &= (hash1[i] == hash2[i]); // Comparación en tiempo constante
//             }
//             return areEqual;
//         }
//     }
// }
