using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.ModeloDatos.Tests
{
    [TestClass()]
    public class ActividadTests
    {
        #region Pruebas de Validación (Lanzamiento de Excepciones)

        /// <summary>
        /// Comprueba que no se puede crear una actividad con kilómetros negativos.
        /// </summary>
        [TestMethod()]
        public void Constructor_ConKmsNegativos_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Actividad(1, "Test", -10.0, 100, TimeSpan.FromHours(1), DateTime.Now, TipoActividad.Running)
            );
        }

        /// <summary>
        /// Comprueba que no se puede crear una actividad con desnivel negativo.
        /// </summary>
        [TestMethod()]
        public void Constructor_ConDesnivelNegativo_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Actividad(1, "Test", 10.0, -100, TimeSpan.FromHours(1), DateTime.Now, TipoActividad.Ciclismo)
            );
        }

        /// <summary>
        /// Comprueba que la duración de la actividad no puede ser cero.
        /// </summary>
        [TestMethod()]
        public void Constructor_ConDuracionCero_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Actividad(1, "Test", 10.0, 100, TimeSpan.Zero, DateTime.Now, TipoActividad.Caminata)
            );
        }

        /// <summary>
        /// Comprueba que la fecha de la actividad no puede ser en el futuro.
        /// </summary>
        [TestMethod()]
        public void Constructor_ConFechaFutura_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Actividad(1, "Test", 10.0, 100, TimeSpan.FromHours(1), DateTime.Now.AddDays(1), TipoActividad.Natacion)
            );
        }

        /// <summary>
        /// Comprueba que la FCMedia no puede estar fuera del rango válido (por debajo).
        /// </summary>
        [TestMethod()]
        public void SetFCMedia_PorDebajoDelRango_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Actividad(1, "Test", 10.0, 100, TimeSpan.FromHours(1), DateTime.Now, TipoActividad.Running, fcMedia: 20)
            );
        }

        /// <summary>
        /// Comprueba que la FCMedia no puede estar fuera del rango válido (por encima).
        /// </summary>
        [TestMethod()]
        public void SetFCMedia_PorEncimaDelRango_LanzaArgumentException()
        {
            var actividad = new Actividad(1, "Test", 10.0, 100, TimeSpan.FromHours(1), DateTime.Now, TipoActividad.Running);
            Assert.ThrowsException<ArgumentException>(() => actividad.FCMedia = 230);
        }

        #endregion

        #region Pruebas de Propiedades Calculadas

        /// <summary>
        /// Verifica que el cálculo del ritmo en min/km es correcto.
        /// </summary>
        [TestMethod()]
        public void RitmoMinPorKm_CalculoCorrecto()
        {
            // 10 km en 60 minutos debería ser un ritmo de 6.0 min/km
            var actividad = new Actividad(1, "Carrera", 10, 200, TimeSpan.FromMinutes(60), DateTime.Now, TipoActividad.Running);
            Assert.AreEqual(6.0, actividad.RitmoMinPorKm);
        }

        /// <summary>
        /// Verifica que el cálculo de la velocidad media en km/h es correcto.
        /// </summary>
        [TestMethod()]
        public void VelocidadMediaKmh_CalculoCorrecto()
        {
            // 25 km en 1 hora debería ser una velocidad de 25.0 km/h
            var actividad = new Actividad(1, "Ciclismo", 25, 400, TimeSpan.FromHours(1), DateTime.Now, TipoActividad.Ciclismo);
            Assert.AreEqual(25.0, actividad.VelocidadMediaKmh);
        }

        /// <summary>
        /// Comprueba que las propiedades calculadas devuelven 0 si los Kms son 0 para evitar divisiones por cero.
        /// </summary>
        [TestMethod()]
        public void PropiedadesCalculadas_ConDistanciaCero_DevuelvenCero()
        {
            var actividad = new Actividad(1, "Caminata", 0, 50, TimeSpan.FromMinutes(30), DateTime.Now, TipoActividad.Caminata);
            Assert.AreEqual(0.0, actividad.RitmoMinPorKm);
            Assert.AreEqual(0.0, actividad.VelocidadMediaKmh);
        }
        #endregion

        #region Pruebas de Actualización

        /*
        /// <summary>
        /// Verifica que el método ActualizarMetricas actualiza correctamente los valores de una actividad.
        /// </summary>
        [TestMethod()]
        public void ActualizarMetricas_ConValoresValidos_ActualizaPropiedades()
        {
            var actividad = new Actividad(1, "Original", 10, 100, TimeSpan.FromHours(1), DateTime.Now.AddDays(-1), TipoActividad.Running);

            string nuevoTitulo = "Actualizado";
            double nuevosKms = 12.5;
            int nuevoDesnivel = 150;
            TimeSpan nuevaDuracion = TimeSpan.FromMinutes(70);
            TipoActividad nuevoTipo = TipoActividad.Caminata;
            int? nuevaFc = 145;

            actividad.ActualizarMetricas(nuevoTitulo, nuevosKms, nuevoDesnivel, nuevaDuracion, nuevoTipo, "Nueva Desc", nuevaFc);

            Assert.AreEqual(nuevoTitulo, actividad.Titulo);
            Assert.AreEqual(nuevosKms, actividad.Kms);
            Assert.AreEqual(nuevoDesnivel, actividad.MetrosDesnivel);
            Assert.AreEqual(nuevaDuracion, actividad.Duracion);
            Assert.AreEqual(nuevoTipo, actividad.Tipo);
            Assert.AreEqual(nuevaFc, actividad.FCMedia);
        }
        */

        #endregion

        #region Pruebas de Métodos Públicos

        /// <summary>
        /// Verifica que el método ToString genera el formato de cadena esperado.
        /// </summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Actividad actividad = new Actividad(1, "paseo mañanero", 24, 200, new TimeSpan(1, 0, 0), DateTime.Now, TipoActividad.Running, "Salida de running por el parque", 150);
            StringAssert.Contains(actividad.ToString(), "24,00 km");
            StringAssert.Contains(actividad.ToString(), "150 bpm");
        }

        #endregion
    }
}

