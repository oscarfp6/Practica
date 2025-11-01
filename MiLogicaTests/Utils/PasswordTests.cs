
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiLogica.Utils;
using System.Collections.Generic;
using System.IO;                 
using Newtonsoft.Json;           
using System;                   

namespace MiLogica.Utils.Tests
{
    [TestClass()]
    public class PasswordTests
    {
        /// <summary>
        /// Prueba individual que verifica el comportamiento básico de ValidarPassword 
        /// con casos de éxito y fallo manuales.
        /// </summary>
        [TestMethod()]
        public void ValidarPasswordTest_Individual() // Renombrado para claridad
        {
            // Assert: Verifica un caso de éxito esperado (contraseña válida).
            Assert.IsTrue(Password.ValidarPassword("@Contraseñavalida123"));
            // Assert: Verifica un caso de fallo esperado (ej. contraseña muy corta).
            Assert.IsFalse(Password.ValidarPassword("short1A@"));
            // ... más casos individuales si fueran necesarios ...
        }

        // --- NUEVO TEST BASADO EN JSON ---

        /// <summary>
        /// Clase auxiliar interna para mapear (deserializar) cada objeto del JSON.
        /// Sus propiedades reflejan la estructura de datos esperada en el archivo casosPassword.json.
        /// </summary>
        public class PasswordTestCase
        {
            public string Caso { get; set; } // Nombre del caso (para debug).
            public string Password { get; set; } // La contraseña a probar.
            public bool Esperado { get; set; } // El resultado esperado (True/False).
        }

        /// <summary>
        /// Test Method que implementa Data-Driven Testing: lee todos los casos de prueba 
        /// desde el archivo casosPassword.json y ejecuta ValidarPassword para cada uno.
        /// </summary>
        [TestMethod]
        public void ValidarPassword_DataDriven_JSON()
        {
            // 1. Definir la ruta relativa del archivo JSON.
            string jsonPath = "casosPassword.json";

            // 2. Verificación crucial: Asegura que el archivo de datos existe antes de intentar leerlo.
            Assert.IsTrue(File.Exists(jsonPath),
                $"El archivo de datos '{jsonPath}' no se encontró en el directorio de salida. " +
                "Revisa la propiedad 'Copy to Output Directory' del archivo JSON en Visual Studio.");

            // 3. Leer todo el contenido del archivo JSON como una única cadena de texto.
            string jsonContent = File.ReadAllText(jsonPath);

            // 4. Deserializar la cadena JSON a una Lista de objetos PasswordTestCase, usando Newtonsoft.Json.
            List<PasswordTestCase> testCases = null;
            try
            {
                testCases = JsonConvert.DeserializeObject<List<PasswordTestCase>>(jsonContent);
            }
            catch (JsonReaderException ex)
            {
                // Si la estructura o sintaxis del JSON es incorrecta, la prueba falla inmediatamente con un error descriptivo.
                Assert.Fail($"Error al leer el JSON '{jsonPath}': {ex.Message}. Revisa la sintaxis del archivo.");
            }


            // 5. Validaciones post-deserialización: Se asegura que la lista no es nula y contiene elementos.
            Assert.IsNotNull(testCases, "La deserialización del JSON devolvió null. Revisa el contenido del archivo.");
            Assert.IsTrue(testCases.Count > 0, $"El archivo JSON '{jsonPath}' parece estar vacío o no contiene casos válidos.");

            // 6. Iterar sobre cada caso de prueba deserializado y ejecutar la validación.
            Console.WriteLine($"--- Iniciando pruebas de Password desde JSON ({testCases.Count} casos) ---");
            foreach (var testCase in testCases)
            {
                // Act: Llamar al método bajo prueba (ValidarPassword).
                // Manejo de caso donde la 'Password' del JSON pudiera ser null (lo convierte a string.Empty).
                bool resultadoActual = Password.ValidarPassword(testCase.Password ?? string.Empty);

                // Assert: Compara el resultado REAL (resultadoActual) con el resultado ESPERADO (testCase.Esperado).
                // Si falla, el mensaje de error incluye el nombre del 'Caso' y la contraseña para facilitar la depuración.
                Assert.AreEqual(testCase.Esperado, resultadoActual,
                    $"Caso '{testCase.Caso}' falló. Password: '{testCase.Password ?? "null"}'. Se esperaba: {testCase.Esperado}, se obtuvo: {resultadoActual}.");

                // Opcional: Imprimir en la consola el resultado de cada caso exitoso.
                Console.WriteLine($"  [OK] Caso '{testCase.Caso}' (Password: '{testCase.Password ?? "null"}') -> Esperado: {testCase.Esperado}, Obtenido: {resultadoActual}");
            }
            Console.WriteLine($"--- Pruebas de Password desde JSON completadas ---");
        }
    }
}
