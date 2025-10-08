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
        public void ActividadTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ActualizarMetricasTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Actividad actividad = new Actividad(1, 23, 200, new TimeSpan(1, 0, 0), DateTime.Now, TipoActividad.Running, "Salida de running por el parque", 150);
            Console.WriteLine("Probamos el ToString de Actividad\n");
            Console.WriteLine(actividad.ToString());
        }
    }
}