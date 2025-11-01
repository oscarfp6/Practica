// Importaciones necesarias para las pruebas unitarias y el acceso al modelo de datos.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Define el espacio de nombres para la clase de pruebas.
namespace MiLogica.ModeloDatos.Tests
{
    // Marca la clase como una clase de prueba de MSTest.
    [TestClass()]
    public class UsuarioTests
    {

        /// <summary>
        /// Prueba que el constructor lanza una ArgumentException si se proporciona un email con formato inválido.
        /// (Validación de entrada en el constructor).
        /// </summary>
        [TestMethod()]
        public void Constructor_ConEmailInvalido_LanzaArgumentException()
        {
            // Espera que se lance una ArgumentException al intentar instanciar un Usuario con el email "email-invalido".
            Assert.ThrowsException<ArgumentException>(() =>
                new Usuario(1, "Test", "@PasswordValida123", "User", "email-invalido", false)
            );
        }

        /// <summary>
        /// Prueba que el constructor lanza una ArgumentException si la contraseña proporcionada es insegura o inválida.
        /// (Validación de seguridad en el constructor).
        /// </summary>
        [TestMethod()]
        public void Constructor_ConPasswordInvalida_LanzaArgumentException()
        {
            // Espera que se lance una ArgumentException al intentar instanciar un Usuario con la contraseña "corta".
            Assert.ThrowsException<ArgumentException>(() =>
                new Usuario(1, "Test", "corta", "User", "test@gmail.com", false)
            );
        }


        /// <summary>
        /// Prueba que al intentar asignar un nombre nulo o que consiste solo en espacios en blanco, se lanza una ArgumentException.
        /// (Validación de la propiedad Nombre al establecer el valor).
        /// </summary>
        [TestMethod()]
        public void SetNombre_ConValorNuloOEspacios_LanzaArgumentException()
        {
            // Arrange: Se crea un usuario válido.
            Usuario usuario = new Usuario(1, "Test", "@PasswordValida123", "User", "test@gmail.com", false);
            // Act & Assert: Se verifica que asignar una cadena de espacios lanza una ArgumentException.
            Assert.ThrowsException<ArgumentException>(() => usuario.Nombre = "   ");
        }


        /// <summary>
        /// Prueba que al intentar asignar una cadena vacía a Apellidos, se lanza una ArgumentException.
        /// (Validación de la propiedad Apellidos al establecer el valor).
        /// </summary>
        [TestMethod()]
        public void SetApellidos_ConValorNuloOEspacios_LanzaArgumentException()
        {
            // Arrange: Se crea un usuario válido.
            Usuario usuario = new Usuario(1, "Test", "@PasswordValida123", "User", "test@gmail.com", false);
            // Act & Assert: Se verifica que asignar una cadena vacía lanza una ArgumentException.
            Assert.ThrowsException<ArgumentException>(() => usuario.Apellidos = "");
        }

        /// <summary>
        /// Prueba que al intentar asignar un Nombre que contiene dígitos, se lanza una ArgumentException.
        /// (Regla de negocio: Nombre no debe contener números).
        /// </summary>
        [TestMethod()]
        public void SetNombre_ConDigitos_LanzaArgumentException()
        {
            // Arrange: Se crea un usuario válido.
            Usuario usuario = new Usuario(1, "Test", "@PasswordValida123", "User", "test@gmail.com", false);
            // Act & Assert: Se verifica que asignar "Oscar456" lanza una ArgumentException.
            Assert.ThrowsException<ArgumentException>(() => usuario.Nombre = "Oscar456");
        }

        /// <summary>
        /// Prueba que al intentar asignar Apellidos que contienen dígitos, se lanza una ArgumentException.
        /// (Regla de negocio: Apellidos no debe contener números).
        /// </summary>
        [TestMethod()]
        public void SetApellidos_ConDigitos_LanzaArgumentException()
        {
            // Arrange: Se crea un usuario válido.
            Usuario usuario = new Usuario(1, "Test", "@PasswordValida123", "User", "test@gmail.com", false);
            // Act & Assert: Se verifica que asignar "Martínez5" lanza una ArgumentException.
            Assert.ThrowsException<ArgumentException>(() => usuario.Apellidos = "Martínez5");
        }


