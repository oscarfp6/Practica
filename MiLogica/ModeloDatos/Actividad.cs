using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.ModeloDatos
{
    /// <summary>
    /// Representa una actividad física o de ocio registrada por un usuario.
    /// Esta clase implementa la validación de dominio: se protege a sí misma
    /// de estados inválidos lanzando excepciones si se intentan asignar
    /// valores incorrectos a sus propiedades.
    /// </summary>
    public class Actividad
    {
        // --- Backing Fields (Campos de Respaldo) ---
        // Campos privados que almacenan el valor real de las propiedades.
        // Se usan para poder ejecutar lógica (validación) en los 'setters'.
        private double _kms;
        private int _metrosDesnivel;
        private TimeSpan _duracion;
        private DateTime _fecha;
        private TipoActividad _tipo;
        private int? _fcMedia; // 'int?' significa 'Nullable<int>', puede ser 'null'.

        // --- Propiedades Públicas ---

        /// <summary>
        /// Identificador único (Clave Primaria) de la actividad.
        /// </summary>
        public int Id { get; /*cambiar a privado con referencias*/ set; }

        /// <summary>
        /// Identificador del usuario (Clave Foránea) al que pertenece la actividad.
        /// Solo se puede establecer en el constructor.
        /// </summary>
        public int IdUsuario { get; private set; }

        /// <summary>
        /// Título descriptivo de la actividad (ej. "Carrera matutina").
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Descripción opcional más detallada de la actividad.
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Distancia de la actividad en kilómetros.
        /// </summary>
        public double Kms
        {
            get => _kms; // Expresión 'get' concisa (body expression)
            set
            {
                // VALIDACIÓN: Se ejecuta CADA VEZ que se asigna 'actividad.Kms = valor'
                if (!ValidarKms(value))
                {
                    // Lanza una excepción si la validación falla. Esto detiene
                    // la ejecución y previene que el objeto entre en estado inválido.
                    throw new ArgumentException("Los kilómetros no pueden ser negativos.");
                }
                _kms = value; // Solo asigna si la validación pasa.
            }
        }

        /// <summary>
        /// Desnivel positivo acumulado en metros.
        /// </summary>
        public int MetrosDesnivel
        {
            get => _metrosDesnivel;
            set
            {
                // VALIDACIÓN:
                if (!ValidarMetrosDesnivel(value))
                {
                    throw new ArgumentException("El desnivel no puede ser negativo.");
                }
                _metrosDesnivel = value;
            }
        }

        /// <summary>
        /// Duración total de la actividad.
        /// </summary>
        public TimeSpan Duracion
        {
            get => _duracion;
            set
            {
                // VALIDACIÓN:
                if (!ValidarDuracion(value))
                {
                    throw new ArgumentException("La duración debe ser mayor que cero.");
                }
                _duracion = value;
            }
        }

        /// <summary>
        /// Fecha y hora en que se realizó la actividad.
        /// </summary>
        public DateTime Fecha
        {
            get => _fecha;
            set
            {
                // VALIDACIÓN:
                if (!ValidarFecha(value))
                {
                    throw new ArgumentException("La fecha no puede ser en el futuro.");
                }
                _fecha = value;
            }
        }

        /// <summary>
        /// Tipo de actividad (ej. Running, Ciclismo).
        /// </summary>
        public TipoActividad Tipo
        {
            get => _tipo;
            set
            {
                // VALIDACIÓN:
                if (!ValidarTipoActividad(value))
                {
                    throw new ArgumentException("El tipo de actividad no es válido.");
                }
                _tipo = value;
            }
        }

        /// <summary>
        /// Frecuencia Cardíaca Media (opcional).
        /// </summary>
        public int? FCMedia
        {
            get => _fcMedia;
            set
            {
                // VALIDACIÓN:
                if (!ValidarFCMedia(value))
                {
                    throw new ArgumentException("La frecuencia cardíaca media debe estar entre 30 y 220 bpm si se proporciona.");
                }
                _fcMedia = value;
            }
        }

        // --- Propiedad de Navegación (para Entity Framework) ---
        /// <summary>
        /// Propiedad de navegación virtual que EF Core usaría para
        /// cargar el objeto Usuario relacionado (IdUsuario).
        /// </summary>
        public virtual Usuario Usuario { get; set; }

        // --- Propiedades Calculadas ---
        // Estas propiedades no se guardan en la BD, se calculan en tiempo real.

        /// <summary>
        /// Calcula el ritmo en minutos por kilómetro.
        /// </summary>
        public double RitmoMinPorKm
        {
            get
            {
                if (Kms > 0)
                {
                    // Cálculo: (Minutos Totales) / Kms
                    return Math.Round(Duracion.TotalMinutes / Kms, 2);
                }
                return 0.0; // Evita división por cero.
            }
        }

        /// <summary>
        /// Calcula la velocidad media en kilómetros por hora.
        /// </summary>
        public double VelocidadMediaKmh
        {
            get
            {
                if (Duracion.TotalHours > 0)
                {
                    // Cálculo: Kms / (Horas Totales)
                    return Math.Round(Kms / Duracion.TotalHours, 2);
                }
                return 0; // Evita división por cero.
            }
        }

        /// <summary>
        /// Constructor simple para crear una actividad con valores mínimos
        /// (ej. al crearla en la BD antes de tener todos los datos).
        /// </summary>
        /// <param name="idUsuario">ID del usuario (obligatorio).</param>
        /// <param name="titulo">Título de la actividad (obligatorio).</param>
        public Actividad(int idUsuario, string titulo)
        {
            // Valida el IdUsuario aquí para evitar actividades "huérfanas".
            if (idUsuario > 0)
            {
                this.IdUsuario = idUsuario;
                this.Titulo = titulo;

                // --- Valores por Defecto ---
                this.Descripcion = "";
                this.Tipo = TipoActividad.Otro; // Valor seguro por defecto
                this.Fecha = DateTime.Now;     // Valor seguro por defecto
                this.Duracion = TimeSpan.FromMinutes(1); // Valor seguro por defecto (evita 'duración <= 0')
                // Kms y MetrosDesnivel ya son 0 por defecto.
                // FCMedia ya es null por defecto.
            }
            // (Si idUsuario no es > 0, el objeto se crea pero IdUsuario será 0,
            // lo cual fallará al intentar guardarlo en CapaDatos. Es una forma de validación).
        }

        /// <summary>
        /// Constructor completo para crear una actividad con todos los datos.
        /// Utilizado por la lógica de negocio o al leer desde la BD.
        /// </summary>
        public Actividad(int idUsuario, string titulo, double kms, int metrosDesnivel, TimeSpan duracion, DateTime fecha, TipoActividad tipo, string descripcion = "", int? fcMedia = null)
        {
            this.IdUsuario = idUsuario;
            this.Titulo = titulo;
            this.Descripcion = descripcion;

            // Al asignar a las PROPIEDADES (ej. this.Kms),
            // se ejecutan automáticamente los 'setters' y, por tanto,
            // la validación que contienen.
            this.Kms = kms;
            this.MetrosDesnivel = metrosDesnivel;
            this.Duracion = duracion;
            this.Fecha = fecha;
            this.Tipo = tipo;
            this.FCMedia = fcMedia;
        }


        /* Método ActualizarMetricas (comentado en el original)
           Efectivamente, como indica el comentario, este método es redundante
           porque la capa de datos (CapaDatos.ActualizaActividad) ya hace este
           trabajo. Mantenerlo podría causar confusión sobre quién es responsable
           de la actualización.
        */

        // --- Métodos de Validación Privados ---
        // Lógica de negocio pura, separada en métodos estáticos y privados.
        // Son 'static' porque no dependen del estado de una instancia (this).

        private static bool ValidarKms(double kms) => kms >= 0;
        private static bool ValidarDuracion(TimeSpan duracion) => duracion.TotalSeconds > 0;
        private static bool ValidarFCMedia(int? fcMedia) =>
            !fcMedia.HasValue || (fcMedia.Value >= 30 && fcMedia.Value <= 220);
        private static bool ValidarMetrosDesnivel(int metrosDesnivel) => metrosDesnivel >= 0;
        private static bool ValidarTipoActividad(TipoActividad tipo) => Enum.IsDefined(typeof(TipoActividad), tipo);
        private static bool ValidarFecha(DateTime fecha) => fecha <= DateTime.Now;

        /// <summary>
        /// Sobrescribe el método ToString() para devolver una representación
        /// legible de la actividad, útil para depuración o logs.
        /// </summary>
        public override string ToString()
        {
            // Formato condicional: muestra FCMedia solo si existe.
            if (FCMedia.HasValue)
            {
                // F2 = Formato con 2 decimales
                // hh\\:mm\\:ss = Formato de horas:minutos:segundos
                return $"{Tipo}: {Kms:F2} km en {Duracion:hh\\:mm\\:ss} el {Fecha:dd/MM/yyyy} con FC media de {FCMedia} bpm. Ritmo: {RitmoMinPorKm:F2} min/km.";
            }
            else
            {
                return $"{Tipo}: {Kms:F2} km en {Duracion:hh\\:mm\\:ss} el {Fecha:dd/MM/yyyy}. Ritmo: {RitmoMinPorKm:F2} min/km.";
            }
        }

        /// <summary>
        /// Método interno, visible solo dentro del mismo proyecto (MiLogica),
        /// usado por CapaDatos para asignar el ID después de guardar
        /// en la "base de datos" (simulada).
        /// </summary>
        /// <param name="id">El nuevo ID asignado por la capa de datos.</param>
        internal void AsignarIdParaPersistencia(int id)
        {
            // Protección para evitar cambiar el ID si ya tiene uno.
            if (this.Id == 0)
            {
                this.Id = id;
            }
        }
    }
}
