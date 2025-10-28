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
    public class UsuarioTests
    {

 
        [TestMethod()]
        public void Constructor_ConEmailInvalido_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Usuario(1, "Test", "@PasswordValida123", "User", "email-invalido", false)
            );
        }

        [TestMethod()]
        public void Constructor_ConPasswordInvalida_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Usuario(1, "Test", "corta", "User", "test@gmail.com", false)
            );
        }


        [TestMethod()]
        public void SetNombre_ConValorNuloOEspacios_LanzaArgumentException()
        {
            Usuario usuario = new Usuario(1, "Test", "@PasswordValida123", "User", "test@gmail.com", false);
            Assert.ThrowsException<ArgumentException>(() => usuario.Nombre = "   ");
        }


        [TestMethod()]
        public void SetApellidos_ConValorNuloOEspacios_LanzaArgumentException()
        {
            Usuario usuario = new Usuario(1, "Test", "@PasswordValida123", "User", "test@gmail.com", false);
            Assert.ThrowsException<ArgumentException>(() => usuario.Apellidos = "");
        }

        [TestMethod()]
        public void SetNombre_ConDigitos_LanzaArgumentException()
        {
            Usuario usuario = new Usuario(1, "Test", "@PasswordValida123", "User", "test@gmail.com", false);
            Assert.ThrowsException<ArgumentException>(() => usuario.Nombre = "Oscar456");
        }

        [TestMethod()]
        public void SetApellidos_ConDigitos_LanzaArgumentException()
        {
            Usuario usuario = new Usuario(1, "Test", "@PasswordValida123", "User", "test@gmail.com", false);
            Assert.ThrowsException<ArgumentException>(() => usuario.Apellidos = "Martínez5");
        }


        [TestMethod()]
        public void PermitirLoginTest()
        {
            Usuario Jose = new Usuario(1, "usuario_jose", "@Contraseñavalida123", "Pérez", "jose@gmail.com", false);
            Assert.IsTrue(Jose.PermitirLogin("@Contraseñavalida123"));
            Console.WriteLine($"Estado tras login exitoso: {Jose.Estado}");
            Assert.AreEqual(EstadoUsuario.Activo, Jose.Estado);

            Jose.PermitirLogin("wrongpass1");
            Jose.PermitirLogin("wrongpass2");
            Assert.IsFalse(Jose.PermitirLogin("wrongpass3"));
            Console.WriteLine($"Estado tras 3 fallos: {Jose.Estado}");
            Assert.AreEqual(EstadoUsuario.Bloqueado, Jose.Estado);

            Console.WriteLine("Esperando 5 segundos para desbloquear..., introducimos contraseña correcta");
            Assert.IsFalse(Jose.PermitirLogin("@Contraseñavalida123"));
        }

        [TestMethod()]
        public void PermitirLoginUsuarioInactivoBloqueoEstrictoTest()
        {
            Usuario David = new Usuario(8, "David", "@Contraseñavalida123", "Martín", "david@gmail.com", false);

            David.LastLogin = DateTime.Now.AddDays(-200);
            David.VerificarInactividad();
            Console.WriteLine($"Estado Inicial: {David.Estado}");
            Assert.AreEqual(EstadoUsuario.Inactivo, David.Estado);

            Usuario InactivoReactivado = new Usuario(9, "Iñigo", "@Contraseñavalida123", "Lopez", "inigo@gmail.com", false);
            InactivoReactivado.LastLogin = DateTime.Now.AddDays(-200);
            InactivoReactivado.VerificarInactividad();
            Assert.IsTrue(InactivoReactivado.PermitirLogin("@Contraseñavalida123"));
            Assert.AreEqual(EstadoUsuario.Activo, InactivoReactivado.Estado, "El login correcto debe reactivar.");

            Assert.IsFalse(David.PermitirLogin("wrongpass1"));
            Console.WriteLine($"Estado tras 1 fallo estando Inactivo: {David.Estado}");
            Assert.AreEqual(EstadoUsuario.Bloqueado, David.Estado, "Un solo fallo en Inactivo debe bloquear inmediatamente");
        }

        [TestMethod()]
        public void PermitirLogin_UsuarioInactivo_SeReactivaExitosamente()
        {
            // Arrange
            Usuario Inactivo = new Usuario(9, "Iñigo", "@Contraseñavalida123", "Lopez", "inigo@gmail.com", false);
            // Simular inactividad de 6 meses
            Inactivo.LastLogin = DateTime.Now.AddDays(-200);
            Inactivo.VerificarInactividad();
            Assert.AreEqual(EstadoUsuario.Inactivo, Inactivo.Estado);

            // Act
            bool resultado = Inactivo.PermitirLogin("@Contraseñavalida123");

            // Assert (Debe ser Activo y conceder acceso)
            Assert.IsTrue(resultado, "El login con pass correcta debería reactivar la cuenta Inactiva.");
            Assert.AreEqual(EstadoUsuario.Activo, Inactivo.Estado, "El estado debe cambiar a Activo tras el login exitoso.");
        }

        [TestMethod()]
        public void ComprobarPassWordTest()
        {
            Usuario juan = new Usuario(2, "juan", "@Contraseñavalida123", "fernandez", "juan@gmail.com", false);
            Assert.IsTrue(juan.ComprobarPassWord("@Contraseñavalida123"));
            Assert.IsFalse(juan.ComprobarPassWord("fake"));
        }

        [TestMethod()]
        public void DesbloquearUsuarioTest()
        {
            Usuario Luis = new Usuario(4, "Luis", "@Contraseñavalida123", "Martinez", "luis@gmail.com", true);
            Luis.PermitirLogin("wrongpass");
            Luis.PermitirLogin("wrongpass");
            Luis.PermitirLogin("wrongpass");
            Console.WriteLine(Luis.Estado);
            Assert.IsFalse(Luis.PermitirLogin("@Contraseñavalida123"));
            Luis.BloqueadoHasta = DateTime.Now.AddMinutes(-1); // Establece BloqueadoHasta a 1 minuto en el pasado.
            Assert.IsTrue(Luis.DesbloquearUsuario("luis@gmail.com", "@Contraseñavalida123"));
            Console.WriteLine(Luis.Estado);
            Assert.IsTrue(Luis.PermitirLogin("@Contraseñavalida123"));
        }

        [TestMethod()]
        public void DesbloquarUsuarioActivoTest()
        {
            Usuario Ana = new Usuario(2, "Ana", "@Contraseñavalida123", "García", "ana@gmail.com", false);
            Assert.IsFalse(Ana.DesbloquearUsuario("ana@gmail.com", "@@Contraseñavalida123"));
        }

        [TestMethod()]
        public void DesbloquearUsuarioTest_FalloPorCooldownActivo()
        {
            // Arrange: Bloquear para activar el cooldown de 2 minutos
            Usuario usuario = new Usuario(10, "Test", "@Contraseñavalida123", "User", "test2@gmail.com", false);
            usuario.PermitirLogin("wrong1");
            usuario.PermitirLogin("wrong2");
            usuario.PermitirLogin("wrong3");
            Assert.AreEqual(EstadoUsuario.Bloqueado, usuario.Estado);

            // Act: Intentar desbloquear INMEDIATAMENTE
            bool resultado = usuario.DesbloquearUsuario("test2@gmail.com", "@Contraseñavalida123");

            // Assert
            Assert.IsFalse(resultado, "El desbloqueo debe fallar porque el cooldown está activo.");
            Assert.AreEqual(EstadoUsuario.Bloqueado, usuario.Estado, "El estado debe permanecer Bloqueado.");
        }

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

        [TestMethod()]
        public void VerificarInactividadTest()
        {
            Usuario Lucia = new Usuario(6, "Lucia", "@Contraseñavalida123", "Ruiz", "lucia@gmail.com", false);
            Lucia.LastLogin = DateTime.Now.AddDays(-200);
            Console.WriteLine(Lucia.Estado);
            Assert.IsTrue(Lucia.Estado == EstadoUsuario.Activo);
            Lucia.VerificarInactividad();
            Assert.IsTrue(Lucia.Estado == EstadoUsuario.Inactivo);
        }

        [TestMethod()]
        public void CambiarPasswordTest()
        {
            Usuario oscar = new Usuario(3, "oscar", "@Contraseñavalida123", "Lopez", "oscar@gmail.com", false);
            Assert.IsTrue(oscar.CambiarPassword("@Contraseñavalida123", "@contraseñaSegura123"));
        }

        [TestMethod()]
        public void CambiarPasswordUsuarioActivoTest()
        {
            Usuario Maria = new Usuario(2, "Maria", "@Contraseñavalida123", "García", "maria@gmail.com", true);
            Console.WriteLine(Maria.Estado);
            Assert.IsFalse(Maria.CambiarPassword("wrongPass", "newPass"));
            Assert.IsTrue(Maria.CambiarPassword("@Contraseñavalida123", "@contraseñaSegura123"));


            Assert.IsTrue(Maria.ComprobarPassWord("@contraseñaSegura123"));
            Console.WriteLine("Probamos a cambiar la contraseña a una inválida/insegura");
            if (Maria.CambiarPassword("newPass", "short"))
                Console.WriteLine("La contraseña se ha cambiado a una insegura, hay un error.");
            else
                Console.WriteLine("La contraseña no cumple los requisitos de seguridad, no se ha cambiado.");
            Assert.IsFalse(Maria.ComprobarPassWord("short"));

        }

        [TestMethod()]
        public void CambiarPasswordUsuarioBloqueadooTest()
        {
            Usuario Ana = new Usuario(2, "Ana", "@Contraseñavalida123", "García", "ana@gmail.com", false);
            Ana.PermitirLogin("wrongpass");
            Ana.PermitirLogin("wrongpass");
            Ana.PermitirLogin("wrongpass");
            Console.WriteLine(Ana.Estado);
            Assert.IsFalse(Ana.CambiarPassword("@Contraseñavalida123", "newPass"));
        }

        /// <summary>
        /// Comprueba que un administrador puede establecer una nueva contraseña de forma segura.
        /// </summary>
        [TestMethod()]
        public void AdminEstablecerPasswordTest_Exito()
        {
            // Arrange: Un usuario bloqueado o en cualquier estado
            Usuario u = new Usuario(12, "TestAdmin", "@OldPass123456@", "User", "adminchange@gmail.com", false);
            u.Estado = EstadoUsuario.Bloqueado;
            u.BloqueadoHasta = DateTime.Now.AddMinutes(5);

            string nuevaPassSegura = "@NuevaPassSegura789";

            // Act
            bool resultado = u.AdminEstablecerPassword(nuevaPassSegura);

            // Assert
            Assert.IsTrue(resultado, "El cambio de contraseña por admin debe ser exitoso.");
            Assert.IsTrue(u.ComprobarPassWord(nuevaPassSegura), "La nueva contraseña debe funcionar.");
            Assert.AreEqual(EstadoUsuario.Activo, u.Estado, "El estado debe restablecerse a Activo.");
            Assert.IsNull(u.BloqueadoHasta, "El cooldown debe eliminarse.");
        }

        /// <summary>
        /// Comprueba que un administrador NO puede establecer una contraseña insegura.
        /// </summary>
        [TestMethod()]
        public void AdminEstablecerPasswordTest_FalloInsegura()
        {
            // Arrange
            Usuario u = new Usuario(13, "TestAdmin", "@OldPass123456@", "User", "adminchange2@gmail.com", false);
            string nuevaPassInsegura = "short";
            string passOriginal = "@OldPass123456@";

            // Act
            bool resultado = u.AdminEstablecerPassword(nuevaPassInsegura);

            // Assert
            Assert.IsFalse(resultado, "El cambio de contraseña por admin debe fallar si es insegura.");
            Assert.IsTrue(u.ComprobarPassWord(passOriginal), "La contraseña original debe conservarse.");
            Assert.AreEqual(EstadoUsuario.Activo, u.Estado, "El estado no cambia al fallar la validación de la nueva pass.");
        }

        [TestMethod()]
        public void ActualizarPerfilTest()
        {
            Usuario Pedro = new Usuario();
            Pedro.ActualizarPerfil("Pedro", "Sánchez", null, null);
            Assert.AreEqual("Pedro", Pedro.Nombre);
            Assert.AreEqual("Sánchez", Pedro.Apellidos);

            // Probar asignación de valores válidos
            Pedro.ActualizarPerfil("Pedro Mod", "Sánchez Mod", 30, 80.5);
            Assert.AreEqual("Pedro Mod", Pedro.Nombre);
            Assert.AreEqual(30, Pedro.Edad);
            Assert.AreEqual(80.5, Pedro.Peso);

            // Probar asignación de valores nulos
            Pedro.ActualizarPerfil("Pedro", "Sánchez", null, null);
            Assert.IsNull(Pedro.Edad);
            Assert.IsNull(Pedro.Peso);

            // Probar límites de Edad (debe lanzar excepción)
            Assert.ThrowsException<ArgumentException>(() => Pedro.ActualizarPerfil("P", "S", 121, 50.0));
            // Probar límites de Peso (debe lanzar excepción)
            Assert.ThrowsException<ArgumentException>(() => Pedro.ActualizarPerfil("P", "S", 30, 500.1));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Usuario Marta = new Usuario(7, "Marta", "@Contraseñavalida123", "Hernandez", "marta@gmail.com", true);
            Marta.Edad = 25;
            Marta.Peso = 65.5;

            // Act
            string result = Marta.ToString();

            // Assert
            StringAssert.Contains(result, "Marta");
            StringAssert.Contains(result, "Hernandez");
            StringAssert.Contains(result, "marta@gmail.com");
            StringAssert.Contains(result, "Edad: 25");
            // Nota: Se verifica el formato con punto decimal (Invariant Culture)
            StringAssert.Contains(result, "Peso: 65.5 kg");
        }
    }
}