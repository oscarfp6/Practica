using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            // 1. Recuperar la conexión DB desde el estado de Aplicación
            lblNingunaActividad.Visible = false;

            if (Application["conexionDB"] == null)
            {
                conexionDB = new CapaDatos();
                Application["conexionDB"] = conexionDB;
            }
            else
            {
                conexionDB = (CapaDatos)Application["conexionDB"];
            }

            // 2. (Validación Importante) Recuperar el usuario de la Sesión
            // ¿Qué pasa si alguien intenta acceder a Menu.aspx sin hacer login?
            if (Session["usuarioautenticado"] == null)
            {
                // Si la sesión no existe, lo devolvemos al Login.
                Response.Redirect("Login.aspx");
                return; // Detenemos la ejecución de la página
            }

            // Si la sesión existe, la recuperamos
            usuarioAutenticado = (Usuario)Session["usuarioautenticado"];

            if (!IsPostBack)
            {
                // 3. (Requisito 1) Mostrar nombre y apellidos
                lblNombreApellidos.Text = $"{usuarioAutenticado.Nombre} {usuarioAutenticado.Apellidos}";

                // 4. (Requisito 3 y 4) Cargar y mostrar actividades
                CargarActividades();
            }
        }

        private void CargarActividades()
        {
            // Obtenemos las actividades para este usuario
            List<Actividad> actividades = conexionDB.ObtenerActividadesUsuario(usuarioAutenticado.Id);
            if (actividades.Count == 0)
            {
                lblNingunaActividad.Visible = true;
                lblNingunaActividad.Text = "No tienes actividades registradas.";
                rptActividades.Visible = false;
                return;
            }
            // (Requisito 4) Ordenamos por fecha, de más reciente a más antigua
            var actividadesOrdenadas = actividades.OrderByDescending(a => a.Fecha).ToList();

            // (Requisito 3) Enlazamos los datos al GridView
            rptActividades.DataSource = actividadesOrdenadas;
            rptActividades.DataBind();

        }

        // (Requisito 2) Evento para el botón de Log Out
        protected void lblLogOut_Click(object sender, EventArgs e)
        {
            // Limpiamos la sesión y redirigimos a Login
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        // (Requisito 2) Evento para el botón de Perfil
        protected void lblPerfil_Click(object sender, EventArgs e)
        {
            // De momento no tenemos página de perfil, pero dejamos el evento listo.
            // Podrías redirigir a una futura "Perfil.aspx"
            // Response.Redirect("Perfil.aspx");

            // Temporalmente, solo mostramos un mensaje
            lblNombreApellidos.Text = "Página de perfil aún no implementada.";
        }

        protected void btnPerfil_Click(object sender, EventArgs e)
        {
            Server.Transfer("Perfil.aspx", true);
        }

        protected void rptActividades_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }
    }
}