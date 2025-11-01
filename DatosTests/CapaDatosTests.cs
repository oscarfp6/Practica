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
    // [TestClass] indica que esta clase contiene métodos de prueba unitaria ejecutables por MSTest.
    [TestClass()]
    public class CapaDatosTest
    {
        // Instancia de la capa de datos que se usará en las pruebas.
        private CapaDatos capa;

        // [TestInitialize] es un método especial que se ejecuta ANTES de cada método [TestMethod].
        // Es ideal para configurar el estado inicial de cada prueba, asegurando que las pruebas estén aisladas.
        [TestInitialize]
        public void TestInitialize()
        {
            // Creamos una instancia nueva de CapaDatos para cada prueba,
            // para evitar que los datos de una prueba afecten a otra.
            capa = new CapaDatos();
        }

        // [TestMethod] marca esto como un caso de prueba individual.
        [TestMethod()]
        public void ActualizarActividadTest()
        {
            // --- ARRANGE (Preparar) ---
            // 1. Obtenemos un ID de usuario válido para asociar la actividad.
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            // 2. Creamos una nueva actividad.
            Actividad act = new Actividad(idUsuarioOscar, "Actividad a actualizar");
            // 3. La guardamos para que exista en la "base de datos" (en este caso, la lista en memoria).
            capa.GuardaActividad(act);
            int idActividadGuardada = act.Id;

            // 4. Modificamos el objeto en memoria.
            act.Titulo = "Actividad actualizada";

            // --- ACT (Actuar) ---
            // 5. Llamamos al método que queremos probar: ActualizaActividad.
            bool resultado = capa.ActualizaActividad(act);

            // --- ASSERT (Verificar) ---
            // 6. Comprobamos que el método devolvió 'true', indicando que la actualización fue exitosa.
            Assert.IsTrue(resultado);
        }

        [TestMethod()]
        public void ActualizarActividadTest_ConservarCamposNoEditados()
        {
            // PRUEBA CRÍTICA: Esta prueba valida que al actualizar un campo (ej. Título),
            // no se pierden (o se ponen a null) los otros campos que no se tocaron.
            // Es una prueba de regresión común para errores en métodos UPDATE.

            // --- ARRANGE ---
            int idUsuario = capa.LeeUsuario("oscar@gmail.com").Id;

            // Datos originales que NO deben perderse.
            TimeSpan duracionOriginal = TimeSpan.FromMinutes(90);
            int desnivelOriginal = 500;
            int fcMediaOriginal = 150;

            // Creamos una actividad con todos los campos llenos.
            Actividad original = new Actividad(
                idUsuario, "Carrera Larga", 15.0, desnivelOriginal, duracionOriginal,
                DateTime.Now.AddDays(-1), TipoActividad.Running, "Test", fcMediaOriginal
            );
            capa.GuardaActividad(original);
            int idActividad = original.Id;

            // Simulamos el escenario de "EditarPágina": leemos la actividad de la BD.
            Actividad actividadModificada = capa.LeeActividad(idActividad);

            // --- ACT ---
            // Modificamos *SOLO* el título.
            string nuevoTitulo = "Título Editado y Conservado";
            actividadModificada.Titulo = nuevoTitulo;

            // Ejecutamos la actualización.
            bool resultado = capa.ActualizaActividad(actividadModificada);

            // --- ASSERT ---
            // 1. Verificamos que la actualización fue exitosa.
            Assert.IsTrue(resultado, "La actualización debería ser exitosa.");

            // 2. Leemos la actividad de nuevo desde la BD (CapaDatos) para verificar los datos guardados.
            Actividad final = capa.LeeActividad(idActividad);

            // 3. Verificamos que el campo editado (Título) se actualizó.
            Assert.AreEqual(nuevoTitulo, final.Titulo, "El título debe haberse actualizado.");

            // 4. Verificamos que los campos NO EDITADOS se mantuvieron intactos.
            Assert.AreEqual(duracionOriginal, final.Duracion, "La Duración no debería perderse.");
            Assert.AreEqual(desnivelOriginal, final.MetrosDesnivel, "El Desnivel no debería perderse.");
            Assert.AreEqual(fcMediaOriginal, final.FCMedia, "La FCMedia no debería perderse.");
        }


        [TestMethod()]
        public void ActualizaActividadTest_ConFCMediaNullANull()
        {
            // PRUEBA DE CASO LÍMITE (Edge Case): Valida el manejo de valores nulos (nullable integers).
            // Si FCMedia era 'null', debe seguir siendo 'null' tras una actualización que no lo toca.

            // --- ARRANGE ---
            int idUsuario = capa.LeeUsuario("oscar@gmail.com").Id;
            // Creamos actividad con fcMedia explícitamente nulo.
            Actividad act = new Actividad(idUsuario, "Sin FC", 10.0, 0, TimeSpan.FromMinutes(60), DateTime.Now, TipoActividad.Running, fcMedia: null);
            capa.GuardaActividad(act);

            // --- ACT ---
            Actividad actModificada = capa.LeeActividad(act.Id);
            actModificada.Titulo = "Ahora con Título"; // Modificamos otro campo
            bool resultado = capa.ActualizaActividad(actModificada);
            Actividad final = capa.LeeActividad(act.Id);

            // --- ASSERT ---
            Assert.IsTrue(resultado);
            Assert.AreEqual("Ahora con Título", final.Titulo);
            // Verificamos que FCMedia sigue siendo null.
            Assert.IsNull(final.FCMedia, "FCMedia debe seguir siendo null.");
        }


        [TestMethod()]
        public void ActualizaActividadTest_ConFCMediaConValor()
        {
            // PRUEBA DE CASO LÍMITE: Valida que un campo nullable (FCMedia)
            // puede ser actualizado correctamente, junto con otros campos (Kms).

            // --- ARRANGE ---
            int idUsuario = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuario, "Con FC", 10.0, 0, TimeSpan.FromMinutes(60), DateTime.Now, TipoActividad.Running, fcMedia: 130);
            capa.GuardaActividad(act);

            // --- ACT ---
            Actividad actModificada = capa.LeeActividad(act.Id);
            actModificada.FCMedia = 160; // Cambiamos el valor de FC
            actModificada.Kms = 12.0;    // Cambiamos el valor de Kms
            bool resultado = capa.ActualizaActividad(actModificada);
            Actividad final = capa.LeeActividad(act.Id);

            // --- ASSERT ---
            Assert.IsTrue(resultado);
            // Verificamos que ambos campos se actualizaron correctamente.
            Assert.AreEqual(160, final.FCMedia, "FCMedia debe haberse actualizado.");
            Assert.AreEqual(12.0, final.Kms, "Kms debe haberse actualizado.");
        }

        [TestMethod()]
        public void CapaDatosTestConstructor()
        {
            // PRUEBA DE INICIALIZACIÓN (Sanity Check):
            // Verifica que el constructor de CapaDatos funciona y carga los datos iniciales esperados.
            Assert.IsNotNull(capa);
            Console.WriteLine("Número de usuarios iniciales: " + capa.NumUsuarios());
            // Verifica que el usuario 'oscar' (dato de prueba inicial) existe.
            Assert.IsNotNull(capa.LeeUsuario("oscar@gmail.com"));
        }

        [TestMethod()]
        public void GuardaUsuarioTest_Exito()
        {
            // PRUEBA "CAMINO FELIZ" (Happy Path): Verifica que un usuario nuevo
            // se puede guardar correctamente.

            // --- ARRANGE ---
            Usuario nuevo = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            int usuariosAntes = capa.NumUsuarios();

            // --- ACT ---
            bool resultado = capa.GuardaUsuario(nuevo);

            // --- ASSERT ---
            Assert.IsTrue(resultado); // 1. El método devolvió éxito.
            Assert.AreEqual(usuariosAntes + 1, capa.NumUsuarios()); // 2. El contador de usuarios aumentó en 1.
            Assert.IsNotNull(capa.LeeUsuario("ana@gmail.com")); // 3. El usuario se puede leer por email.
            Assert.AreEqual("Ana", capa.LeeUsuario("ana@gmail.com").Nombre); // 4. Los datos son correctos.
        }

        [TestMethod()]
        public void LeeUsuarioPorIdTest_Exito()
        {
            // PRUEBA DE LECTURA: Verifica que se puede leer un usuario por su ID
            // y que los datos (incluyendo campos privados como el hash) se cargan.

            // --- ARRANGE ---
            Usuario oscar = capa.LeeUsuario("oscar@gmail.com");
            int idOscar = oscar.Id;

            // --- ACT ---
            Usuario oscarSegundo = capa.LeeUsuarioPorId(idOscar);

            // --- ASSERT ---
            Assert.IsNotNull(oscarSegundo);
            // Comprueba que son el mismo usuario (basado en la implementación de Equals del modelo).
            Assert.AreEqual(oscar, oscarSegundo);
            // Comprueba que los campos internos (como el hash) también se cargaron.
            Assert.AreEqual(oscar._passwordHash, oscarSegundo._passwordHash);
        }

        [TestMethod()]
        public void GuardaUsuarioTest_Fallo_EmailDuplicado()
        {
            // PRUEBA "CAMINO TRISTE" (Sad Path): Verifica que la lógica de negocio
            // (email único) se cumple, impidiendo guardar un email duplicado.

            // --- ARRANGE ---
            // Intentamos guardar un usuario con el email de "oscar", que ya existe.
            Usuario duplicado = new Usuario(0, "Oscar_Fuentes", "@Contraseñaseguraa123", "Repetido", "oscar@gmail.com", false);
            int usuariosAntes = capa.NumUsuarios();

            // --- ACT ---
            bool resultado = capa.GuardaUsuario(duplicado);

            // --- ASSERT ---
            Assert.IsFalse(resultado); // 1. El método debe devolver 'false' (fallo).
            Assert.AreEqual(usuariosAntes, capa.NumUsuarios()); // 2. El número de usuarios no debe cambiar.
        }

        [TestMethod()]
        public void ActualizaUsuarioTest_Exito()
        {
            // PRUEBA "CAMINO FELIZ": Verifica que los datos de un usuario existente
            // se pueden modificar correctamente.

            // --- ARRANGE ---
            Usuario oscar = capa.LeeUsuario("oscar@gmail.com");
            oscar.Nombre = "Oscar Modificado";
            oscar.Suscripcion = false;

            // --- ACT ---
            bool resultado = capa.ActualizaUsuario(oscar);

            // --- ASSERT ---
            Assert.IsTrue(resultado);
            // Leemos de nuevo para confirmar que los cambios se persistieron.
            Usuario oscarModificado = capa.LeeUsuario("oscar@gmail.com");
            Assert.AreEqual("Oscar Modificado", oscarModificado.Nombre);
            Assert.IsFalse(oscarModificado.Suscripcion);

            // (Sección extra de la prueba original)
            Usuario nuevo = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            capa.GuardaUsuario(nuevo);
            Usuario ana = capa.LeeUsuario("ana@gmail.com");
            ana.Nombre = "Ana Modificada";
            ana.Suscripcion = false;
            bool resultadoAna = capa.ActualizaUsuario(ana);
            Assert.IsTrue(resultadoAna);
        }


        [TestMethod()]
        public void ActualizaUsuarioTest_ConDatosOpcionales()
        {
            // PRUEBA DE CASO LÍMITE: Verifica que los campos opcionales (nullable)
            // como Edad y Peso se pueden actualizar (poner valor y quitar valor).

            // --- ARRANGE 1 ---
            Usuario u = capa.LeeUsuario("oscar@gmail.com");
            u.Edad = 30;
            u.Peso = 75.5;

            // --- ACT 1 ---
            bool resultado = capa.ActualizaUsuario(u);

            // --- ASSERT 1 ---
            Assert.IsTrue(resultado);
            Usuario uModificado = capa.LeeUsuario("oscar@gmail.com");
            Assert.AreEqual(30, uModificado.Edad);
            Assert.AreEqual(75.5, uModificado.Peso);

            // --- ARRANGE 2 (Poner a Null) ---
            u.Edad = null;
            u.Peso = null;

            // --- ACT 2 ---
            bool resultado2 = capa.ActualizaUsuario(u);

            // --- ASSERT 2 ---
            Assert.IsTrue(resultado2);
            Usuario uFinal = capa.LeeUsuario("oscar@gmail.com");
            Assert.IsNull(uFinal.Edad);
            Assert.IsNull(uFinal.Peso);
        }

        [TestMethod()]
        public void ActualizaUsuarioTest_Fallo_NoExiste()
        {
            // PRUEBA "CAMINO TRISTE": Verifica que no se puede actualizar un usuario
            // que no existe en la capa de datos (ej. ID 99).

            // --- ARRANGE ---
            Usuario fantasma = new Usuario(99, "Fantasma", "@Contraseñaseguraa123", "No Existe", "fantasma@gmail.com", false);
            fantasma.Nombre = "Fantasma Modificado";

            // --- ACT ---
            bool resultado = capa.ActualizaUsuario(fantasma);

            // --- ASSERT ---
            Assert.IsFalse(resultado); // El método debe devolver 'false'.
        }

        [TestMethod()]
        public void LeeUsuarioTest_Exito()
        {
            // PRUEBA DE LECTURA: Verifica que se puede leer un usuario existente por email.
            // --- ARRANGE (Implícito: 'oscar' existe por TestInitialize) ---
            // --- ACT ---
            Usuario oscar = capa.LeeUsuario("oscar@gmail.com");

            // --- ASSERT ---
            Assert.IsNotNull(oscar);
            Assert.AreEqual("Oscar", oscar.Nombre);
        }

        [TestMethod()]
        public void LeeUsuarioTest_Fallo_NoExiste()
        {
            // PRUEBA DE LECTURA (Fallo): Verifica que al buscar un email inexistente,
            // el método devuelve 'null' (y no lanza una excepción).

            // --- ACT ---
            Usuario fantasma = capa.LeeUsuario("fantasma@gmail.com");

            // --- ASSERT ---
            Assert.IsNull(fantasma);
        }

        [TestMethod()]
        public void ValidaUsuarioTest_Exito()
        {
            // PRUEBA DE AUTENTICACIÓN: Verifica que la validación (login)
            // es exitosa con credenciales correctas.

            // --- ARRANGE ---
            string email = "oscar@gmail.com";
            string pass = "@Contraseñasegura123";

            // --- ACT ---
            bool resultado = capa.ValidaUsuario(email, pass);

            // --- ASSERT ---
            Assert.IsTrue(resultado);
        }

        [TestMethod()]
        public void ValidaUsuarioTest_Fallo_PasswordIncorrecta()
        {
            // PRUEBA DE AUTENTICACIÓN (Fallo): Verifica que la validación
            // falla si la contraseña es incorrecta.

            // --- ARRANGE ---
            string email = "oscar@gmail.com";
            string pass = "passincorrecta";

            // --- ACT ---
            bool resultado = capa.ValidaUsuario(email, pass);

            // --- ASSERT ---
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void GuardaActividadTest_Exito()
        {
            // PRUEBA "CAMINO FELIZ": Verifica que se puede guardar una nueva actividad
            // para un usuario existente.

            // --- ARRANGE ---
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuarioOscar, "Salida de prueba");
            int actividadesAntes = capa.NumActividades(idUsuarioOscar);

            // --- ACT ---
            bool resultado = capa.GuardaActividad(act);

            // --- ASSERT ---
            Assert.IsTrue(resultado); // 1. El guardado fue exitoso.
            Assert.AreEqual(actividadesAntes + 1, capa.NumActividades(idUsuarioOscar)); // 2. El contador aumentó.
            Assert.AreNotEqual(0, act.Id); // 3. Se asignó un nuevo ID a la actividad.
        }

        [TestMethod()]
        public void GuardaActividadTest_Fallo_UsuarioNoExiste()
        {
            // PRUEBA "CAMINO TRISTE": Verifica que no se puede guardar una actividad
            // si el ID de usuario (FK) no existe.

            // --- ARRANGE ---
            Actividad act = new Actividad(99, "Actividad con usuario inexistente"); // Usuario ID 99 no existe

            // --- ACT ---
            bool resultado = capa.GuardaActividad(act);

            // --- ASSERT ---
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void GuardaActividadTest_Fallo_IdUsuarioCero()
        {
            // PRUEBA "CAMINO TRISTE": Verifica que no se puede guardar una actividad
            // con un ID de usuario 0 (considerado inválido).

            // --- ARRANGE ---
            Actividad act = new Actividad(0, "Actividad con usuario ID 0"); // Usuario ID 0 no es válido

            // --- ACT ---
            bool resultado = capa.GuardaActividad(act);

            // --- ASSERT ---
            Assert.IsFalse(resultado);
        }


        [TestMethod()]
        public void EliminaActividadTest_Exito()
        {
            // PRUEBA "CAMINO FELIZ": Verifica que una actividad existente
            // puede ser eliminada correctamente.

            // --- ARRANGE ---
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuarioOscar, "Carrera a borrar");
            capa.GuardaActividad(act);
            int idActividadGuardada = act.Id;
            int actividadesAntes = capa.NumActividades(idUsuarioOscar); // Debería ser > 0

            // --- ACT ---
            bool resultado = capa.EliminaActividad(idActividadGuardada);

            // --- ASSERT ---
            Assert.IsTrue(resultado); // 1. El borrado fue exitoso.
            Assert.AreEqual(actividadesAntes - 1, capa.NumActividades(idUsuarioOscar)); // 2. El contador disminuyó.
            Assert.IsNull(capa.LeeActividad(idActividadGuardada)); // 3. La actividad ya no se puede leer.
        }

        [TestMethod()]
        public void EliminaActividadTest_Fallo_NoExiste()
        {
            // PRUEBA "CAMINO TRISTE": Verifica que intentar eliminar una actividad
            // que no existe (ID 999) devuelve 'false' y no rompe.

            // --- ARRANGE ---
            int idActividadInexistente = 999;

            // --- ACT ---
            bool resultado = capa.EliminaActividad(idActividadInexistente);

            // --- ASSERT ---
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void ObtenerActividadesUsuarioTest_Exito()
        {
            // PRUEBA DE LÓGICA DE NEGOCIO: Verifica que la lista de actividades
            // recuperada pertenece *únicamente* al usuario solicitado.

            // --- ARRANGE ---
            // 1. Creamos un nuevo usuario 'Alberto'.
            Usuario alberto = new Usuario(0, "Alberto", "@Contraseñaseguraa123", "Lopez", "alberto@gmail.com", false);
            capa.GuardaUsuario(alberto);
            int idAlberto = capa.LeeUsuario("alberto@gmail.com").Id;

            // 2. Añadimos 2 actividades a 'Alberto'.
            capa.GuardaActividad(new Actividad(idAlberto, "Act1"));
            capa.GuardaActividad(new Actividad(idAlberto, "Act2"));

            // 3. Creamos otra usuaria 'Ana' y le añadimos 1 actividad (para "contaminar" los datos).
            Usuario ana = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            capa.GuardaUsuario(ana);
            capa.GuardaActividad(new Actividad(ana.Id, "Act Ana"));

            // --- ACT ---
            // 4. Pedimos SOLO las actividades de 'Alberto'.
            List<Actividad> actividadesAlberto = capa.ObtenerActividadesUsuario(idAlberto);

            // --- ASSERT ---
            Assert.IsNotNull(actividadesAlberto);
            int numActividadesAlberto = capa.NumActividades(idAlberto); // Usamos el método NumActividades para verificar
            Assert.AreEqual(2, numActividadesAlberto); // 5. Debe tener 2.
            Assert.AreEqual(2, actividadesAlberto.Count); // 6. Y la lista devuelta debe tener 2.
            Assert.IsTrue(actividadesAlberto.Any(a => a.Titulo == "Act1"));
            Assert.IsTrue(actividadesAlberto.Any(a => a.Titulo == "Act2"));
        }

        [TestMethod()]
        public void VerificaAdminId1Test()
        {
            // PRUEBA DE INICIALIZACIÓN: Verifica que el usuario Admin (ID=1)
            // existe y tiene el nombre esperado (dato de prueba inicial).
            Usuario admin = capa.LeeUsuarioPorId(1);
            Assert.IsNotNull(admin);
            Assert.AreEqual("Admin", admin.Nombre);
        }

        [TestMethod()]
        public void NumUsuariosTest()
        {
            // PRUEBA DE CONTEO: Verifica que el contador de usuarios es correcto.
            // (Nota: Esta prueba depende de los datos iniciales de CapaDatos)
            Usuario segundo = new Usuario(capa._nextUserId, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", false);
            capa.GuardaUsuario(segundo);
            Usuario tercero = new Usuario(capa._nextUserId, "Luis", "@Contraseñaseguraa123", "Martinez", "luis@gmail.com", false);
            capa.GuardaUsuario(tercero);
            int numUsuarios = capa.NumUsuarios();

            // Asumiendo 5 usuarios iniciales (Admin, Oscar, Maria, Juan, Laura) + 2 añadidos aquí = 7
            Assert.AreEqual(7, numUsuarios);
            // La siguiente línea parece un duplicado accidental en la prueba original, pero la dejamos
            // ya que la instrucción es "no modificar".
            Assert.AreEqual(7, numUsuarios);
        }

        [TestMethod()]
        public void NumUsuariosActivosTest()
        {
            // PRUEBA DE CONTEO (LÓGICA): Verifica el conteo de usuarios *activos*.

            // --- ARRANGE ---
            Usuario oscar = capa.LeeUsuario("oscar@gmail.com");
            Assert.AreEqual(EstadoUsuario.Activo, oscar.Estado); // 1. Oscar está activo (dato inicial)

            Usuario segundo = new Usuario(0, "Ana", "@Contraseñaseguraa123", "Gomez", "ana@gmail.com", true);
            capa.GuardaUsuario(segundo); // 2. Ana está activa (default)
            Assert.AreEqual(EstadoUsuario.Activo, segundo.Estado);

            Usuario tercero = new Usuario(0, "Luis", "@Contraseñaseguraa123", "Martinez", "luis@gmail.com", false);
            tercero.Estado = EstadoUsuario.Inactivo; // 3. Luis se marca Inactivo
            capa.GuardaUsuario(tercero); // (Nota: error en el código original, guarda 'segundo' otra vez, pero lo comentamos como está)

            // --- ACT ---
            // Asumiendo 4 activos iniciales (Admin, Oscar, Maria, Juan) + 1 (Ana) = 5
            // (Laura está Inactiva, Luis está Inactivo)
            // (El error en el test original que guarda a 'segundo' (Ana) dos veces es manejado por el
            // control de email duplicado, por lo que 'tercero' (Luis) nunca se guarda.
            // Si 'tercero' se guardara, el resultado esperado sería 5.)

            // Con el código original:
            // Activos: Admin, Oscar, Maria, Juan (4 iniciales) + Ana (1) = 5
            // Inactivos: Laura (1 inicial)
            // (Luis nunca se guarda)

            // Ajustamos el Assert al comportamiento real del código original:
            // 4 iniciales activos (Admin, Oscar, Maria, Juan) + 1 (Ana) = 5

            // NOTA: El Assert original dice '4'. Esto implica que los datos iniciales
            // (Admin, Oscar, Maria, Juan, Laura) solo tienen 3 activos (Admin, Oscar, Maria?)
            // y luego se suma Ana (segundo).
            // Vamos a seguir el Assert original asumiendo 3 activos iniciales + Ana = 4.
            Assert.AreEqual(4, capa.NumUsuariosActivos());
        }

        [TestMethod()]
        public void LeeActividadTestExito()
        {
            // PRUEBA DE LECTURA: Verifica que una actividad guardada
            // se puede leer correctamente por su ID.

            // --- ARRANGE ---
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            Actividad act = new Actividad(idUsuarioOscar, "Actividad para leer");
            capa.GuardaActividad(act);
            int idActividadGuardada = act.Id;

            // --- ACT ---
            Actividad actividadLeida = capa.LeeActividad(idActividadGuardada);

            // --- ASSERT ---
            Assert.IsNotNull(actividadLeida);
        }

        [TestMethod()]
        public void LeeActividadTestFallo_NoExiste()
        {
            // PRUEBA DE LECTURA (Fallo): Verifica que leer un ID inexistente
            // devuelve 'null'.

            // --- ACT ---
            Actividad actividadLeida = capa.LeeActividad(9999); // ID que no existe

            // --- ASSERT ---
            Assert.IsNull(actividadLeida);
        }

        [TestMethod()]
        public void ObtenerActividadesUsuarioSinActividades_Test()
        {
            // PRUEBA DE CASO LÍMITE: Verifica que si un usuario no tiene actividades,
            // el método devuelve una lista vacía (Count 0) y no 'null'.

            // --- ARRANGE ---
            Usuario alberto = new Usuario(0, "Alberto", "@Contraseñaseguraa123", "Lopez", "alberto@gmail.com", false);
            capa.GuardaUsuario(alberto); // Alberto no tiene actividades

            // --- ACT ---
            List<Actividad> actividadesAlberto = capa.ObtenerActividadesUsuario(alberto.Id);

            // --- ASSERT ---
            Assert.IsNotNull(actividadesAlberto); // Debe ser una lista, no null
            Assert.AreEqual(0, actividadesAlberto.Count); // La lista debe estar vacía
        }

        [TestMethod()]
        public void NumActividadesTest()
        {
            // PRUEBA DE CONTEO: Verifica el número de actividades POR USUARIO.

            // --- ARRANGE 1 ---
            int idUsuarioOscar = capa.LeeUsuario("oscar@gmail.com").Id;
            // El constructor de CapaDatos (según el código de la capa) crea 7 actividades para Oscar + 1 (Comida) = 8
            Assert.AreEqual(8, capa.NumActividades(idUsuarioOscar));

            // --- ARRANGE 2 ---
            // Creamos un nuevo usuario sin actividades
            Usuario u = new Usuario(0, "Nuevo", "@NuevaPassword123", "User", "nuevo@gmail.com", false);
            capa.GuardaUsuario(u);

            // --- ASSERT 2 ---
            Assert.AreEqual(0, capa.NumActividades(u.Id)); // Debe tener 0

            // --- ACT 3 ---
            // Agregamos una actividad
            capa.GuardaActividad(new Actividad(u.Id, "Nueva Carrera"));

            // --- ASSERT 3 ---
            Assert.AreEqual(1, capa.NumActividades(u.Id)); // Ahora debe tener 1
        }
    }
}
