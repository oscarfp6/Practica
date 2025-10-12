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


        [TestMethod()]
        public void ActualizarMetricasTest()
        {
            Actividad actividad = new Actividad(1, "paseo mañanero", 24, 200, new TimeSpan(1, 0, 0), DateTime.Now, TipoActividad.Running, "Salida de running por el parque", 150);
            Assert.IsTrue(actividad.ActualizarMetricas("paseo", 24, 200, new TimeSpan(1, 0, 0), TipoActividad.Running, "Salida de running por el parque",150));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Actividad actividad = new Actividad(1, "paseo mañanero",24 ,200, new TimeSpan(1, 0, 0), DateTime.Now, TipoActividad.Running, "Salida de running por el parque", 150);
            Console.WriteLine("Probamos el ToString de Actividad\n");
            Console.WriteLine(actividad.ToString());
        }
    }
}