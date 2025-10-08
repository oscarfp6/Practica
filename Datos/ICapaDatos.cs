using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiLogica.ModeloDatos;



namespace Datos
{

    namespace Database
    {
        internal interface ICapaDatos
        {
            /// Este Interfaz se entrga a modo de requisitos mínimos a implementar y probar.
            /// Debéis de incluir funcionalidades adicionales

            /// <summary>
            /// RF01: Almacena el usuario. También se puede usar para actualizar si ya tiene ID.
            /// </summary>
            /// <param name="u">Objeto de la clase Usuario que se desea almacenar.</param>
            /// <returns>True o False en función de si ha conseguido insertar/actualizar la información.</returns>
            bool GuardaUsuario(Usuario u);

            /// <summary>
            /// RF04, RF05, RF06, RF07, RF08: Actualiza los campos mutables del usuario. (nombre, apellidos, estado, hash, etc.).
            /// </summary>
            /// <param name="u">Objeto de la clase Usuario que se desea actualizar.</param>
            /// <returns>True o False en función de si ha conseguido actualizar la información</returns>
            bool ActualizaUsuario(Usuario u);


            /// <summary>
            /// RF02: Lee los datos del usuario que se corresponde con la clave Email que se recibe como parámetro.
            /// </summary>
            /// <param name="email">Cadena con el EMail del usuario que se quiere consultar.</param>
            /// <returns>Retorna el objeto con la infromación del usuario buscado o NULL si no se localiza.</returns>
            Usuario LeeUsuario(String email);

            /// <summary>
            /// RF02: Lee los datos del usuario que se corresponde con la clave ID que se recibe como parámetro.
            /// </summary>
            /// <param name="idUsuario">Identificador del Usuario cuyos datos se quieren consultar.</param>
            /// <returns>Retorna el objeto con la infromación del usuario buscado o NULL si no se localiza.</returns>
            Usuario LeeUsuarioPorId(int idUsuario);

            /// <summary>
            /// Comprueba si el usuario existe existe y el password se corresponde con la almacenada de forma cifrada.
            /// </summary>
            /// <param name="email">Cadena con el EMail del usuario que se quiere consultar.</param>
            /// <param name="password">Cadena con el EMail del usuario que se quiere consultar.</param>
            /// <returns>Retorna TRUE si los datos de autenticación son válidos.</returns>
            bool ValidaUsuario(string email, string password);

            /// <summary>
            /// Retorna el número de usuarios registrados.
            /// </summary>
            /// <returns>Número de Usuarios.</returns>
            int NumUsuarios();

            /// <summary>
            /// OPCIONAL
            /// Retorna el número de usuarios registrados.
            /// </summary>
            /// <returns>Número de Usuarios.</returns>
            int NumUsuariosActivos();

            /// <summary>
            /// RF09: Almacena una nueva Actividad
            /// </summary>
            /// <param name="e">Objeto de la clase Actividad que se quiere almacenar.</param>
            /// <returns>Trueo o False en función de si ha conseguido insertar/ actualizar la información.</returns>
            bool GuardaActividad(Actividad e);

            /// <summary>
            /// RF11: Actualiza una Actividad existente
            /// </summary>
            /// <param name="e">Objeto de la clase Actividad que se quiere actualizar.</param>
            /// <returns>Trueo o False en función de si ha conseguido actualizar la información.</returns>
            bool ActualizaActividad(Actividad e);

            /// <summary>
            /// RF12: Elimina una Actividad por su ID.
            /// </summary>
            /// <param name="idElemento">Identificador del Actividad que se quiere eliminar.</param>
            /// <returns>True o False en función de si ha conseguido eliminar la información.</returns>
            bool EliminaActividad(int idElemento);

            /// <summary>
            /// Lee los datos del elemento referenciado por su ID.
            /// </summary>
            /// <param name="idElemento">Identificador del Actividad que se quiere consultar.</param>
            /// <returns>Retorna el objeto con la infromación del conponente buscado o NULL si no se localiza.</returns>
            Actividad LeeActividad(int idElemento);

            List<Actividad>  LeeActividades(int idUsuario);

            /// <summary>
            /// RF13: Obtiene el listado completo de actividades de un usuario.
            /// </summary>
            /// <param name="idUsuario">Identificador del Usuario cuyos datos se quieren consultar.</param>
            /// <returns>Retorna una lista con las actividades del Usuario</returns>
            List<Actividad> ObtenerActividadesUsuario(int idUsuario);

            /// <summary>
            /// Retorna el número de Actividades registrados.
            /// </summary>
            /// <param name="idUsuario">Identificador del Usuario cuyos datos se quieren consultar.</param>
            /// <returns>Número de Actividades.</returns>
            int NumActividades(int idUsuario);

        }
    }
}
