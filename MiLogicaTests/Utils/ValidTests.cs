using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiLogica.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define el espacio de nombres de la clase de pruebas.
namespace MiLogica.Utils.Tests
{
    // Marca la clase como una clase de prueba (Test Class) de MSTest.
    [TestClass()]
    public class ValidTests
    {
        /// <summary>
        /// Prueba la validación del Número de Identificación Fiscal (NIF) español.
        /// Se verifican casos límite y de éxito de la regla de validación.
        /// </summary>
        [TestMethod()]
        public void NIFTest()
        {
            // Caso 1: Cadena vacía (se espera que sea inválida).
            Assert.IsFalse(Valid.NIF(""));
            // Caso 2: NIF que solo contiene números (falta la letra de control, se espera fallo).
            Assert.IsFalse(Valid.NIF("12345678"));
            // Caso 3: NIF con un formato de letra incorrecto o dígito de control erróneo (se espera fallo).
            Assert.IsFalse(Valid.NIF("12345678X"));
            // Caso 4: NIF válido y correcto (se espera que pase la validación).
            Assert.IsTrue(Valid.NIF("71364350L"));
        }

        /// <summary>
        /// Prueba la validación del International Bank Account Number (IBAN).
        /// </summary>
        [TestMethod()]
        public void IBANTest()
        {
            // Caso 1: IBAN con formato o dígito de control incorrecto (se espera que falle).
            Assert.IsFalse(Valid.IBAN("ES9121000418456172314546"));
        }

        /// <summary>
        /// Prueba la validación de un nombre, típicamente para asegurar que solo contiene caracteres alfabéticos
        /// y no está vacío.
        /// </summary>
        [TestMethod()]
        public void NombreTest()
        {
            // Caso 1: Nombre válido, solo letras (se espera éxito).
            Assert.IsTrue(Valid.Nombre("Oscar"));
            // Caso 2: Nombre con dígitos (se espera fallo, ya que los nombres no deben contener números).
            Assert.IsFalse(Valid.Nombre("Oscar4"));
            // Caso 3: Cadena vacía (se espera fallo).
            Assert.IsFalse(Valid.Nombre(""));
            // Caso 4: Cadena con solo espacios en blanco (se espera fallo).
            Assert.IsFalse(Valid.Nombre(" "));
        }
    }
}