        /// <summary>
        /// Prueba el flujo completo de inicio de sesión: éxito inicial y bloqueo por múltiples fallos.
        /// (Prueba de la lógica de login y el mecanismo de bloqueo por fuerza bruta).
        /// </summary>
        [TestMethod()]
        public void PermitirLoginTest()
        {
            // Arrange: Crea un nuevo usuario.
            Usuario Jose = new Usuario(1, "usuario_jose", "@Contraseñavalida123", "Pérez", "jose@gmail.com", false);

            // 1. Prueba de login exitoso.
            Assert.IsTrue(Jose.PermitirLogin("@Contraseñavalida123"));
            Console.WriteLine($"Estado tras login exitoso: {Jose.Estado}");
            // Verifica que el estado cambie a Activo tras el éxito.
            Assert.AreEqual(EstadoUsuario.Activo, Jose.Estado);

            // 2. Simula dos intentos fallidos.
            Jose.PermitirLogin("wrongpass1");
            Jose.PermitirLogin("wrongpass2");
            // 3. Simula el tercer intento fallido (debe resultar en bloqueo).
            Assert.IsFalse(Jose.PermitirLogin("wrongpass3"));
            Console.WriteLine($"Estado tras 3 fallos: {Jose.Estado}");
            // Verifica que el estado cambie a Bloqueado.
            Assert.AreEqual(EstadoUsuario.Bloqueado, Jose.Estado);

            // 4. Prueba que el login falla incluso con la contraseña correcta si está Bloqueado.
            Console.WriteLine("Esperando 5 segundos para desbloquear..., introducimos contraseña correcta");
            Assert.IsFalse(Jose.PermitirLogin("@Contraseñavalida123"));
        }

        /// <summary>
        /// Prueba las transiciones de estado que involucran Inactivo y Bloqueado, incluyendo reactivación.
        /// (Prueba de la lógica de inactividad y bloqueo estricto).
        /// </summary>
        [TestMethod()]
        public void PermitirLoginUsuarioInactivoBloqueoEstrictoTest()
        {
            // Arrange: Usuario David.
            Usuario David = new Usuario(8, "David", "@Contraseñavalida123", "Martín", "david@gmail.com", false);

            // Simula inactividad.
            David.LastLogin = DateTime.Now.AddDays(-200);
            David.VerificarInactividad();
            Console.WriteLine($"Estado Inicial: {David.Estado}");
            // Verifica que el estado es Inactivo.
            Assert.AreEqual(EstadoUsuario.Inactivo, David.Estado);

            // Arrange: Usuario Inactivo que se reactiva.
            Usuario InactivoReactivado = new Usuario(9, "Iñigo", "@Contraseñavalida123", "Lopez", "inigo@gmail.com", false);
            InactivoReactivado.LastLogin = DateTime.Now.AddDays(-200);
            InactivoReactivado.VerificarInactividad();
            // Act: Login correcto debe reactivar.
            Assert.IsTrue(InactivoReactivado.PermitirLogin("@Contraseñavalida123"));
            // Assert: Verifica la reactivación.
            Assert.AreEqual(EstadoUsuario.Activo, InactivoReactivado.Estado, "El login correcto debe reactivar.");

            // Act: Un solo fallo en David (Inactivo).
            Assert.IsFalse(David.PermitirLogin("wrongpass1"));
            Console.WriteLine($"Estado tras 1 fallo estando Inactivo: {David.Estado}");
            // Assert: Verifica que un único fallo bloquea inmediatamente si el usuario estaba Inactivo.
            Assert.AreEqual(EstadoUsuario.Bloqueado, David.Estado, "Un solo fallo en Inactivo debe bloquear inmediatamente");
        }

