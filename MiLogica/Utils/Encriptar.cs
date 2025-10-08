using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace MiLogica.Utils
{
    public static class Encriptar
    {
        public static string EncriptarPasswordSHA256(string password)
        {
            // 1. Crea una instancia del algoritmo SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                // 2. Convierte el texto de entrada a un array de bytes
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                // 3. Computa el hash de los bytes
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // 4. Convierte el array de bytes del hash a una cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    // Formatea cada byte como dos caracteres hexadecimales
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
