using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.ModeloDatos
{
    public class Actividad
    {
        // --- Backing Fields ---
        // Campos privados para almacenar el valor de las propiedades.
        // La validación se hará antes de asignar un valor a estos campos.
        private double _kms;
        private int _metrosDesnivel;
        private TimeSpan _duracion;
        private DateTime _fecha;
        private TipoActividad _tipo;
        private int? _fcMedia;

        // --- Propiedades Públicas ---

        // Renombramos a "Id" para seguir la convención de EF Core para Primary Key
        public int Id { get; private set; }
        public int IdUsuario { get; private set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }


        public double Kms
        {
            get => _kms;
            set
            {
                if (!ValidarKms(value))
                {
                    throw new ArgumentException("Los kilómetros no pueden ser negativos.");
                }
                _kms = value;
            }
        }

        public int MetrosDesnivel
        {
            get => _metrosDesnivel;
            set
            {
                if (!ValidarMetrosDesnivel(value))
                {
                    throw new ArgumentException("El desnivel no puede ser negativo.");
                }
                _metrosDesnivel = value;
            }
        }

        public TimeSpan Duracion
        {
            get => _duracion;
            set
            {
                if (!ValidarDuracion(value))
                {
                    throw new ArgumentException("La duración debe ser mayor que cero.");
                }
                _duracion = value;
            }
        }

        public DateTime Fecha
        {
            get => _fecha;
            set
            {
                if (!ValidarFecha(value))
                {
                    throw new ArgumentException("La fecha no puede ser en el futuro.");
                }
                _fecha = value;
            }
        }

        public TipoActividad Tipo
        {
            get => _tipo;
            set
            {
                if (!ValidarTipoActividad(value))
                {
                    throw new ArgumentException("El tipo de actividad no es válido.");
                }
                _tipo = value;
            }
        }

        public int? FCMedia
        {
            get => _fcMedia;
            set
            {
                if (!ValidarFCMedia(value))
                {
                    throw new ArgumentException("La frecuencia cardíaca media debe estar entre 30 y 220 bpm si se proporciona.");
                }
                _fcMedia = value;
            }
        }

        // --- Propiedad de Navegación ---
        public virtual Usuario Usuario { get; set; }

        // --- Propiedades Calculadas ---
        public double RitmoMinPorKm
        {
            get
            {
                if (Kms > 0)
                {
                    return Math.Round(Duracion.TotalMinutes / Kms, 2);
                }
                return 0.0;
            }
        }

        public double VelocidadMediaKmh
        {
            get
            {
                if (Duracion.TotalHours > 0)
                {
                    return Math.Round(Kms / Duracion.TotalHours, 2);
                }
                return 0;
            }
        }

        // Constructor para Entity Framework
        private Actividad() { }

        // Constructor para ser usado por la lógica de negocio.
        // Ahora es mucho más limpio. Las asignaciones llaman a los 'setters' que ya contienen la validación.
        public Actividad(int idUsuario, string titulo, double kms, int metrosDesnivel, TimeSpan duracion, DateTime fecha, TipoActividad tipo, string descripcion = "", int? fcMedia = null)
        {
            this.IdUsuario = idUsuario;
            this.Titulo = titulo;
            this.Kms = kms; // Llama al set de Kms, que ejecuta la validación
            this.MetrosDesnivel = metrosDesnivel; // Llama al set de MetrosDesnivel
            this.Duracion = duracion; // Llama al set de Duracion
            this.Fecha = fecha; // Llama al set de Fecha
            this.Tipo = tipo; // Llama al set de Tipo
            this.Descripcion = descripcion;
            this.FCMedia = fcMedia; // Llama al set de FCMedia
        }

        // Este método ahora es inherentemente seguro. Cada asignación dispara la validación correspondiente.
        public bool ActualizarMetricas( string titulo, double kms, int metrosDesnivel, TimeSpan duracion, TipoActividad tipo, string descripcion = "", int? fcMedia = null)
        {
            Titulo = titulo;
            Kms = kms;
            MetrosDesnivel = metrosDesnivel;
            Duracion = duracion;
            Tipo = tipo;
            Descripcion = descripcion;
            FCMedia = fcMedia;

            return true;
        }

        // --- Métodos de Validación Privados ---
        // Estos métodos se mantienen igual, ya que contienen la lógica pura de validación.
        private static bool ValidarKms(double kms) => kms >= 0;
        private static bool ValidarDuracion(TimeSpan duracion) => duracion.TotalSeconds > 0;
        private static bool ValidarFCMedia(int? fcMedia) => !fcMedia.HasValue || (fcMedia.Value >= 30 && fcMedia.Value <= 220);
        private static bool ValidarMetrosDesnivel(int metrosDesnivel) => metrosDesnivel >= 0;
        private static bool ValidarTipoActividad(TipoActividad tipo) => Enum.IsDefined(typeof(TipoActividad), tipo);
        private static bool ValidarFecha(DateTime fecha) => fecha <= DateTime.Now;

        public override string ToString()
        {
            if (FCMedia.HasValue)
            {
                return $"{Tipo}: {Kms:F2} km en {Duracion:hh\\:mm\\:ss} el {Fecha:dd/MM/yyyy} con FC media de {FCMedia} bpm. Ritmo: {RitmoMinPorKm:F2} min/km.";
            }
            else
            {
                return $"{Tipo}: {Kms:F2} km en {Duracion:hh\\:mm\\:ss} el {Fecha:dd/MM/yyyy}. Ritmo: {RitmoMinPorKm:F2} min/km.";
            }
        }

        internal void AsignarIdParaPersistencia(int id)
        {
            // Podríamos añadir una validación para asegurar que el ID solo se asigna una vez.
            if (this.Id == 0)
            {
                this.Id = id;
            }
        }
    }
}