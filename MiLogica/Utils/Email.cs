using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; // Importado, aunque no se usa en este método
using System.Threading.Tasks;


namespace MiLogica.Utils
{
    /// <summary>
    /// Proporciona métodos de utilidad estáticos para tareas de validación.
    /// Al ser 'static', no necesitamos crear una instancia de 'Email'
    /// para usar sus métodos (ej. Email.ValidarEmail(...)).
    /// </summary>
    public static class Email
    {

        /// <summary>
        /// Valida si una cadena de texto tiene un formato de email básico.
        /// Esta es una validación de "caja blanca": conocemos las reglas
        /// internas (debe tener '@', '.', no '..', etc.) y las probamos.
        /// </summary>
        /// <param name="email">El email a validar.</param>
        /// <returns>True si el formato es aceptable, False si no lo es.</returns>
        public static bool ValidarEmail(string email)
        {
            // --- Prueba de Caja Negra (Valor Límite) ---
            // Caso 1: ¿Qué pasa si el email es nulo, vacío o solo espacios?
            // 'IsNullOrWhiteSpace' es la forma más robusta de comprobar esto.
            if (string.IsNullOrWhiteSpace(email)) return false;

            // --- Prueba de Caja Blanca (Caso Específico Inválido) ---
            // Caso 2: Un email no puede tener dos puntos seguidos (ej. "test@gmail..com")
            if (email.Contains("..")) return false;

            // --- Lógica de Particiones de Equivalencia ---
            // Un email válido se parte en 3: [local]@[dominio].[tld]
            // Buscamos la posición del '@' y del ÚLTIMO '.'

            // Caso 3: Encontrar el '@'.
            int atIndex = email.IndexOf('@');

            // Caso 4: Encontrar el ÚLTIMO '.' (para manejar subdominios ej. "test@mail.google.com")
            int dotIndex = email.LastIndexOf('.');

            // --- Validación de Reglas/Estructura ---
            // 1. (atIndex > 0): Debe haber un '@' y no puede ser el primer caracter.
            // 2. (dotIndex > atIndex + 1): Debe haber un '.' DESPUÉS del '@' y no justo pegado (ej. "test@.com").
            // 3. (dotIndex < email.Length - 1): No puede ser el último caracter (ej. "test@gmail.").
            return atIndex > 0 && dotIndex > atIndex + 1 && dotIndex < email.Length - 1;
        }
    }



}
