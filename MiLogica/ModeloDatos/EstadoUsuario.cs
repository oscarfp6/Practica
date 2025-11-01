using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.ModeloDatos
{
    /// <summary>
    /// Define los posibles estados en los que se puede encontrar una cuenta de usuario.
    /// Un 'enum' (enumeración) es un tipo de dato especial que consiste en un conjunto
    /// de constantes con nombre, lo que hace el código más legible y mantenible
    /// que usar números mágicos (ej. 0, 1, 2).
    /// </summary>
    public enum EstadoUsuario
    {
        /// <summary>
        /// El usuario puede iniciar sesión y utilizar la aplicación con normalidad.
        /// </summary>
        Activo,

        /// <summary>
        /// El usuario ha estado inactivo por un período prolongado (ej. 6 meses).
        /// Todavía puede iniciar sesión (lo que lo reactivará), pero un fallo
        /// de contraseña en este estado puede llevar a un bloqueo inmediato.
        /// </summary>
        Inactivo,

        /// <summary>
        /// El usuario ha fallado al iniciar sesión múltiples veces (ej. 3 intentos).
        /// No se le permite iniciar sesión, incluso con la contraseña correcta,
        /// hasta que pase un período de "cooldown" o sea desbloqueado.
        /// </summary>
        Bloqueado
    }
}
