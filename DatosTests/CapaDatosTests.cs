using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiLogica.ModeloDatos;

namespace Datos.Tests
{
    [TestClass()]
    public class CapaDatosTest
    {
        [TestMethod()]
        public void CapaDatosTestConstructor()
        {
            CapaDatos capa = new CapaDatos();
            
            Assert.IsNotNull(capa);
            Console.WriteLine("Número de usuarios iniciales: " + capa.NumUsuarios());
            Assert.IsNotNull(capa.LeeUsuario("oscar@gmail.com"));

        }


        [TestMethod()]
        public void GuardaUsuarioTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ActualizaUsuarioTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LeeUsuarioTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LeeUsuarioPorIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidaUsuarioTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NumUsuariosTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NumUsuariosActivosTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GuardaActividadTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ActualizaActividadTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EliminaActividadTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LeeActividadTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ObtenerActividadesUsuarioTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NumActividadesTest()
        {
            Assert.Fail();
        }
    }
}