        /// <summary>
        /// Prueba específica para la reactivación exitosa de un usuario en estado Inactivo al iniciar sesión.
        /// </summary>
        [TestMethod()]
        public void PermitirLogin_UsuarioInactivo_SeReactivaExitosamente()
        {
            // Arrange
            Usuario Inactivo = new Usuario(9, "Iñigo", "@Contraseñavalida123", "Lopez", "inigo@gmail.com", false);
            // Simular inactividad de 6 meses
            Inactivo.LastLogin = DateTime.Now.AddDays(-200);
            Inactivo.VerificarInactividad();
            Assert.AreEqual(EstadoUsuario.Inactivo, Inactivo.Estado);

            // Act: Intento de login con contraseña correcta
            bool resultado = Inactivo.PermitirLogin("@Contraseñavalida123");

            // Assert (Debe ser Activo y conceder acceso)
            Assert.IsTrue(resultado, "El login con pass correcta debería reactivar la cuenta Inactiva.");
            Assert.AreEqual(EstadoUsuario.Activo, Inactivo.Estado, "El estado debe cambiar a Activo tras el login exitoso.");
        }

        /// <summary>
        /// Prueba la función de comprobación de contraseña sin modificar el estado del usuario.
        /// (Prueba de utilidad de comparación de contraseña).
        /// </summary>
        [TestMethod()]
        public void ComprobarPassWordTest()
        {
            // Arrange
            Usuario juan = new Usuario(2, "juan", "@Contraseñavalida123", "fernandez", "juan@gmail.com", false);
            // Assert: Contraseña correcta.
            Assert.IsTrue(juan.ComprobarPassWord("@Contraseñavalida123"));
            // Assert: Contraseña incorrecta.
            Assert.IsFalse(juan.ComprobarPassWord("fake"));
        }

        /// <summary>
        /// Prueba el proceso de desbloqueo manual (o automático al expirar el tiempo de bloqueo) con credenciales correctas.
        /// (Prueba del mecanismo de desbloqueo).
        /// </summary>
        [TestMethod()]
        public void DesbloquearUsuarioTest()
        {
            // Arrange: Bloquear al usuario.
            Usuario Luis = new Usuario(4, "Luis", "@Contraseñavalida123", "Martinez", "luis@gmail.com", true);
            Luis.PermitirLogin("wrongpass");
            Luis.PermitirLogin("wrongpass");
            Luis.PermitirLogin("wrongpass");
            Console.WriteLine(Luis.Estado);
            Assert.IsFalse(Luis.PermitirLogin("@Contraseñavalida123")); // Está bloqueado.

            // Simula que el tiempo de bloqueo ha expirado.
            Luis.BloqueadoHasta = DateTime.Now.AddMinutes(-1); // Establece BloqueadoHasta a 1 minuto en el pasado.

            // Act & Assert: Intenta desbloquear con credenciales correctas.
            Assert.IsTrue(Luis.DesbloquearUsuario("luis@gmail.com", "@Contraseñavalida123"));
            Console.WriteLine(Luis.Estado);
            // Verifica que tras el desbloqueo, el login es permitido.
            Assert.IsTrue(Luis.PermitirLogin("@Contraseñavalida123"));
        }

        /// <summary>
        /// Prueba que el intento de desbloqueo de un usuario que ya está Activo falla.
        /// (Prueba de estado límite).
        /// </summary>
        [TestMethod()]
        public void DesbloquarUsuarioActivoTest()
        {
            // Arrange: Usuario Activo.
            Usuario Ana = new Usuario(2, "Ana", "@Contraseñavalida123", "García", "ana@gmail.com", false);
            // Assert: El intento de desbloqueo debe fallar.
            Assert.IsFalse(Ana.DesbloquearUsuario("ana@gmail.com", "@@Contraseñavalida123"));
        }

