using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;


        protected void Page_Load(object sender, EventArgs e)
        {
            // Ocultar mensajes al cargar
            lblNingunaActividad.Visible = false;
            lblMenuMessage.Visible = false; // Ocultar mensaje general

            // --- Recuperar Conexión y Usuario (sin cambios) ---
            if (Application["conexionDB"] is CapaDatos capa) { conexionDB = capa; }
            else { /* Manejar error crítico si es necesario */ Response.Redirect("Login.aspx"); return; }
            if (Session["usuarioautenticado"] is Usuario user) { usuarioAutenticado = user; }
            else { Response.Redirect("Login.aspx"); return; }
            // --- Fin Recuperación ---

            if (!IsPostBack)
            {
                lblNombreApellidos.Text = $"{usuarioAutenticado.Nombre} {usuarioAutenticado.Apellidos}";
                CargarActividades();
            }

        }

        private void CargarActividades()
        {
            // ... (Código existente sin cambios) ...
            List<Actividad> actividades = conexionDB.ObtenerActividadesUsuario(usuarioAutenticado.Id);
            if (actividades.Count == 0)
            {
                lblNingunaActividad.Visible = true;
                rptActividades.DataSource = null; // Limpiar datasource
                rptActividades.DataBind(); // Enlazar vacío para que no muestre nada
                rptActividades.Visible = false; // Ocultar repeater
                return;
            }

            lblNingunaActividad.Visible = false; // Asegurar que no se muestre si hay actividades
            rptActividades.Visible = true; // Asegurar que se muestre

            var actividadesOrdenadas = actividades.OrderByDescending(a => a.Fecha).ToList();
            rptActividades.DataSource = actividadesOrdenadas;
            rptActividades.DataBind();
        }


        // --- EVENTO ItemCommand ACTUALIZADO ---
        protected void rptActividades_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            lblMenuMessage.Visible = false; // Ocultar mensaje previo

            // Intentar convertir el ID (CommandArgument)
            if (!int.TryParse(e.CommandArgument?.ToString(), out int actividadId))
            {
                MostrarMensaje("Error: ID de actividad inválido.", "error");
                return;
            }

            // --- Lógica para EDITAR ---
            if (e.CommandName == "EditActivity")
            {
                // Verificar suscripción
                if (usuarioAutenticado.Suscripcion)
                {
                    
                    // 3. Redirige a EditarActividad.aspx, pasando el ID en la QueryString
                    // ESTO ES LO QUE SOLUCIONA QUE SIEMPRE TE LLEVE A LA PRIMERA ACTIVIDAD.
                    Response.Redirect($"EditarActividad.aspx?id={actividadId}");
                    
                }
                else
                {
                    // Mostrar mensaje de advertencia
                    MostrarMensaje("Necesitas una suscripción activa para editar actividades.", "warning");
                }
            }
            // --- Lógica para ELIMINAR ---
            else if (e.CommandName == "DeleteActivity")
            {
                try
                {
                    bool eliminado = conexionDB.EliminaActividad(actividadId);
                    if (eliminado)
                    {
                        MostrarMensaje("Actividad eliminada correctamente.", "success");
                        CargarActividades(); // Recargar la lista para reflejar el cambio
                    }
                    else
                    {
                        // Podría pasar si la actividad se borró justo antes, etc.
                        MostrarMensaje("Error: No se pudo encontrar o eliminar la actividad.", "error");
                    }
                }
                catch (Exception ex)
                {
                    // Loggear el error 'ex' en un sistema real
                    MostrarMensaje("Error inesperado al intentar eliminar la actividad.", "error");
                }
            }
        }

        // --- Función auxiliar para mostrar mensajes ---
        private void MostrarMensaje(string mensaje, string tipo) // tipo = "success", "error", "warning"
        {
            lblMenuMessage.Text = mensaje;
            lblMenuMessage.CssClass = $"menu-message message-{tipo}"; // Asigna la clase CSS dinámicamente
            lblMenuMessage.Visible = true;
        }


        // --- Otros Handlers (btnLogOut, btnPerfil, btnRegistrarActividad sin cambios) ---
        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            Session.Clear(); Session.Abandon(); Response.Redirect("Login.aspx");
        }
        protected void btnPerfil_Click(object sender, EventArgs e)
        {
            // Usar Response.Redirect es generalmente mejor que Server.Transfer si no necesitas mantener el estado de la request
            Response.Redirect("Perfil.aspx");
            // Server.Transfer("Perfil.aspx", true);
        }
        protected void btnRegistrarActividad_Click(object sender, EventArgs e)
        {
            Response.Redirect("RegistrarActividad.aspx");
            // Server.Transfer("RegistrarActividad.aspx", true);
        }
    }
}