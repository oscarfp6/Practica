using Datos.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using MiLogica.ModeloDatos;
using System.Runtime.CompilerServices;

namespace Datos
{
    /// <summary>
    /// Implementación de la capa de acceso a datos en memoria (mock).
    /// Simula una base de datos para facilitar el desarrollo y las pruebas unitarias.
    /// </summary>
    public class CapaDatos : ICapaDatos
    {
        // Almacenes de datos en memoria, simulando tablas de base de datos.
        private List<Usuario> tblUsuarios;
        private List<Actividad> tblActividades;

        // Contadores para simular claves primarias autoincrementales.
        public int _nextUserId = 1;
        private int _nextActividadId = 1;


        /// <summary>
        /// Inicializa la capa de datos y carga un conjunto de datos de prueba (seeding)
        /// para tener usuarios y actividades disponibles desde el arranque.
        /// </summary>
        public CapaDatos()
        {
            tblUsuarios = new List<Usuario>();
            tblActividades = new List<Actividad>();

            // --- Carga de Datos de Prueba (Seeding) ---
            Usuario admin = new Usuario(_nextUserId, "Admin", "@AdminPassword1234", "Admin Apellidos", "admin@gmail.com", true);
            GuardaUsuario(admin);
            Usuario u = new Usuario(_nextUserId, "Oscar", "@Contraseñasegura123", "Fuentes Paniego", "oscar@gmail.com", true);
            GuardaUsuario(u);
            Usuario usuarioBloqueado = new Usuario(_nextUserId, "Bloqueado", "@BloqueadoPassword123", "Usuario Bloqueado", "bloqueado@gmail.com", false);
            usuarioBloqueado.Estado = EstadoUsuario.Bloqueado;
            GuardaUsuario(usuarioBloqueado);
            Usuario uPrueba = new Usuario(_nextUserId, "Prueba", "@PruebaPassword123", "Usuario", "prueba@gmail.com", false);
            GuardaUsuario(uPrueba);
            Usuario segundoBloqueado = new Usuario(_nextUserId, "SegundoBloqueado", "@SegundoBloqueado123", "Segundo Bloqueado ", "segundobloqueado@gmail.com", true);
            segundoBloqueado.Estado = EstadoUsuario.Bloqueado;
            GuardaUsuario(segundoBloqueado);

            Actividad a1 = new Actividad(u.Id, "Ruta por la montaña", 15.5, 800, TimeSpan.FromHours(1.5), DateTime.Now, TipoActividad.Ciclismo, "Una ruta espectacular por las montañas.", 130);
            GuardaActividad(a1);
            Actividad incompleta = new Actividad(u.Id, "Actividad Incompleta");
            GuardaActividad(incompleta);
            Actividad a2 = new Actividad(u.Id, "Carrera urbana", 10.0, 100, TimeSpan.FromHours(0.8), DateTime.Now, TipoActividad.Running, "Carrera rápida por la ciudad.", 160);
            GuardaActividad(a2);
            Actividad a3 = new Actividad(u.Id, "Caminata relajada", 5.0, 50, TimeSpan.FromHours(1.0), DateTime.Now, TipoActividad.Caminata, "Paseo tranquilo por el parque.", 110);
            GuardaActividad(a3);
            Actividad a4 = new Actividad(u.Id, "Entrenamiento de intervalos", 8.0, 200, TimeSpan.FromHours(1.2), DateTime.Now, TipoActividad.Running, "Sesión intensa de intervalos en la pista.", 170);
            GuardaActividad(a4);
            Actividad a5 = new Actividad(u.Id, "Ruta en bicicleta de montaña", 20.0, 600, TimeSpan.FromHours(2.0), DateTime.Now, TipoActividad.Ciclismo, "Desafiante ruta de montaña con vistas increíbles.", 140);
            GuardaActividad(a5);
            Actividad a6 = new Actividad(u.Id, "Caminata por la naturaleza", 12.0, 300, TimeSpan.FromHours(2.5), DateTime.Now, TipoActividad.Caminata, "Explorando senderos naturales y disfrutando del paisaje.", 120);
            GuardaActividad(a6);
            Actividad a7 = new Actividad(u.Id, "Maratón de la ciudad", 42.195, 500, TimeSpan.FromHours(4.0), DateTime.Now, TipoActividad.Running, "Participación en el maratón anual de la ciudad.", 155);
            GuardaActividad(a7);

        }

