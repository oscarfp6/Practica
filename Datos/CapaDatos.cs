using Datos.Database;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class CapaDatos : ICapaDatos
    {
        private static List<Usuario> tblUsuarios;
        private static List<Actividad> tblActividades;

        private static int _nextUserId = 1;
        private static int _nextActividadId = 1;


        public void Inicializa()
        {

           
            if (tblUsuarios == null) 
            {
                tblUsuarios = new List<Usuario>();
                tblActividades = new List<Actividad>();
                _nextUserId = 0;
                _nextActividadId = 0;
            }
            
        }


        bool ICapaDatos.GuardaActividad(Actividad e)
        {
            e.Id = _nextActividadId++;

            tblActividades.Add(e);
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

        bool ICapaDatos.GuardaUsuario(Usuario u)
        {
            if(!tblUsuarios.Any(user => user.Email == u.Email))
            {
                u.Id = _nextUserId++;

                tblUsuarios.Add(u);
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

        Actividad ICapaDatos.LeeActividad(int idElemento)
        {
            return tblActividades.FirstOrDefault(act => act.Id == idElemento);
        }

        List<Actividad> ICapaDatos.LeeActividades(int idUsuario)
        {
            return tblActividades.Where(act => act.IdUsuario == idUsuario).ToList();
        }

        int ICapaDatos.NumActividades(int idUsuario)
        {
            return tblActividades.Count(act => act.IdUsuario == idUsuario);
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
            throw new NotImplementedException();
        }
    }
}
