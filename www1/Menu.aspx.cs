// Usings necesarios para la funcionalidad de la página.
using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq; // Necesario para OrderByDescending
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    // Clase code-behind para el formulario del Menú principal (WebForm2).
    public partial class WebForm2 : System.Web.UI.Page
    {
        // Campos privados para la conexión a la base de datos y la instancia del usuario.
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;


        /// <summary>
        /// Se ejecuta al cargar la página. Inicializa la conexión y autentica la sesión.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Ocultar mensajes de estado iniciales.
            lblNingunaActividad.Visible = false;
            lblMenuMessage.Visible = false;

            // --- Recuperar Conexión y Usuario ---
            // Recupera la conexión de Application State o redirige si falla.
            if (Application["conexionDB"] is CapaDatos capa) { conexionDB = capa; }
            else { /* Manejar error crítico si es necesario */ Response.Redirect("Login.aspx"); return; }

            // Recupera el usuario de Session State o redirige si no está autenticado.
            if (Session["usuarioautenticado"] is Usuario user) { usuarioAutenticado = user; }
            else { Response.Redirect("Login.aspx"); return; }
            // --- Fin Recuperación ---

            // Lógica de inicialización ejecutada solo en la primera carga (IsPostBack = false).
            if (!IsPostBack)
            {
                // Muestra el nombre completo del usuario en la UI.
                lblNombreApellidos.Text = $"{usuarioAutenticado.Nombre} {usuarioAutenticado.Apellidos}";
                // Carga inicial de la lista de actividades.
                CargarActividades();
            }

        }

        /// <summary>
        /// Obtiene las actividades del usuario desde la base de datos, las ordena y enlaza al Repeater.
        /// </summary>
        private void CargarActividades()
        {
            // Llama a la Capa de Datos para obtener la lista de actividades del usuario actual.
            List<Actividad> actividades = conexionDB.ObtenerActividadesUsuario(usuarioAutenticado.Id);

            // Lógica de validación de estado: si no hay actividades.
            if (actividades.Count == 0)
            {
                lblNingunaActividad.Visible = true;
                rptActividades.DataSource = null; // Limpiar datasource
                rptActividades.DataBind(); // Enlazar vacío
                rptActividades.Visible = false; // Ocultar el control Repeater
                return;
            }

            // Ocultar el mensaje "ninguna actividad" si hay datos.
            lblNingunaActividad.Visible = false;
            rptActividades.Visible = true;

            // Ordena las actividades por fecha descendente (la más reciente primero).
            var actividadesOrdenadas = actividades.OrderByDescending(a => a.Fecha).ToList();

            // Enlaza la lista ordenada al control Repeater para mostrarla en la UI.
            rptActividades.DataSource = actividadesOrdenadas;
            rptActividades.DataBind();
        }


        /// <summary>
        /// Manejador de eventos para comandos (clics en botones) dentro del Repeater.
        /// Controla las acciones de edición y eliminación.
        /// </summary>
        protected void RptActividades_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            lblMenuMessage.Visible = false; // Oculta cualquier mensaje previo.

            // 1. Intenta obtener el ID de la actividad desde el CommandArgument.
            if (!int.TryParse(e.CommandArgument?.ToString(), out int actividadId))
            {
                MostrarMensaje("Error: ID de actividad inválido.", "error");
                return;
            }

            // --- Lógica para EDITAR (CommandName == "EditActivity") ---
            if (e.CommandName == "EditActivity")
            {
                // 2. Validación de Regla de Negocio: Verificar si el usuario tiene suscripción.
                if (usuarioAutenticado.Suscripcion)
                {
                    // Éxito: Redirige a la página de edición, pasando el ID como parámetro de URL.
                    // Esto permite a la página de destino cargar la actividad correcta.
                    Response.Redirect($"EditarActividad.aspx?id={actividadId}");
                }
                else
                {
                    // Fallo: Muestra un mensaje de advertencia.
                    MostrarMensaje("Necesitas una suscripción activa para editar actividades.", "warning");
                }
            }
            // --- Lógica para ELIMINAR (CommandName == "DeleteActivity") ---
            else if (e.CommandName == "DeleteActivity")
            {
                try
                {
                    // 3. Llama a la Capa de Datos para intentar la eliminación.
                    bool eliminado = conexionDB.EliminaActividad(actividadId);

                    if (eliminado)
                    {
                        MostrarMensaje("Actividad eliminada correctamente.", "success");
                        CargarActividades(); // Vuelve a cargar la lista para reflejar el borrado (Validación visual).
                    }
                    else
                    {
                        // Fallo en la Capa de Datos.
                        MostrarMensaje("Error: No se pudo encontrar o eliminar la actividad.", "error");
                    }
                }
                catch (Exception ex)
                {
                    // Captura de errores inesperados (ej. problemas de conexión a la BD).
                    // Loggear el error 'ex' en un sistema real
                    MostrarMensaje("Error inesperado al intentar eliminar la actividad.", "error");
                }
            }
        }

        // --- Función auxiliar para mostrar mensajes ---
        /// <summary>
        /// Muestra un mensaje de estado usando el label de la página, aplicando la clase CSS según el tipo.
        /// </summary>
        private void MostrarMensaje(string mensaje, string tipo) // tipo = "success", "error", "warning"
        {
            lblMenuMessage.Text = mensaje;
            // Asigna la clase CSS dinámicamente para el estilo (ej. message-error).
            lblMenuMessage.CssClass = $"menu-message message-{tipo}";
            lblMenuMessage.Visible = true;
        }


        // --- Otros Handlers (LogOut, Perfil, RegistrarActividad) ---
        protected void BtnLogOut_Click(object sender, EventArgs e)
        {
            // Cierra la sesión y redirige al login (Prueba de Finalización de Sesión).
            Session.Clear(); Session.Abandon(); Response.Redirect("Login.aspx");
        }

        protected void BtnPerfil_Click(object sender, EventArgs e)
        {
            // Redirige a la página de Perfil.
            Response.Redirect("Perfil.aspx");
        }

        protected void BtnRegistrarActividad_Click(object sender, EventArgs e)
        {
            // Redirige a la página de registro de actividades.
            Response.Redirect("RegistrarActividad.aspx");
        }

        // Esta función será llamada desde el Repeater en el archivo .aspx
        /// <summary>
        /// Devuelve el texto formateado para el ritmo o la velocidad, según el tipo de actividad.
        /// Esta es una función de Formato y Presentación (Capa de Vista).
        /// </summary>
        protected string FormatearRitmo(object tipoActividad, object velocidad, object ritmo)
        {
            try
            {
                // 1. Convertimos los objetos 'evaluados' a sus tipos correctos.
                TipoActividad tipo = (TipoActividad)tipoActividad;
                double velKmh = (double)velocidad;
                double ritMinKm = (double)ritmo;

                // 2. Aplica la lógica de presentación basada en el tipo de actividad.
                switch (tipo)
                {
                    case TipoActividad.Ciclismo:
                        // Si es Ciclismo, muestra la velocidad en km/h.
                        return (velKmh > 0) ? $"{velKmh:N2} km/h" : "N/A";

                    case TipoActividad.Running:
                    case TipoActividad.Caminata:
                        // Si es Running o Caminata, muestra el ritmo en min/km.
                        return (ritMinKm > 0) ? $"{ritMinKm:N2} min/km" : "N/A";

                    case TipoActividad.Natacion:
                    case TipoActividad.Gimnasio:
                    case TipoActividad.Otro:
                    default:
                        // Para los demás tipos, no se muestra dato de ritmo/velocidad.
                        return string.Empty;
                }
            }
            catch
            {
                // En caso de cualquier error (ej. error de casting o datos nulos), devuelve vacío.
                return string.Empty;
            }
        }
    }
}
