using System.Linq; // Necesario para usar métodos de extensión como FirstOrDefault, Any, etc.
using System.Text.RegularExpressions; // Necesario para usar expresiones regulares (Regex) para IBAN

namespace MiLogica.Utils
{
    /// <summary>
    /// Proporciona métodos de utilidad estáticos para validaciones complejas de formatos
    /// como NIF, IBAN, y validación básica de nombres.
    /// </summary>
    public static class Valid
    {
        /// <summary>
        /// Valida el formato y la letra de control de un NIF español.
        /// </summary>
        /// <param name="nif">La cadena de NIF a validar.</param>
        /// <returns>True si el NIF es válido según el algoritmo, False en caso contrario.</returns>
        public static bool NIF(string nif)
        {
            // 1. Validación de estructura (Caja Negra: Longitud y no nulo/vacío)
            if (string.IsNullOrWhiteSpace(nif) || nif.Length != 9)
                return false;

            // 2. Extracción de partes
            string numeros = nif.Substring(0, 8);
            char letra = char.ToUpper(nif[8]); // La letra de control debe ser mayúscula

            // 3. Validación de tipo (Asegurar que los 8 primeros son números)
            if (!int.TryParse(numeros, out int numero))
                return false; // Los números son inválidos o no se pudieron parsear

            // 4. Algoritmo de verificación
            string letras = "TRWAGMYFPDXBNJZSQVHLCKE";
            // El índice de la letra correcta se obtiene del resto de la división del número entre 23
            char letraCorrecta = letras[numero % 23];

            // 5. Comparación (Caja Blanca: Comprobar la lógica del algoritmo)
            return letra == letraCorrecta;
        }

        // Validación de IBAN español
        /// <summary>
        /// Valida el formato y el dígito de control de un IBAN español (código ES).
        /// Utiliza el algoritmo MOD 97-10.
        /// </summary>
        public static bool IBAN(string iban)
        {
            // 1. Validación de estructura básica
            if (string.IsNullOrWhiteSpace(iban))
                return false;

            // 2. Normalización y comprobación de formato (ESxx + 22 dígitos)
            iban = iban.Replace(" ", "").ToUpper();

            // Usa RegEx para verificar el patrón exacto del IBAN español (ES + 22 dígitos)
            if (!Regex.IsMatch(iban, @"^ES\d{22}$"))
                return false;

            // 3. Reformulación (Algoritmo de Modificación)
            // Mover los 4 primeros caracteres (ES + 2 dígitos de control) al final
            string reformulado = iban.Substring(4) + iban.Substring(0, 4);

            // 4. Conversión al formato numérico del algoritmo
            string numerico = "";
            foreach (char c in reformulado)
            {
                if (char.IsLetter(c))
                    // Las letras se convierten a su valor numérico (A=10, B=11, ..., Z=35)
                    numerico += (c - 'A' + 10).ToString();
                else
                    numerico += c; // Los dígitos se mantienen igual
            }

            // 5. Validación con el algoritmo Modulo 97 (Debe dar 1)
            return Modulo97(numerico) == 1;
        }

        /// <summary>
        /// Algoritmo de cálculo iterativo de Módulo 97 (usado para la validación de IBAN).
        /// </summary>
        private static int Modulo97(string input)
        {
            // Este método maneja cadenas numéricas muy largas fragmentando la división.
            string fragmento = "";
            foreach (char c in input)
            {
                fragmento += c;
                // Procesamos fragmentos de 9 dígitos para evitar desbordamiento de 'int'
                if (fragmento.Length >= 9)
                {
                    int num = int.Parse(fragmento);
                    // Aplicamos el módulo y el resultado se convierte en el nuevo fragmento.
                    fragmento = (num % 97).ToString();
                }
            }
            // El resultado final del proceso iterativo se devuelve módulo 97.
            return int.Parse(fragmento) % 97;
        }

        /// <summary>
        /// Valida que una cadena de nombre no esté vacía y no contenga dígitos.
        /// </summary>
        /// <param name="input">La cadena a validar (nombre o apellido).</param>
        /// <returns>True si es un nombre/apellido válido, False en caso contrario.</returns>
        public static bool Nombre(string input)
        {
            // 1. Comprobación de existencia
            if (string.IsNullOrWhiteSpace(input)) return false;

            // 2. Comprobación de dígitos
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    return false; // Contiene un número
                }
            }

            return true; // Pasa ambas validaciones

        }

        //El resto de métodos de validación están en Password.cs y Email.cs
        // Nota: Los comentarios son solo una descripción informativa aquí.
        // La implementación se encuentra en los archivos separados.
    }
}
