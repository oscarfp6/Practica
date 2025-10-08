using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.ModeloDatos
{
    public class Actividad
    {
        // Renombramos a "Id" para seguir la convención de EF Core para Primary Key
        public int Id { get;  set; }

        // --- Clave Foránea ---
        public int IdUsuario { get;  set; }

        public string Titulo { get; set; }

        public double Kms { get; set; }
        public int MetrosDesnivel { get; set; }
        public TimeSpan Duracion { get; set; }
        public DateTime Fecha { get; set; }
        public TipoActividad Tipo { get; set; }
        public String Descripcion { get; set; }
        public int? FCMedia { get; set; }

        // --- Propiedad de Navegación ---
        // Esto le dice a EF Core que cada Actividad pertenece a un Usuario.
        public virtual Usuario Usuario { get; set; }

        /// <summary>
        /// Propiedad calculada. Devuelve el ritmo en minutos por kilómetro.
        /// Útil para actividades como Running o Caminata.
        /// </summary>
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

        private Actividad() { }


        // Constructor para ser usado por la lógica de negocio
        public Actividad(int idUsuario, double kms, int metrosDesnivel, TimeSpan duracion, DateTime fecha, TipoActividad tipo, string descripcion = "", int? fcMedia=null)
        {
            // Validación de datos en el constructor para asegurar la integridad del objeto
            ValidarMetricas(kms, duracion);

            this.IdUsuario = idUsuario;
            this.Kms = kms;
            this.MetrosDesnivel = metrosDesnivel;
            this.Duracion = duracion;
            this.Fecha = fecha;
            this.Tipo = tipo;
            this.Descripcion = descripcion;
            this.FCMedia = fcMedia;
        }

        public void ActualizarMetricas(double kms, int metrosDesnivel, TipoActividad tipo, TimeSpan duracion, string descripcion = "", int? fcMedia = null)
        {
            ValidarMetricas(kms, duracion);
            
            Kms = kms;
            MetrosDesnivel = metrosDesnivel;
            Duracion = duracion;
            Descripcion = descripcion;
            FCMedia = fcMedia;
            Tipo = tipo;
        }

        private static void ValidarMetricas(double kms, TimeSpan duracion) 
        {
            if (kms < 0) throw new ArgumentException("Los kilómetros no pueden ser negativos.");
            if (duracion.TotalSeconds <= 0) throw new ArgumentException("La duración debe ser positiva.");
        }

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
    }
}