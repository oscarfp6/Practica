using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiLogica.ModeloDatos;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Datos.Tests
{
    [TestClass()]
    public class CapaDatosTest
    {
        private CapaDatos capa;

        // ESTO ES CLAVE: Se ejecuta ANTES de CADA test
        [TestInitialize]
        public void TestInitialize()
        {
            // Gracias a los cambios del Paso 1, cada vez que llamamos a "new CapaDatos()",
            // obtenemos una instancia NUEVA y LIMPIA con solo el usuario "oscar@gmail.com".
            capa = new CapaDatos();
        }

        [TestMethod()]
        public void CapaDatosTestConstructor()
        {
            Assert.IsNotNull(capa);
            Console.WriteLine("Número de usuarios iniciales: " + capa.NumUsuarios());
            Assert.IsNotNull(capa.LeeUsuario("oscar@gmail.com"));

        }


        [TestMethod()]
        public void GuardaUsuarioTest_Exito()
        {
            // Arrange
            Usuario nuevo = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            int usuariosAntes = capa.NumUsuarios(); // Debería ser 1

            // Act
            bool resultado = capa.GuardaUsuario(nuevo);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(usuariosAntes + 1, capa.NumUsuarios()); // Ahora debería ser 2
            Assert.IsNotNull(capa.LeeUsuario("ana@gmail.com"));
            Assert.AreEqual("Ana", capa.LeeUsuario("ana@gmail.com").Nombre);
        }

        [TestMethod()]
        public void GuardaUsuarioTest_Fallo_EmailDuplicado()
        {
            // Arrange
            // Intentamos guardar un usuario con el email de "Oscar", que ya existe
            Usuario duplicado = new Usuario(0, "Oscar2", "@Contraseñaseguraa123", "Repetido", "oscar@gmail.com", false);
            int usuariosAntes = capa.NumUsuarios(); // Debería ser 1

            // Act
            bool resultado = capa.GuardaUsuario(duplicado);

            // Assert
            Assert.IsFalse(resultado);
            Assert.AreEqual(usuariosAntes, capa.NumUsuarios()); // El número no debe cambiar
        }

        [TestMethod()]
        public void ActualizaUsuarioTest_Exito()
        {
            // Arrange
            Usuario oscar = capa.LeeUsuario("oscar@gmail.com");
            oscar.Nombre = "Oscar Modificado";
            oscar.Suscripcion = false;

            // Act
            bool resultado = capa.ActualizaUsuario(oscar);

            // Assert
            Assert.IsTrue(resultado);
            Usuario oscarModificado = capa.LeeUsuario("oscar@gmail.com");
            Assert.AreEqual("Oscar Modificado", oscarModificado.Nombre);
            Assert.IsFalse(oscarModificado.Suscripcion);

            Usuario nuevo = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            capa.GuardaUsuario(nuevo);
            Usuario ana = capa.LeeUsuario("ana@gmail.com");
            ana.Nombre = "Ana Modificada";
            ana.Suscripcion = false;
            bool resultadoAna = capa.ActualizaUsuario(ana);
            Assert.IsTrue(resultadoAna);
        }

        [TestMethod()]
        public void ActualizaUsuarioTest_Fallo_NoExiste()
        {
            // Arrange
            Usuario fantasma = new Usuario(99, "Fantasma", "@Contraseñaseguraa123", "No Existe", "fantasma@gmail.com", false);
            // Act
            fantasma.Nombre = "Fantasma Modificado";
            bool resultado = capa.ActualizaUsuario(fantasma);

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void LeeUsuarioTest_Exito()
        {
            // Arrange (Hecho en TestInitialize)

            // Act
            Usuario oscar = capa.LeeUsuario("oscar@gmail.com");

            // Assert
            Assert.IsNotNull(oscar);
            Assert.AreEqual("Oscar", oscar.Nombre);
        }

        [TestMethod()]
        public void LeeUsuarioTest_Fallo_NoExiste()
        {
            // Arrange

            // Act
            Usuario fantasma = capa.LeeUsuario("fantasma@gmail.com");

            // Assert
            Assert.IsNull(fantasma);
        }

        [TestMethod()]
        public void ValidaUsuarioTest_Exito()
        {
            // Arrange
            string email = "oscar@gmail.com";
            string pass = "@Contraseñaseguraa123";

            // Act
            bool resultado = capa.ValidaUsuario(email, pass);

            // Assert
            Assert.IsTrue(resultado);
        }

        [TestMethod()]
        public void ValidaUsuarioTest_Fallo_PasswordIncorrecta()
        {
            // Arrange
            string email = "oscar@gmail.com";
            string pass = "passincorrecta";

            // Act
            bool resultado = capa.ValidaUsuario(email, pass);

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void GuardaActividadTest_Exito()
        {
            // Arrange
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuarioOscar, "Salida de prueba");
            int actividadesAntes = capa.NumActividades(idUsuarioOscar); // Debería ser 0

            // Act
            bool resultado = capa.GuardaActividad(act);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(actividadesAntes + 1, capa.NumActividades(idUsuarioOscar));
            Assert.AreNotEqual(0, act.Id); // El ID debería haberse asignado
        }

        [TestMethod()]
        public void GuardaActividadTest_Fallo_UsuarioNoExiste()
        {
            // Arrange
            Actividad act = new Actividad(99, "Actividad con usuario inexistente"); // Usuario ID 99 no existe
            // Act
            bool resultado = capa.GuardaActividad(act);
            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void GuardaActividadTest_Fallo_IdUsuarioCero()
        {
            // Arrange
            Actividad act = new Actividad(0, "Actividad con usuario ID 0"); // Usuario ID 0 no es válido
            // Act
            bool resultado = capa.GuardaActividad(act);
            // Assert
            Assert.IsFalse(resultado);
        }


        [TestMethod()]
        public void EliminaActividadTest_Exito()
        {
            // Arrange
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuarioOscar, "Carrera a borrar");
            capa.GuardaActividad(act);
            int idActividadGuardada = act.Id;
            int actividadesAntes = capa.NumActividades(idUsuarioOscar); // Debería ser 1

            // Act
            bool resultado = capa.EliminaActividad(idActividadGuardada);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(actividadesAntes - 1, capa.NumActividades(idUsuarioOscar)); // Debería ser 0
            Assert.IsNull(capa.LeeActividad(idActividadGuardada));
        }

        [TestMethod()]
        public void EliminaActividadTest_Fallo_NoExiste() { 
            // Arrange
            int idActividadInexistente = 999; // ID que no existe
            // Act
            bool resultado = capa.EliminaActividad(idActividadInexistente);
            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void ObtenerActividadesUsuarioTest_Exito()
        {
            // Arrange
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            capa.GuardaActividad(new Actividad(idUsuarioOscar, "Act1"));
            capa.GuardaActividad(new Actividad(idUsuarioOscar, "Act2"));

            // Guardamos una actividad de OTRO usuario para asegurar que no se mezcla
            Usuario ana = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            capa.GuardaUsuario(ana);
            capa.GuardaActividad(new Actividad(ana.Id, "Act Ana"));


            // Act
            List<Actividad> actividadesOscar = capa.ObtenerActividadesUsuario(idUsuarioOscar);

            // Assert
            Assert.IsNotNull(actividadesOscar);
            Assert.AreEqual(2, actividadesOscar.Count); // Solo debe devolver las 2 de Oscar
            Assert.IsTrue(actividadesOscar.Any(a => a.Titulo == "Act1"));
            Assert.IsTrue(actividadesOscar.Any(a => a.Titulo == "Act2"));
        }

        [TestMethod()]
        public void ValidaUsuarioTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NumUsuariosTest()
        {
            Usuario segundo = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            capa.GuardaUsuario(segundo);
            Usuario tercero = new Usuario(0, "Luis", "@Contraseñaseguraa123", "Martinez", "luis@gmail.com", false);
            capa.GuardaUsuario(tercero);
            int numUsuarios = capa.NumUsuarios();
            Assert.AreEqual(3, numUsuarios); // Debería haber 3 usuarios ahora
        }




        [TestMethod()]
        public void NumUsuariosActivosTest()
        {
            Usuario oscar = capa.LeeUsuario("oscar@gmail.com");
            Assert.AreEqual(EstadoUsuario.Activo, oscar.Estado);
            Usuario segundo = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", true);
            capa.GuardaUsuario(segundo);
            Assert.AreEqual(EstadoUsuario.Activo, segundo.Estado);
            Usuario tercero = new Usuario(0, "Luis", "@Contraseñaseguraa123", "Martinez", "luis@gmail.com", false);
            tercero.Estado = EstadoUsuario.Inactivo;
            capa.GuardaUsuario(segundo);
            Assert.AreEqual(2, capa.NumUsuariosActivos()); // Debería haber 2 usuarios activos
        }







        [TestMethod()]
        public void LeeActividadTestExito()
        {
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuarioOscar, "Actividad para leer");
            capa.GuardaActividad(act);
            int idActividadGuardada = act.Id;
            Actividad actividadLeida = capa.LeeActividad(idActividadGuardada);
            Assert.IsNotNull(actividadLeida);
        }

        [TestMethod()]
        public void LeeActividadTestFallo_NoExiste()
        {
            Actividad actividadLeida = capa.LeeActividad(9999); // ID que no existe
            Assert.IsNull(actividadLeida);
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