        /// <summary>
        /// Prueba que DesbloquearUsuario falla si el tiempo de cooldown (BloqueadoHasta) aún está activo.
        /// </summary>
        [TestMethod()]
        public void DesbloquearUsuarioTest_FalloPorCooldownActivo()
        {
            // Arrange: Bloquear para activar el cooldown de 2 minutos
            Usuario usuario = new Usuario(10, "Test", "@Contraseñavalida123", "User", "test2@gmail.com", false);
            usuario.PermitirLogin("wrong1");
            usuario.PermitirLogin("wrong2");
            usuario.PermitirLogin("wrong3");
            Assert.AreEqual(EstadoUsuario.Bloqueado, usuario.Estado);

            // Act: Intentar desbloquear INMEDIATAMENTE (mientras el cooldown está vigente)
            bool resultado = usuario.DesbloquearUsuario("test2@gmail.com", "@Contraseñavalida123");

            // Assert
            Assert.IsFalse(resultado, "El desbloqueo debe fallar porque el cooldown está activo.");
            Assert.AreEqual(EstadoUsuario.Bloqueado, usuario.Estado, "El estado debe permanecer Bloqueado.");
        }

        /// <summary>
        /// Prueba que un intento de desbloqueo fallido por contraseña incorrecta no altera el estado de bloqueo ni el tiempo de cooldown.
        /// </summary>
        // Nota: Este método no tiene [TestMethod()] en el código original, pero es tratado como una prueba de funcionalidad.
        public void DesbloquearUsuarioTest_FalloPorPasswordIncorrectaReiniciaBloqueo()
        {
            // Arrange: Bloquear al usuario y expirar el cooldown
            Usuario usuario = new Usuario(11, "Cooldown", "@CoolPass123456@", "Test", "cooldown@gmail.com", false);
            usuario.PermitirLogin("wrong1");
            usuario.PermitirLogin("wrong2");
            usuario.PermitirLogin("wrong3");
            usuario.BloqueadoHasta = DateTime.Now.AddMinutes(-1); // Expirar cooldown

            // Act: Intento de desbloqueo con contraseña INCORRECTA
            bool resultado = usuario.DesbloquearUsuario("cooldown@gmail.com", "incorrecta");

            // Assert
            Assert.IsFalse(resultado, "El desbloqueo debe fallar por contraseña incorrecta.");
            Assert.AreEqual(EstadoUsuario.Bloqueado, usuario.Estado, "El estado debe permanecer Bloqueado.");
            // La lógica en DesbloquearUsuario no reinicia el BloqueadoHasta, pero sí lo hace la lógica en Login.aspx.cs.
            // Para testear la pura lógica del modelo, DesbloquearUsuario simplemente devuelve false, sin cambiar nada si falla.
            Assert.IsNull(usuario.BloqueadoHasta, "El modelo no debe reasignar BloqueadoHasta si el desbloqueo falla por password.");
        }

        /// <summary>
        /// Prueba la transición de estado de Activo a Inactivo si el último login excede un umbral de tiempo.
        /// (Prueba de la lógica de inactividad).
        /// </summary>
        [TestMethod()]
        public void VerificarInactividadTest()
        {
            // Arrange: Usuario con login antiguo.
            Usuario Lucia = new Usuario(6, "Lucia", "@Contraseñavalida123", "Ruiz", "lucia@gmail.com", false);
            Lucia.LastLogin = DateTime.Now.AddDays(-200);
            Console.WriteLine(Lucia.Estado);
            Assert.IsTrue(Lucia.Estado == EstadoUsuario.Activo);

            // Act: Se verifica la inactividad.
            Lucia.VerificarInactividad();

            // Assert: El estado debe ser Inactivo.
            Assert.IsTrue(Lucia.Estado == EstadoUsuario.Inactivo);
        }

