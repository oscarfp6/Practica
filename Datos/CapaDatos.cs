using Datos.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using MiLogica.ModeloDatos;
using System.Runtime.CompilerServices;

namespace Datos
{
    public class CapaDatos : ICapaDatos
    {
        private List<Usuario> tblUsuarios;
        private List<Actividad> tblActividades;
        public int _nextUserId = 1;
        private int _nextActividadId = 1;



        public CapaDatos()
        {

            tblUsuarios = new List<Usuario>();
            tblActividades = new List<Actividad>();
            Usuario admin = new Usuario(_nextUserId, "Admin", "@AdminPassword1234", "Admin Apellidos", "admin@gmail.com", true);
            GuardaUsuario(admin);
            Usuario u = new Usuario(_nextUserId, "Oscar", "@Contraseñasegura123", "Fuentes Paniego", "oscar@gmail.com", true);
            GuardaUsuario(u);
            Usuario usuarioInactivo = new Usuario(_nextUserId, "Inactivo", "@InactivoPassword1234", "Usuario Inactivo", "inactivo@gmail.com", false);
            usuarioInactivo.Estado = EstadoUsuario.Inactivo;
            GuardaUsuario(usuarioInactivo);
            
            Actividad a1 = new Actividad(u.Id, "Ruta por la montaña", 15.5,  800, TimeSpan.FromHours(1.5),DateTime.Now, TipoActividad.Ciclismo, "Una ruta espectacular por las montañas.", 130);
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

        public bool GuardaUsuario(Usuario usuario)
        {
            // Verificamos si ya existe un usuario con el mismo email.
            var existente = tblUsuarios.FirstOrDefault(u => u.Email.Equals(usuario.Email, StringComparison.OrdinalIgnoreCase));

            if (existente != null)
            {
                // Si encontramos un usuario, no lo añadimos y devolvemos false.
                return false;
            }

            // Asignamos un nuevo ID al usuario y lo incrementamos para el siguiente.
            usuario.Id = _nextUserId++;

            // Añadimos el usuario a nuestra "tabla".
            tblUsuarios.Add(usuario);

            return true;
        }



        public bool ActualizaUsuario(Usuario usuario)
        {
            var existente = tblUsuarios.FirstOrDefault(u => u.Id == usuario.Id);
            if (existente == null)
            {
                // Si no encontramos el usuario, devolvemos false.
                return false;
            }
            // Actualizamos los campos del usuario existente.
            existente.Nombre = usuario.Nombre;
            existente.Apellidos = usuario.Apellidos;
            existente.Estado = usuario.Estado;
            existente._passwordHash = usuario._passwordHash;
            existente.Suscripcion = usuario.Suscripcion;
            existente.Peso = usuario.Peso;
            existente.Edad = usuario.Edad;
            return true;
        }

        public Usuario LeeUsuario(string email)
        {
            Usuario usuario = null;
            usuario = tblUsuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return usuario;
        }

        public Usuario LeeUsuarioPorId(int idUsuario)
        {
            return tblUsuarios.FirstOrDefault(u => u.Id == idUsuario);
        }

        public bool ValidaUsuario(string email, string password)
        {
            var usuario = LeeUsuario(email);
            if (usuario == null) return false;
            return usuario.ComprobarPassWord(password);


        }

        public int NumUsuarios()
        {
            return tblUsuarios.Count;
        }

        public int NumUsuariosActivos()
        {
            return tblUsuarios.Count(u => u.Estado == EstadoUsuario.Activo);
        }


        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            // Devolvemos una nueva lista para que el original (tblUsuarios) no se modifique desde fuera
            return tblUsuarios.ToList();
        }

        public bool GuardaActividad(Actividad actividad)
        {
            // 1. Validar que el IdUsuario exista
            var usuario = tblUsuarios.FirstOrDefault(u => u.Id == actividad.IdUsuario);
            if (usuario == null)
            {
                return false; // El usuario al que pertenece la actividad no existe
            }

            // 2. Validar que no sea una actividad duplicada (si ya tiene un ID)
            // (Esta lógica asume que 0 es una actividad nueva)
            if (actividad.Id != 0)
            {
                var existente = tblActividades.FirstOrDefault(a => a.Id == actividad.Id);
                if (existente != null)
                {
                    // Ya existe un registro con ese ID, usar ActualizaActividad en su lugar
                    return false;
                }
            }

            // 3. Asignar nuevo ID y guardar
            actividad.Id = _nextActividadId++;
            tblActividades.Add(actividad);
            return true;
        }

