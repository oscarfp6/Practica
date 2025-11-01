// Importaciones necesarias para las pruebas unitarias.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiLogica.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.Utils.Tests
{
    [TestClass()]
    public class EncriptarTests
    {
        /// <summary>
        /// Prueba que el método EncriptarPasswordSHA256 genera el mismo hash para la misma entrada de contraseña.
        /// (Prueba de determinismo para un algoritmo de hashing unidireccional como SHA256).
        /// </summary>
        [TestMethod()]
        public void EncriptarPasswordSHA256Test()
        {
            // Arrange: Se definen dos variables con el mismo valor de entrada.
            string password1 = "hola1234";
            string password2 = "hola1234";

            // Act: Se encripta cada contraseña utilizando el método SHA256.
            string password1Encriptada = Encriptar.EncriptarPasswordSHA256(password1);
            string password2Encriptada = Encriptar.EncriptarPasswordSHA256(password2);

            // Assert: Se verifica que el hash generado para ambas entradas idénticas también es idéntico.
            // Esto confirma el carácter determinista del algoritmo de hashing.
            Assert.AreEqual(password1Encriptada, password2Encriptada);
        }
    }
}
