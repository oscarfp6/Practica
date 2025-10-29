using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiLogica.Utils;
using System.Collections.Generic; // Para List<>
using System.IO;                 // Para File
using Newtonsoft.Json;           // ¡El paquete que instalamos!
using System;                    // Para Console.WriteLine

namespace MiLogica.Utils.Tests
{
    [TestClass()]
    public class PasswordTests
    {
        // Tu test existente (puedes mantenerlo o eliminarlo si el nuevo lo cubre)
        [TestMethod()]
        public void ValidarPasswordTest_Individual() // Renombrado para claridad
        {
            Assert.IsTrue(Password.ValidarPassword("@Contraseñavalida123"));
            Assert.IsFalse(Password.ValidarPassword("short1A@"));
            // ... etc ...
        }

        // --- NUEVO TEST BASADO EN JSON ---

        /// <summary>
        /// Clase auxiliar interna para mapear (deserializar) cada objeto del JSON.
        /// Los nombres de las propiedades (Caso, Password, Esperado) DEBEN coincidir
        /// exactamente (mayúsculas/minúsculas incluidas) con las claves en el JSON.
        /// </summary>
        public class PasswordTestCase
        {
            public string Caso { get; set; }
            public string Password { get; set; }
            public bool Esperado { get; set; }
        }

        /// <summary>
        /// Test Method que lee casos de prueba desde casosPassword.json
        /// y ejecuta la validación para cada uno.
        /// </summary>
        [TestMethod]
        public void ValidarPassword_DataDriven_JSON()
        {
            // 1. Definir la ruta relativa del archivo JSON.
            //    Gracias a "Copy if newer", estará en el directorio de ejecución del test.
            string jsonPath = "casosPassword.json";

            // 2. Verificación crucial: Asegurarnos de que el archivo existe ANTES de intentar leerlo.
            Assert.IsTrue(File.Exists(jsonPath),
                $"El archivo de datos '{jsonPath}' no se encontró en el directorio de salida. " +
                "Revisa la propiedad 'Copy to Output Directory' del archivo JSON en Visual Studio.");

            // 3. Leer todo el contenido del archivo JSON como una cadena de texto.
            string jsonContent = File.ReadAllText(jsonPath);

            // 4. Deserializar la cadena JSON a una Lista de objetos PasswordTestCase.
            //    Newtonsoft.Json se encarga de mapear las claves del JSON a las propiedades de la clase.
            List<PasswordTestCase> testCases = null;
            try
            {
                testCases = JsonConvert.DeserializeObject<List<PasswordTestCase>>(jsonContent);
            }
            catch (JsonReaderException ex)
            {
                Assert.Fail($"Error al leer el JSON '{jsonPath}': {ex.Message}. Revisa la sintaxis del archivo.");
            }


            // 5. Validaciones post-deserialización.
            Assert.IsNotNull(testCases, "La deserialización del JSON devolvió null. Revisa el contenido del archivo.");
            Assert.IsTrue(testCases.Count > 0, $"El archivo JSON '{jsonPath}' parece estar vacío o no contiene casos válidos.");

            // 6. Iterar sobre cada caso de prueba deserializado y ejecutar la validación.
            Console.WriteLine($"--- Iniciando pruebas de Password desde JSON ({testCases.Count} casos) ---");
            foreach (var testCase in testCases)
            {
                // Arrange (implícito): Los datos vienen de testCase

                // Act: Llamar al método que estamos probando.
                // Usamos el operador ?? "" para manejar de forma segura si "Password" fuera null en el JSON,
                // aunque en nuestro caso, una contraseña vacía ("") es un caso válido a probar.
                bool resultadoActual = Password.ValidarPassword(testCase.Password ?? string.Empty);

                // Assert: Comparamos el resultado obtenido con el esperado en el JSON.
                // El mensaje de error ahora incluye el nombre del 'Caso' para fácil depuración.
                Assert.AreEqual(testCase.Esperado, resultadoActual,
                    $"Caso '{testCase.Caso}' falló. Password: '{testCase.Password ?? "null"}'. Se esperaba: {testCase.Esperado}, se obtuvo: {resultadoActual}.");

                // Opcional: Imprimir éxito para cada caso (útil durante el desarrollo)
                Console.WriteLine($"  [OK] Caso '{testCase.Caso}' (Password: '{testCase.Password ?? "null"}') -> Esperado: {testCase.Esperado}, Obtenido: {resultadoActual}");
            }
            Console.WriteLine($"--- Pruebas de Password desde JSON completadas ---");
        }
    }
}