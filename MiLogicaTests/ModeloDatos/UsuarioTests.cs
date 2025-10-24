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
        public void PermitirLoginTest()
        {
            Usuario Jose = new Usuario(1, "usuario1", "@Contraseñavalida123", "Pérez", "jose@gmail.com", false);
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



        [TestMethod()]
        public void ActualizarPerfilTest()
        {
            Usuario Pedro = new Usuario();
            Pedro.ActualizarPerfil("Pedro", "Sánchez", null,null);
            Assert.AreEqual("Pedro", Pedro.Nombre);
            Assert.AreEqual("Sánchez", Pedro.Apellidos);
            Pedro.ActualizarPerfil("Pedro", "Sánchez", 30, 80.5);
            Assert.AreEqual(30, Pedro.Edad);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Usuario Marta = new Usuario(7, "Marta", "@Contraseñavalida123", "Hernandez", "marta@gmail.com", true);
            Console.WriteLine(Marta.ToString());
            Assert.IsTrue(Marta.ToString().Contains("Marta"));
            Assert.IsTrue(Marta.ToString().Contains("Hernandez"));
            Assert.IsTrue(Marta.ToString().Contains("marta@gmail.com"));
        }
    }
}