        public bool ActualizaActividad(Actividad actividad)
        {
            var existente = tblActividades.FirstOrDefault(a => a.Id ==actividad.Id);
            if(existente == null)
            {
                // Si no encontramos la actividad, devolvemos false.
                return false;
            }
            // Actualizamos los campos de la actividad existente.
            existente.Titulo = actividad.Titulo;
            existente.Descripcion = actividad.Descripcion;
            existente.Kms = actividad.Kms;
            existente.Tipo = actividad.Tipo;
            existente.Fecha = actividad.Fecha;
            return true;
        }

        public bool EliminaActividad(int idElemento)
        {
            var existente = tblActividades.FirstOrDefault(a => a.Id == idElemento);
            if (existente == null)
            {
                // Si no encontramos la actividad, devolvemos false.
                return false;
            }
            // Eliminamos la actividad de nuestra "tabla".
            tblActividades.Remove(existente);
            return true;
        }

        public Actividad LeeActividad (int idElemento)
        {
            return tblActividades.FirstOrDefault(a => a.Id == idElemento);
        }

        public List<Actividad> ObtenerActividadesUsuario (int idUsuario)
        {
            //Si el usuario no tiene actividades, devuelve una lista vacía
            return tblActividades.Where(a => a.IdUsuario == idUsuario).ToList();
        }

        public int NumActividades (int idUsuario)
        {
            return tblActividades.Count(a => a.IdUsuario == idUsuario);
        }


        /*



        bool ICapaDatos.GuardaActividad(Actividad e)
        {
            if (e.Id > 0 && tblActividades.Any(act => act.Id == e.Id))
            {
                return ((ICapaDatos)this).ActualizaActividad(e);
            }

            // 2. Caso de Inserción: Actividad nueva (o ID inválido)
            e.AsignarIdParaPersistencia(_nextActividadId++); tblActividades.Add(e);
            return true;
        }

        bool ICapaDatos.ActualizaActividad(Actividad e)
        {
            var actividadExistente = tblActividades.FirstOrDefault(act => act.Id == e.Id);
            if (actividadExistente != null)
            {
                actividadExistente.Titulo = e.Titulo;
                actividadExistente.Descripcion = e.Descripcion;
                actividadExistente.Kms = e.Kms;
                actividadExistente.MetrosDesnivel = e.MetrosDesnivel;
                actividadExistente.Duracion = e.Duracion;
                actividadExistente.Tipo = e.Tipo;
                actividadExistente.FCMedia = e.FCMedia;
                return true;
            }
            return false;
        }

        bool ICapaDatos.EliminaActividad(int idElemento)
        {
            var actividadExistente = tblActividades.FirstOrDefault(act => act.Id == idElemento);
            if (actividadExistente != null)
            {
                tblActividades.Remove(actividadExistente);
                return true;
            }
            return false;
        }



        bool ICapaDatos.ActualizaUsuario(Usuario u)
        {
            var usuarioExistente = tblUsuarios.FirstOrDefault(user => user.Id == u.Id);
            if (usuarioExistente != null)
            {
                usuarioExistente.Nombre = u.Nombre;
                usuarioExistente.Apellidos = u.Apellidos;
                usuarioExistente.Estado = u.Estado;
                usuarioExistente._passwordHash = u._passwordHash;
                usuarioExistente.Suscripcion = u.Suscripcion;
                return true;
            }
            return false;
        }

        Usuario ICapaDatos.LeeUsuario(string email)
        {
            return tblUsuarios.FirstOrDefault(user => user.Email == email);
        }

        Usuario ICapaDatos.LeeUsuarioPorId(int idUsuario)
        {
            return tblUsuarios.FirstOrDefault(user => user.Id == idUsuario);
        }





        int ICapaDatos.NumUsuarios()
        {
            return tblUsuarios.Count;
        }

        int ICapaDatos.NumUsuariosActivos()
        {
            return tblUsuarios.Count(user => user.Estado == EstadoUsuario.Activo);
        }

        bool ICapaDatos.ValidaUsuario(string email, string password)
        {
            var usuario = ((ICapaDatos)this).LeeUsuario(email);
            if (usuario == null) return false;
            return usuario.ComprobarPassWord(password);
        }

        public List<Actividad> ObtenerActividadesUsuario(int idUsuario)
        {
            return tblActividades.Where(act => act.IdUsuario == idUsuario).ToList();
        }

        */


    }
}
