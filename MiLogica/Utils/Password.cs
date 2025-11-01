using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.Utils
{
    /// <summary>
    /// Proporciona métodos estáticos para aplicar políticas de seguridad
    /// en las contraseñas antes de su encriptación.
    /// </summary>
    public static class Password
    {
        /// <summary>
        /// Valida si una contraseña cumple con los requisitos mínimos de seguridad.
        /// </summary>
        /// <param name="password">La contraseña en texto plano.</param>
        /// <returns>True si es segura, False si no cumple algún requisito.</returns>
        public static bool ValidarPassword(string password)
        {
            // --- Reglas de Validación (Requisitos de Seguridad) ---

            // Requisito 1: Longitud Mínima (12 caracteres).
            // Este es el primer filtro y una de las medidas más importantes contra ataques de fuerza bruta.
            if (password.Length < 12) return false;

            // Requisito 2: Al menos una letra Mayúscula.
            // Utiliza LINQ para comprobar si existe algún carácter que sea mayúscula.
            if (!password.Any(char.IsUpper)) return false;

            // Requisito 3: Al menos una letra Minúscula.
            if (!password.Any(char.IsLower)) return false;

            // Requisito 4: Al menos un Dígito numérico (0-9).
            if (!password.Any(char.IsDigit)) return false;

            // Requisito 5: Al menos un Carácter Especial (no alfanumérico).
            // Comprueba si existe algún carácter que NO sea una letra o un dígito.
            if (!password.Any(ch => !char.IsLetterOrDigit(ch))) return false;

            // Si pasa todos los filtros, la contraseña es considerada segura.
            return true;
        }
    }
}
