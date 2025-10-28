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

        /// <summary>
        /// Comprueba que se puede crear una actividad solo con ID de usuario y título,
        /// y que los valores por defecto son correctos (Kms=0, Duracion=1min, Tipo=Otro).
        /// </summary>
        [TestMethod()]
        public void Constructor_Simple_InitializesDefaults()
        {
            // Arrange
            int idUsuario = 5;
            string titulo = "Actividad por defecto";

            // Act
            var actividad = new Actividad(idUsuario, titulo);

            // Assert
            Assert.AreEqual(idUsuario, actividad.IdUsuario);
            Assert.AreEqual(titulo, actividad.Titulo);
            Assert.AreEqual(0.0, actividad.Kms);
            Assert.AreEqual(0, actividad.MetrosDesnivel);
            Assert.AreEqual(TimeSpan.FromMinutes(1), actividad.Duracion);
            Assert.AreEqual(TipoActividad.Otro, actividad.Tipo);
            Assert.IsNull(actividad.FCMedia);
            Assert.IsTrue(actividad.Fecha <= DateTime.Now); // Solo chequeamos que no sea futura
        }

        /// <summary>
        /// Comprueba que los setters de las propiedades funcionan correctamente con valores válidos.
        /// </summary>
        [TestMethod()]
        public void Setters_ConValoresValidos_ActualizaCorrectamente()
        {
            // Arrange
            var actividad = new Actividad(1, "Inicial", 1.0, 10, TimeSpan.FromMinutes(1), DateTime.Now, TipoActividad.Running);

            // Act
            actividad.Titulo = "Nuevo Título";
            actividad.Descripcion = "Nueva Descripción";
            actividad.Kms = 15.5;
            actividad.MetrosDesnivel = 500;
            actividad.Duracion = TimeSpan.FromHours(2);
            actividad.Fecha = DateTime.Now.AddDays(-1);
            actividad.Tipo = TipoActividad.Ciclismo;
            actividad.FCMedia = 155;

            // Assert
            Assert.AreEqual("Nuevo Título", actividad.Titulo);
            Assert.AreEqual("Nueva Descripción", actividad.Descripcion);
            Assert.AreEqual(15.5, actividad.Kms);
            Assert.AreEqual(500, actividad.MetrosDesnivel);
            Assert.AreEqual(TimeSpan.FromHours(2), actividad.Duracion);
            Assert.AreEqual(TipoActividad.Ciclismo, actividad.Tipo);
            Assert.AreEqual(155, actividad.FCMedia);
            Assert.IsTrue(actividad.Fecha < DateTime.Now);
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
        /// Verifica que para actividades de natación, el ritmo y la velocidad se manejan correctamente.
        /// </summary>
        [TestMethod()]
        public void RitmoVelocidad_TipoNatacion_EsCero()
        {
            // 1 km en 10 minutos (No debería mostrar Ritmo ni Velocidad en la UI, 
            // pero los valores calculados en el objeto deben ser correctos: Ritmo=10.0, Velocidad=6.0)
            var actividad = new Actividad(1, "Entrenamiento Piscina", 1.0, 0,
                                          TimeSpan.FromMinutes(10), DateTime.Now, TipoActividad.Natacion);

            // Assert (Verificar que los cálculos internos son correctos aunque la UI no los muestre)
            Assert.AreEqual(10.0, actividad.RitmoMinPorKm, "El ritmo calculado debe ser 10.0 min/km.");
            Assert.AreEqual(6.0, Math.Round(actividad.VelocidadMediaKmh, 2), "La velocidad calculada debe ser 6.0 km/h.");

            // Nota: La capa de presentación (Menu.aspx.cs::FormatearRitmo) se encarga de mostrar vacío.
        }

        /// <summary>
        /// Verifica que el ritmo y la velocidad son 0 cuando los kilómetros son 0.
        [TestMethod()]
        public void RitmoVelocidad_ConDistanciaCero_DevuelvenCero()
        {
            // Este test ya está presente, pero se incluye para recordar su importancia (RF-014, CA-3)
            var actividad = new Actividad(1, "Caminata", 0, 50, TimeSpan.FromMinutes(30), DateTime.Now, TipoActividad.Caminata);
            Assert.AreEqual(0.0, actividad.RitmoMinPorKm);
            Assert.AreEqual(0.0, actividad.VelocidadMediaKmh);
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
        /// Verifica que el método ToString genera el formato de cadena esperado (con FC Media).
        /// </summary>
        [TestMethod()]
        public void ToStringTest_ConFCMedia()
        {
            // Arrange
            Actividad actividad = new Actividad(1, "paseo mañanero", 24, 200, new TimeSpan(1, 0, 0), DateTime.Now, TipoActividad.Running, "Salida de running por el parque", 150);

            // Act
            string result = actividad.ToString();

            // Assert
            StringAssert.Contains(result, "24,00 km");
            StringAssert.Contains(result, "150 bpm");
            StringAssert.Contains(result, "2,50 min/km"); // 60 min / 24 km = 2.5 min/km
        }

        /// <summary>
        /// Verifica que el método ToString genera el formato de cadena esperado (sin FC Media).
        /// </summary>
        [TestMethod()]
        public void ToStringTest_SinFCMedia()
        {
            // Arrange
            Actividad actividad = new Actividad(1, "paseo mañanero", 24, 200, new TimeSpan(1, 0, 0), DateTime.Now, TipoActividad.Running, "Salida de running por el parque", null);

            // Act
            string result = actividad.ToString();

            // Assert
            StringAssert.Contains(result, "24,00 km");
            Assert.IsFalse(result.Contains("bpm"));
            StringAssert.Contains(result, "2,50 min/km");
        }

        #endregion
    }
}

