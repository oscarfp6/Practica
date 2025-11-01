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
        /// <summary>
        /// Prueba básica del método ValidarEmail para cubrir casos límite y caminos felices sencillos.
        /// Este es un ejemplo de Pruebas de Partición de Equivalencia.
        /// </summary>
        [TestMethod()]
        public void ValidarEmailTest()
        {
            // Caso de Partición Inválida: Le falta el separador principal '@'.
            Assert.IsFalse(Email.ValidarEmail("oscargmail.com"));

            // Caso de Partición Inválida: Le falta el punto de separación del TLD (Top-Level Domain).
            Assert.IsFalse(Email.ValidarEmail("oscar@gmailcom"));

            // Caso de Partición Inválida: El punto está justo después del '@' (ej. local@.com).
            Assert.IsFalse(Email.ValidarEmail("oscar@.com"));

            // Caso de Partición Inválida: Termina en punto (le falta el TLD).
            Assert.IsFalse(Email.ValidarEmail("oscar@gmail."));

            // Caso de Partición Válida: Formato estándar correcto.
            Assert.IsTrue(Email.ValidarEmail("gepeto@gmail.com"));

            // Caso de Partición Inválida: Puntos consecutivos en el dominio.
            Assert.IsFalse(Email.ValidarEmail("oscar@gmail..com"));
        }

        // --- NUEVO TEST BASADO EN DATOS (Data-Driven Testing) ---

        /// <summary>
        /// Prueba el método ValidarEmail con un conjunto de datos (válidos e inválidos) 
        /// definidos directamente en el código a través de atributos [DataRow].
        /// Esto permite un test de regresión más exhaustivo y fácil de mantener.
        /// </summary>
        /// <param name="email">El email a probar.</param>
        /// <param name="esperado">El resultado booleano esperado (true si es válido, false si no).</param>
        [DataTestMethod]
        [DataRow("gepeto@gmail.com", true)]      // Caso válido (estándar)
        [DataRow("test.valido@dominio.co.uk", true)] // Caso válido (subdominio y TLD complejo)
        [DataRow("oscargmail.com", false)]       // Inválido (sin @)
        [DataRow("oscar@gmailcom", false)]       // Inválido (sin . en dominio)
        [DataRow("oscar@.com", false)]           // Inválido (dominio vacío)
        [DataRow("oscar@gmail.", false)]         // Inválido (TLD vacío)
        [DataRow("oscar@gmail..com", false)]     // Inválido (puntos seguidos)
        [DataRow("", false)]                     // Inválido (vacío)
        [DataRow(null, false)]                   // Inválido (nulo)
        public void ValidarEmail_DataDriven(string email, bool esperado)
        {
            // Arrange
            // Los datos 'email' y 'esperado' vienen automáticamente de los [DataRow]

            // Act
            bool resultado = Email.ValidarEmail(email);

            // Assert
            // Se usa el mensaje de error para identificar rápidamente qué caso falló.
            Assert.AreEqual(esperado, resultado, $"El email: '{email ?? "null"}' no dio el resultado esperado.");
        }
    }
}
