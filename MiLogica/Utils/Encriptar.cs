using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography; // Necesario para usar SHA256


namespace MiLogica.Utils
{
    /// <summary>
    /// Proporciona métodos estáticos para la encriptación de datos,
    /// específicamente para el hashing de contraseñas.
    /// </summary>
    public static class Encriptar
    {
        /// <summary>
        /// Genera un hash SHA256 de una cadena de texto (contraseña) dada.
        /// Este proceso es unidireccional (no se puede revertir).
        /// </summary>
        /// <param name="password">La contraseña en texto plano.</param>
        /// <returns>La representación de la contraseña como una cadena hexadecimal SHA256.</returns>
        public static string EncriptarPasswordSHA256(string password)
        {
            // 1. Crea una instancia del algoritmo SHA256 (implementado en .NET).
            using (SHA256 sha256 = SHA256.Create())
            {
                // 2. Convierte el texto de entrada a un array de bytes (usando codificación UTF-8).
                //    Esto es crucial ya que los algoritmos de hash operan sobre bytes.
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                // 3. Computa el hash de los bytes.
                //    'ComputeHash' aplica la función matemática SHA256, devolviendo 32 bytes (256 bits).
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // 4. Convierte el array de bytes del hash a una cadena hexadecimal.
                //    Una cadena hexadecimal es la forma estándar de almacenar y comparar hashes.
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    // Formatea cada byte como dos caracteres hexadecimales ('x2').
                    // Esto resulta en la cadena de 64 caracteres que usas para _passwordHash.
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