        /// <summary>
        /// Guarda un nuevo usuario en la colección, si el email no existe previamente.
        /// </summary>
        /// <param name="usuario">El objeto Usuario a guardar.</param>
        /// <returns>True si el usuario fue guardado; False si el email ya existía.</returns>
        public bool GuardaUsuario(Usuario usuario)
        {
            // Validación de regla de negocio: El email debe ser único (ignorando mayúsculas).
            var existente = tblUsuarios.FirstOrDefault(u => u.Email.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase));

            if (existente != null)
            {
                // El email ya está en uso, se rechaza el guardado.
                return false;
            }

            // Asignación de ID simulando autoincremento.
            usuario.Id = _nextUserId++;

            // Persistencia en memoria.
            tblUsuarios.Add(usuario);

            return true;
        }


        /// <summary>
        /// Actualiza un usuario existente basado en su ID.
        /// </summary>
        /// <param name="usuario">El objeto Usuario con los datos actualizados.</param>
        /// <returns>True si el usuario fue encontrado y actualizado; False si el ID no se encontró.</returns>
        public bool ActualizaUsuario(Usuario usuario)
        {
            // Búsqueda del registro a actualizar.
            var existente = tblUsuarios.FirstOrDefault(u => u.Id == usuario.Id);
            if (existente == null)
            {
                // El usuario no existe, no se puede actualizar.
                return false;
            }

            // Actualización de los campos (mapeo).
            existente.Nombre = usuario.Nombre;
            existente.Apellidos = usuario.Apellidos;
            existente.Estado = usuario.Estado;
            existente._passwordHash = usuario._passwordHash;
            existente.Suscripcion = usuario.Suscripcion;
            existente.Peso = usuario.Peso;
            existente.Edad = usuario.Edad;
            return true;
        }

        /// <summary>
        /// Busca un usuario por su email, ignorando mayúsculas y minúsculas.
        /// </summary>
        /// <param name="email">El email a buscar.</param>
        /// <returns>El objeto Usuario si se encuentra; null si no.</returns>
        public Usuario LeeUsuario(string email)
        {
            Usuario usuario = null;
            // La comparación OrdinalIgnoreCase es una regla de negocio clave.
            usuario = tblUsuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return usuario;
        }

        /// <summary>
        /// Busca un usuario por su ID único.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario.</param>
        /// <returns>El objeto Usuario si se encuentra; null si no.</returns>
        public Usuario LeeUsuarioPorId(int idUsuario)
        {
            return tblUsuarios.FirstOrDefault(u => u.Id == idUsuario);
        }

        /// <summary>
        /// Valida las credenciales de un usuario.
        /// </summary>
        /// <param name="email">Email del usuario.</param>
        /// <param name="password">Contraseña a verificar.</param>
        /// <returns>True si el email y contraseña coinciden; False en caso contrario.</returns>
        public bool ValidaUsuario(string email, string password)
        {
            var usuario = LeeUsuario(email);
            if (usuario == null) return false;

            // Delega la comprobación de la contraseña al modelo de Usuario.
            return usuario.ComprobarPassWord(password);
        }

        /// <summary>
        /// Obtiene el número total de usuarios registrados.
        /// </summary>
        public int NumUsuarios()
        {
            return tblUsuarios.Count;
        }

        /// <summary>
        /// Obtiene el número de usuarios cuyo estado es 'Activo'.
        /// </summary>
        public int NumUsuariosActivos()
        {
            return tblUsuarios.Count(u => u.Estado == EstadoUsuario.Activo);
        }

