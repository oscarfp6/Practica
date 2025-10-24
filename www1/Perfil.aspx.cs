using Datos;
using Datos.Database;      // Para ICapaDatos
using MiLogica.ModeloDatos; // Para Usuario
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    public partial class Perfil : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioautenticado;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Verificar Sesión (protección de la página)
            if (Session["IdUsuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // 2. Obtener la Capa de Datos (desde Application)
            if (Application["CapaDatos"] is CapaDatos capaDatos)
            {
                conexionDB = capaDatos;
            }
            else
            {
                lblMensaje.Text = "Error de configuración de datos.";
                lblMensaje.CssClass = "message-error";
                return;
            }

            // 3. Cargar Usuario desde la sesión (o desde la DAL si no está en sesión)
            int idUsuario = (int)Session["IdUsuario"];

            if (Session["UsuarioActual"] is Usuario usuario)
            {
                usuarioautenticado = usuario;
            }
            else
            {
                usuarioautenticado = conexionDB.LeeUsuarioPorId(idUsuario); // LeeUsuarioPorId existe en ICapaDatos
                if (usuarioautenticado == null)
                {
                    lblMensaje.Text = "Error: Usuario no encontrado.";
                    lblMensaje.CssClass = "message-error";
                    Session.Abandon();
                    return;
                }
                Session["UsuarioActual"] = usuarioautenticado;
            }

            if (!IsPostBack)
            {
                CargarDatosPerfil();
            }
        }

        private void CargarDatosPerfil()
        {
            // Cargar los datos del objeto Usuario en los TextBox
            tbxNombre.Text = usuarioautenticado.Nombre;
            tbxApellidos.Text = usuarioautenticado.Apellidos;
            tbxEmail.Text = usuarioautenticado.Email;
            chkSuscripcion.Checked = usuarioautenticado.Suscripcion;
        }

        protected void btnGuardarPerfil_Click(object sender, EventArgs e)
        {
            if (usuarioautenticado == null) return;

            lblMensaje.Text = string.Empty;
            lblMensaje.CssClass = "message-success";

            // 1. Obtener y validar nuevos valores (la validación de formato simple se puede hacer aquí)
            if (string.IsNullOrWhiteSpace(tbxNombre.Text) || string.IsNullOrWhiteSpace(tbxApellidos.Text))
            {
                lblMensaje.Text = "El nombre y los apellidos no pueden estar vacíos.";
                lblMensaje.CssClass = "message-error";
                return;
            }

            string nuevoNombre = tbxNombre.Text.Trim();
            string nuevoApellidos = tbxApellidos.Text.Trim();
            bool nuevaSuscripcion = chkSuscripcion.Checked;

            // 2. Actualizar el objeto de dominio (la lógica está en el objeto Usuario)
            usuarioautenticado.ActualizarPerfil(nuevoNombre, nuevoApellidos, nuevaSuscripcion); // Lógica de dominio

            // 3. Persistir los cambios en la Capa de Datos
            if (conexionDB.ActualizaUsuario(usuarioautenticado)) // ActualizaUsuario existe en ICapaDatos
            {
                // 4. Actualizar la Sesión y mostrar éxito
                Session["UsuarioActual"] = usuarioautenticado;
                lblMensaje.Text = "Perfil actualizado correctamente.";
            }
            else
            {
                lblMensaje.Text = "Error al guardar el perfil. El ID de usuario podría ser inválido.";
                lblMensaje.CssClass = "message-error";
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            // Redirigir al menú principal
            Response.Redirect("Menu.aspx");
        }
    }
}