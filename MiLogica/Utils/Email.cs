using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.Utils
{
    public static class Email
    {
        public static bool ValidarEmail(string email)
        {
            // Ejemplo de validación simple: debe contener un "@" y un "."
            if (string.IsNullOrWhiteSpace(email)) return false;
            int atIndex = email.IndexOf('@');
            int dotIndex = email.LastIndexOf('.');
            return atIndex > 0 && dotIndex > atIndex + 1 && dotIndex < email.Length - 1;
        }
    }
}