        /// <summary>
        /// Prueba el cambio de contraseña estándar con valores válidos.
        /// </summary>
        [TestMethod()]
        public void CambiarPasswordTest()
        {
            // Arrange
            Usuario oscar = new Usuario(3, "oscar", "@Contraseñavalida123", "Lopez", "oscar@gmail.com", false);
            // Act & Assert: Intenta cambiar la contraseña.
            Assert.IsTrue(oscar.CambiarPassword("@Contraseñavalida123", "@contraseñaSegura123"));
        }

        /// <summary>
        /// Prueba el cambio de contraseña en un usuario Activo con escenarios de éxito y fallo (contraseña actual incorrecta y nueva insegura).
        /// </summary>
        [TestMethod()]
        public void CambiarPasswordUsuarioActivoTest()
        {
            // Arrange
            Usuario Maria = new Usuario(2, "Maria", "@Contraseñavalida123", "García", "maria@gmail.com", true);
            Console.WriteLine(Maria.Estado);

            // 1. Prueba de fallo por contraseña actual incorrecta.
            Assert.IsFalse(Maria.CambiarPassword("wrongPass", "newPass"));

            // 2. Prueba de éxito.
            Assert.IsTrue(Maria.CambiarPassword("@Contraseñavalida123", "@contraseñaSegura123"));

            // 3. Verifica que la nueva contraseña funciona.
            Assert.IsTrue(Maria.ComprobarPassWord("@contraseñaSegura123"));

            // 4. Prueba de fallo por nueva contraseña insegura.
            Console.WriteLine("Probamos a cambiar la contraseña a una inválida/insegura");
            if (Maria.CambiarPassword("newPass", "short"))
                Console.WriteLine("La contraseña se ha cambiado a una insegura, hay un error.");
            else
                Console.WriteLine("La contraseña no cumple los requisitos de seguridad, no se ha cambiado.");

            // Verifica que la nueva contraseña insegura NO funciona.
            Assert.IsFalse(Maria.ComprobarPassWord("short"));

        }

        /// <summary>
        /// Prueba que un usuario en estado Bloqueado no puede cambiar su contraseña.
        /// (Prueba de restricción de estado).
        /// </summary>
        [TestMethod()]
        public void CambiarPasswordUsuarioBloqueadooTest()
        {
            // Arrange: Bloquear al usuario.
            Usuario Ana = new Usuario(2, "Ana", "@Contraseñavalida123", "García", "ana@gmail.com", false);
            Ana.PermitirLogin("wrongpass");
            Ana.PermitirLogin("wrongpass");
            Ana.PermitirLogin("wrongpass");
            Console.WriteLine(Ana.Estado);
            // Assert: El cambio de contraseña debe fallar.
            Assert.IsFalse(Ana.CambiarPassword("@Contraseñavalida123", "newPass"));
        }

        /// <summary>
        /// Comprueba que un administrador puede establecer una nueva contraseña de forma segura, restableciendo el estado del usuario.
        /// (Prueba de funcionalidad administrativa de restablecimiento de contraseña).
        /// </summary>
        [TestMethod()]
        public void AdminEstablecerPasswordTest_Exito()
        {
            // Arrange: Un usuario bloqueado o en cualquier estado
            Usuario u = new Usuario(12, "TestAdmin", "@OldPass123456@", "User", "adminchange@gmail.com", false);
            u.Estado = EstadoUsuario.Bloqueado;
            u.BloqueadoHasta = DateTime.Now.AddMinutes(5); // Cooldown activo

            string nuevaPassSegura = "@NuevaPassSegura789";

            // Act: El administrador establece la nueva contraseña.
            bool resultado = u.AdminEstablecerPassword(nuevaPassSegura);

            // Assert
            Assert.IsTrue(resultado, "El cambio de contraseña por admin debe ser exitoso.");
            Assert.IsTrue(u.ComprobarPassWord(nuevaPassSegura), "La nueva contraseña debe funcionar.");
            Assert.AreEqual(EstadoUsuario.Activo, u.Estado, "El estado debe restablecerse a Activo.");
            Assert.IsNull(u.BloqueadoHasta, "El cooldown debe eliminarse.");
        }

