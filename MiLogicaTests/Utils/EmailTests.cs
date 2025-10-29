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
    public class EmailTests
    {
        [TestMethod()]
        public void ValidarEmailTest()
        {
            Assert.IsFalse(Email.ValidarEmail("oscargmail.com")); // No tiene '@'
            Assert.IsFalse(Email.ValidarEmail("oscar@gmailcom")); // No tiene '.'
            Assert.IsFalse(Email.ValidarEmail("oscar@.com")); // No tiene texto antes del '.'
            Assert.IsFalse(Email.ValidarEmail("oscar@gmail.")); // No tiene texto después del '.'
            Assert.IsTrue(Email.ValidarEmail("gepeto@gmail.com"));
            Assert.IsFalse(Email.ValidarEmail("oscar@gmail..com")); // Tiene dos puntos seguidos))
        }

        // --- NUEVO TEST BASADO EN DATOS ---

        /// <summary>
        /// Prueba el método ValidarEmail con un conjunto de datos (válidos e inválidos).
        /// </summary>
        /// <param name="email">El email a probar.</param>
        /// <param name="esperado">El resultado booleano esperado (true si es válido, false si no).</param>
        [DataTestMethod]
        [DataRow("gepeto@gmail.com", true)]      // Caso válido
        [DataRow("test.valido@dominio.co.uk", true)] // Caso válido complejo
        [DataRow("oscargmail.com", false)]       // Inválido (sin @)
        [DataRow("oscar@gmailcom", false)]       // Inválido (sin . en dominio)
        [DataRow("oscar@.com", false)]           // Inválido (dominio vacío)
        [DataRow("oscar@gmail.", false)]         // Inválido (TLD vacío)
        [DataRow("oscar@gmail..com", false)]     // Inválido (puntos seguidos)
        [DataRow("", false)]                    // Inválido (vacío)
        [DataRow(null, false)]                   // Inválido (nulo)
        public void ValidarEmail_DataDriven(string email, bool esperado)
        {
            // Arrange
            // Los datos 'email' y 'esperado' vienen automáticamente de los [DataRow]

            // Act
            bool resultado = Email.ValidarEmail(email);

            // Assert
            Assert.AreEqual(esperado, resultado, $"El email: '{email ?? "null"}' no dio el resultado esperado.");
        }
    }
}