        /// <summary>
        /// Obtiene una copia de la lista de todos los usuarios.
        /// </summary>
        /// <returns>Una nueva lista de usuarios. Modificar esta lista no afectará al almacén de datos.</returns>
        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            // Se devuelve una nueva lista (copia) para proteger la encapsulación.
            // Esto evita que código externo modifique la lista original (tblUsuarios).
            return tblUsuarios.ToList();
        }

        /// <summary>
        /// Guarda una nueva actividad, asignándole un ID.
        /// </summary>
        /// <param name="actividad">La actividad a guardar.</param>
        /// <returns>True si se guardó; False si el usuario asociado no existe o si la actividad ya tiene un ID (indicando que es una actualización).</returns>
        public bool GuardaActividad(Actividad actividad)
        {
            // 1. Validación de clave foránea: El usuario debe existir.
            var usuario = tblUsuarios.FirstOrDefault(u => u.Id == actividad.IdUsuario);
            if (usuario == null)
            {
                return false; // El usuario al que pertenece la actividad no existe
            }

            // 2. Validación de inserción: No se debe guardar una actividad que ya tenga ID.
            if (actividad.Id != 0)
            {
                // Este método es solo para actividades nuevas (Id=0).
                // Para actualizar, se debe usar ActualizaActividad.
                return false;
            }

            // 3. Asignar nuevo ID y guardar
            actividad.Id = _nextActividadId++;
            tblActividades.Add(actividad);
            return true;
        }

        /// <summary>
        /// Actualiza una actividad existente basada en su ID.
        /// </summary>
        /// <param name="actividad">El objeto Actividad con los datos actualizados.</param>
        /// <returns>True si se actualizó; False si no se encontró el ID.</returns>
        public bool ActualizaActividad(Actividad actividad)
        {
            var existente = tblActividades.FirstOrDefault(a => a.Id == actividad.Id);
            if (existente == null)
            {
                // La actividad no existe.
                return false;
            }

            // Mapeo de campos para la actualización.
            existente.Titulo = actividad.Titulo;
            existente.Descripcion = actividad.Descripcion;
            existente.Kms = actividad.Kms;
            existente.Tipo = actividad.Tipo;
            existente.Fecha = actividad.Fecha;
            existente.MetrosDesnivel = actividad.MetrosDesnivel;
            existente.Duracion = actividad.Duracion;
            existente.FCMedia = actividad.FCMedia;
            return true;
        }

        /// <summary>
        /// Elimina una actividad de la colección.
        /// </summary>
        /// <param name="idElemento">El ID de la actividad a eliminar.</param>
        /// <returns>True si se eliminó; False si no se encontró el ID.</returns>
        public bool EliminaActividad(int idElemento)
        {
            var existente = tblActividades.FirstOrDefault(a => a.Id == idElemento);
            if (existente == null)
            {
                // No se puede eliminar algo que no existe.
                return false;
            }

            tblActividades.Remove(existente);
            return true;
        }

        /// <summary>
        /// Lee una actividad por su ID único.
        /// </summary>
        /// <param name="idElemento">El ID de la actividad.</param>
        /// <returns>El objeto Actividad si se encuentra; null si no.</returns>
        public Actividad LeeActividad(int idElemento)
        {
            return tblActividades.FirstOrDefault(a => a.Id == idElemento);
        }

        /// <summary>
        /// Obtiene todas las actividades registradas para un usuario específico.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario.</param>
        /// <returns>Una lista de actividades; devuelve una lista vacía si el usuario no tiene actividades.</returns>
        public List<Actividad> ObtenerActividadesUsuario(int idUsuario)
        {
            return tblActividades.Where(a => a.IdUsuario == idUsuario).ToList();
        }

        /// <summary>
        /// Devuelve el número total de actividades para un usuario específico.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario.</param>
        public int NumActividades(int idUsuario)
        {
            return tblActividades.Count(a => a.IdUsuario == idUsuario);
        }
    }
}
