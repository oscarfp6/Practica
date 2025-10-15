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
        private static List<Usuario> tblUsuarios;
        private static List<Actividad> tblActividades;

        private static int _nextUserId = 1;
        private static int _nextActividadId = 1;


        static CapaDatos()
        {
            tblUsuarios = new List<Usuario>();
            tblActividades = new List<Actividad>();
            Usuario u = new Usuario(1, "Oscar", "@Contraseñaseguraa123","Fuentes Paniego", "oscar@gmail.com", true);
            
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

        public bool GuardaActividad (Actividad actividad)
        {
            var existente = tblActividades.FirstOrDefault(a => a.Id == actividad.Id);
            if (existente != null)
            {
                // Si encontramos una actividad con el mismo ID, no la añadimos y devolvemos false.
                return false;
            }
            // Asignamos un nuevo ID a la actividad y lo incrementamos para la siguiente.
            actividad.Id = _nextActividadId++;
            // Añadimos la actividad a nuestra "tabla".
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
            existente.MetrosDesnivel = actividad.MetrosDesnivel;
            existente.Tipo = actividad.Tipo;
            existente.FCMedia = actividad.FCMedia;
            existente.Duracion = actividad.Duracion;
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
