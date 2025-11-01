using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.ModeloDatos
{
    /// <summary>
    /// Define las categorías predefinidas para clasificar una 'Actividad'.
    /// Al usar una enumeración (enum), se asegura que los datos sean consistentes
    /// y se evita que los usuarios escriban valores "a mano" (ej. "Bici", "Ciclismo", "bicicleta"),
    /// lo cual complicaría filtrar y agrupar datos.
    /// </summary>
    public enum TipoActividad
    {
        /// <summary>
        /// Actividades de ciclismo, ya sea de carretera o montaña.
        /// </summary>
        Ciclismo,

        /// <summary>
        /// Actividades de senderismo o caminata.
        /// </summary>
        Caminata,

        /// <summary>
        /// Actividades de carrera a pie.
        /// </summary>
        Running,

        /// <summary>
        /// Actividades de natación en piscina o aguas abiertas.
        /// </summary>
        Natacion,

        /// <summary>
        /// Actividades de entrenamiento en gimnasio (pesas, cardio, etc.).
        /// </summary>
        Gimnasio,

        /// <summary>
        /// Una categoría genérica para cualquier actividad que no encaje
        /// en las anteriores (ej. Yoga, Escalada, Fútbol).
        /// </summary>
        Otro
    }
}
