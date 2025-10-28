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

        [TestInitialize]
        public void TestInitialize()
        {
            // Gracias a los cambios del Paso 1, cada vez que llamamos a "new CapaDatos()",
            // obtenemos una instancia NUEVA y LIMPIA con solo el usuario "oscar@gmail.com".
            capa = new CapaDatos();
        }

        [TestMethod()]
        public void ActualizarActividadTest()
        {
            // Arrange
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuarioOscar, "Actividad a actualizar");
            capa.GuardaActividad(act);
            int idActividadGuardada = act.Id;
            // Act
            act.Titulo = "Actividad actualizada";
            bool resultado = capa.ActualizaActividad(act);
            // Assert
            Assert.IsTrue(resultado);

        }

        [TestMethod()]
        public void ActualizarActividadTest_ConservarCamposNoEditados()
        {

            int idUsuario = capa.LeeUsuario("oscar@gmail.com").Id;

            TimeSpan duracionOriginal = TimeSpan.FromMinutes(90);
            int desnivelOriginal = 500;
            int fcMediaOriginal = 150;

            Actividad original = new Actividad(
                idUsuario, "Carrera Larga", 15.0, desnivelOriginal, duracionOriginal,
                DateTime.Now.AddDays(-1), TipoActividad.Running, "Test", fcMediaOriginal
            );
            capa.GuardaActividad(original);
            int idActividad = original.Id;

            Actividad actividadModificada = capa.LeeActividad(idActividad);

            // Modificar SOLO el título (simulando la edición en EditarActividad.aspx)
            string nuevoTitulo = "Título Editado y Conservado";
            actividadModificada.Titulo = nuevoTitulo;

            // Act: Guardar la actividad. Si CapaDatos.ActualizaActividad no transfiere 
            // Duracion/Desnivel/FCMedia, se perderán.
            bool resultado = capa.ActualizaActividad(actividadModificada);

            // Assert
            Assert.IsTrue(resultado, "La actualización debería ser exitosa.");

            Actividad final = capa.LeeActividad(idActividad);

            Assert.AreEqual(nuevoTitulo, final.Titulo, "El título debe haberse actualizado.");

            // Verificar que los campos NO EDITADOS SE MANTIENEN (Crucial para el FIX)
            Assert.AreEqual(duracionOriginal, final.Duracion, "La Duración no debería perderse.");
            Assert.AreEqual(desnivelOriginal, final.MetrosDesnivel, "El Desnivel no debería perderse.");
            Assert.AreEqual(fcMediaOriginal, final.FCMedia, "La FCMedia no debería perderse.");
        }

        /// <summary>
        /// Comprueba que se actualiza una actividad que inicialmente tenía FCMedia = null.
        /// </summary>
        [TestMethod()]
        public void ActualizaActividadTest_ConFCMediaNullANull()
        {
            // Arrange
            int idUsuario = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuario, "Sin FC", 10.0, 0, TimeSpan.FromMinutes(60), DateTime.Now, TipoActividad.Running, fcMedia: null);
            capa.GuardaActividad(act);

            // Act
            Actividad actModificada = capa.LeeActividad(act.Id);
            actModificada.Titulo = "Ahora con Título";
            bool resultado = capa.ActualizaActividad(actModificada);
            Actividad final = capa.LeeActividad(act.Id);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual("Ahora con Título", final.Titulo);
            Assert.IsNull(final.FCMedia, "FCMedia debe seguir siendo null.");
        }

        /// <summary>
        /// Comprueba que se actualiza FCMedia de un valor a otro.
        /// </summary>
        [TestMethod()]
        public void ActualizaActividadTest_ConFCMediaConValor()
        {
            // Arrange
            int idUsuario = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuario, "Con FC", 10.0, 0, TimeSpan.FromMinutes(60), DateTime.Now, TipoActividad.Running, fcMedia: 130);
            capa.GuardaActividad(act);

            // Act
            Actividad actModificada = capa.LeeActividad(act.Id);
            actModificada.FCMedia = 160; // Cambio el valor de un campo no editable (pero que el DAL debe persistir)
            actModificada.Kms = 12.0;
            bool resultado = capa.ActualizaActividad(actModificada);
            Actividad final = capa.LeeActividad(act.Id);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(160, final.FCMedia, "FCMedia debe haberse actualizado.");
            Assert.AreEqual(12.0, final.Kms, "Kms debe haberse actualizado.");
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
        public void LeeUsuarioPorIdTest_Exito()
        {
            // Arrange
            Usuario oscar = capa.LeeUsuario("oscar@gmail.com");
            int idOscar = oscar.Id;
            Usuario oscarSegundo = capa.LeeUsuarioPorId(idOscar);
            Assert.IsNotNull(oscarSegundo);
            Assert.AreEqual(oscar, oscarSegundo);
            Assert.AreEqual(oscar._passwordHash, oscarSegundo._passwordHash);

        }

        [TestMethod()]
        public void GuardaUsuarioTest_Fallo_EmailDuplicado()
        {
            // Arrange
            // Intentamos guardar un usuario con el email de "Oscar", que ya existe
            Usuario duplicado = new Usuario(0, "Oscar_Fuentes", "@Contraseñaseguraa123", "Repetido", "oscar@gmail.com", false);
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

        /// <summary>
        /// Comprueba que se actualizan correctamente los campos opcionales Edad y Peso.
        /// </summary>
        [TestMethod()]
        public void ActualizaUsuarioTest_ConDatosOpcionales()
        {
            // Arrange
            Usuario u = capa.LeeUsuario("oscar@gmail.com");
            u.Edad = 30;
            u.Peso = 75.5;

            // Act
            bool resultado = capa.ActualizaUsuario(u);

            // Assert
            Assert.IsTrue(resultado);
            Usuario uModificado = capa.LeeUsuario("oscar@gmail.com");
            Assert.AreEqual(30, uModificado.Edad);
            Assert.AreEqual(75.5, uModificado.Peso);

            // Act 2: Eliminar datos opcionales (pasar a null)
            u.Edad = null;
            u.Peso = null;
            bool resultado2 = capa.ActualizaUsuario(u);

            // Assert 2
            Assert.IsTrue(resultado2);
            Usuario uFinal = capa.LeeUsuario("oscar@gmail.com");
            Assert.IsNull(uFinal.Edad);
            Assert.IsNull(uFinal.Peso);
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
            string pass = "@Contraseñasegura123";

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
        public void EliminaActividadTest_Fallo_NoExiste()
        {
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
            Usuario alberto = new Usuario(0, "Alberto", "@Contraseñaseguraa123", "Lopez", "alberto@gmail.com", false);
            capa.GuardaUsuario(alberto);
            int idAlberto = capa.LeeUsuario("alberto@gmail.com").Id;
            capa.GuardaActividad(new Actividad(idAlberto, "Act1"));
            capa.GuardaActividad(new Actividad(idAlberto, "Act2"));

            // Guardamos una actividad de OTRO usuario para asegurar que no se mezcla
            Usuario ana = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            capa.GuardaUsuario(ana);
            capa.GuardaActividad(new Actividad(ana.Id, "Act Ana"));

            // Act
            List<Actividad> actividadesAlberto = capa.ObtenerActividadesUsuario(idAlberto);

            // Assert
            Assert.IsNotNull(actividadesAlberto);
            int numActividadesOscar = capa.NumActividades(idAlberto);
            Assert.AreEqual(2, numActividadesOscar); // Solo debe devolver las 2 de Oscar
            Assert.IsTrue(actividadesAlberto.Any(a => a.Titulo == "Act1"));
            Assert.IsTrue(actividadesAlberto.Any(a => a.Titulo == "Act2"));
            Assert.AreEqual(actividadesAlberto[1].Titulo, "Act2");
        }

        [TestMethod()]
        public void VerificaAdminId1Test()
        {
            Usuario admin = capa.LeeUsuarioPorId(1);
            Assert.IsNotNull(admin);
            Assert.AreEqual("Admin", admin.Nombre);
        }

        [TestMethod()]
        public void NumUsuariosTest()
        {
            Usuario segundo = new Usuario(capa._nextUserId, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            capa.GuardaUsuario(segundo);
            Usuario tercero = new Usuario(capa._nextUserId, "Luis", "@Contraseñaseguraa123", "Martinez", "luis@gmail.com", false);
            capa.GuardaUsuario(tercero);
            int numUsuarios = capa.NumUsuarios();
<<<<<<< HEAD
            Assert.AreEqual(7, numUsuarios); // Debería haber 6 usuarios ahora (admin creado)
=======
            Assert.AreEqual(7, numUsuarios); // Debería haber 7 usuarios ahora (admin creado)
>>>>>>> 9a98641d6f515f243c9b0fc6e2c44a0bd06e599d
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
            Assert.AreEqual(4, capa.NumUsuariosActivos()); // Debería haber 4 usuarios activos
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
        public void ObtenerActividadesUsuarioSinActividades_Test()
        {
            Usuario alberto = new Usuario(0, "Alberto", "@Contraseñaseguraa123", "Lopez", "alberto@gmail.com", false);
            capa.GuardaUsuario(alberto);
            List <Actividad> actividadesAlberto = capa.ObtenerActividadesUsuario(alberto.Id);
            Assert.AreEqual(actividadesAlberto.Count, 0);
        }

        [TestMethod()]
        public void NumActividadesTest()
        {
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            // El constructor de CapaDatos crea 7 actividades para Oscar
            Assert.AreEqual(8, capa.NumActividades(idUsuarioOscar));

            // Creamos un nuevo usuario sin actividades (aparte de las iniciales)
            Usuario u = new Usuario(0, "Nuevo", "@NuevaPassword123", "User", "nuevo@gmail.com", false);
            capa.GuardaUsuario(u);

            Assert.AreEqual(0, capa.NumActividades(u.Id));

            // Agregamos una actividad
            capa.GuardaActividad(new Actividad(u.Id, "Nueva Carrera"));

            Assert.AreEqual(1, capa.NumActividades(u.Id));
        }
    }
}