        /// <summary>
        /// Comprueba que un administrador NO puede establecer una contraseña insegura, manteniendo las reglas de seguridad.
        /// </summary>
        [TestMethod()]
        public void AdminEstablecerPasswordTest_FalloInsegura()
        {
            // Arrange
            Usuario u = new Usuario(13, "TestAdmin", "@OldPass123456@", "User", "adminchange2@gmail.com", false);
            string nuevaPassInsegura = "short";
            string passOriginal = "@OldPass123456@";

            // Act: Intento de cambio de contraseña con valor inseguro.
            bool resultado = u.AdminEstablecerPassword(nuevaPassInsegura);

            // Assert
            Assert.IsFalse(resultado, "El cambio de contraseña por admin debe fallar si es insegura.");
            Assert.IsTrue(u.ComprobarPassWord(passOriginal), "La contraseña original debe conservarse.");
            // El estado no cambia porque el fallo ocurre durante la validación de la nueva pass.
            Assert.AreEqual(EstadoUsuario.Activo, u.Estado, "El estado no cambia al fallar la validación de la nueva pass.");
        }

        /// <summary>
        /// Prueba la función de Actualización de Perfil, verificando la asignación de valores válidos, nulos y la validación de límites.
        /// (Prueba de lógica de negocio para la actualización de datos).
        /// </summary>
        [TestMethod()]
        public void ActualizarPerfilTest()
        {
            // Arrange: Se crea un usuario (usa el constructor por defecto).
            Usuario Pedro = new Usuario();

            // 1. Asignación inicial de Nombre/Apellidos.
            Pedro.ActualizarPerfil("Pedro", "Sánchez", null, null);
            Assert.AreEqual("Pedro", Pedro.Nombre);
            Assert.AreEqual("Sánchez", Pedro.Apellidos);

            // 2. Probar asignación de valores válidos para Edad y Peso.
            Pedro.ActualizarPerfil("Pedro Mod", "Sánchez Mod", 30, 80.5);
            Assert.AreEqual("Pedro Mod", Pedro.Nombre);
            Assert.AreEqual(30, Pedro.Edad);
            Assert.AreEqual(80.5, Pedro.Peso);

            // 3. Probar asignación de valores nulos (debe ser aceptada).
            Pedro.ActualizarPerfil("Pedro", "Sánchez", null, null);
            Assert.IsNull(Pedro.Edad);
            Assert.IsNull(Pedro.Peso);

            // 4. Probar límites de Edad (valor máximo + 1) - debe lanzar excepción.
            Assert.ThrowsException<ArgumentException>(() => Pedro.ActualizarPerfil("P", "S", 121, 50.0));

            // 5. Probar límites de Peso (valor máximo + 0.1) - debe lanzar excepción.
            Assert.ThrowsException<ArgumentException>(() => Pedro.ActualizarPerfil("P", "S", 30, 500.1));
        }

        /// <summary>
        /// Prueba la implementación del método ToString() para asegurar que devuelve una representación legible y completa del usuario.
        /// </summary>
        [TestMethod()]
        public void ToStringTest()
        {
            // Arrange
            Usuario Marta = new Usuario(7, "Marta", "@Contraseñavalida123", "Hernandez", "marta@gmail.com", true);
            Marta.Edad = 25;
            Marta.Peso = 65.5;

            // Act
            string result = Marta.ToString();

            // Assert: Verifica que la cadena resultante contiene todos los datos esperados.
            StringAssert.Contains(result, "Marta");
            StringAssert.Contains(result, "Hernandez");
            StringAssert.Contains(result, "marta@gmail.com");
            StringAssert.Contains(result, "Edad: 25");
            // Nota: Se verifica el formato con punto decimal (Invariant Culture)
            StringAssert.Contains(result, "Peso: 65.5 kg");
        }
    }
}
