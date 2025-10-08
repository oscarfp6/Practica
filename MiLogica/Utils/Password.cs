using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.Utils
{
    public static class Password
    {
        public static bool ValidarPassword(string password)
        {
            // Ejemplo de validación: al menos 12 caracteres, una mayúscula, una minúscula, un número y un carácter especial
            if (password.Length < 12) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(ch => !char.IsLetterOrDigit(ch))) return false;
            return true;

        }
    }
}
