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
    public partial class MenuAdmin : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- 1. Recuperar Conexión y Usuario ---
            if (Application["conexionDB"] is CapaDatos capa) { conexionDB = capa; }
            else { Response.Redirect("Login.aspx"); return; } // Error crítico

            if (Session["usuarioautenticado"] is Usuario user) { usuarioAutenticado = user; }
            else { Response.Redirect("Login.aspx"); return; } // No hay sesión

            // --- 2. VALIDACIÓN DE SEGURIDAD ---
            // ¡El paso más importante! Solo el ID 1 puede estar aquí.
            if (usuarioAutenticado.Id != 1)
            {
                // Si un usuario no admin intenta entrar, lo echamos al menú normal
                Response.Redirect("Menu.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblAdminNombre.Text = $"Bienvenido, Administrador ({usuarioAutenticado.Nombre})";
                CargarUsuarios();
            }
        }

        /// <summary>
        /// Carga la lista de todos los usuarios en el Repeater.
        /// </summary>
        private void CargarUsuarios()
        {
            // Usamos el nuevo método que añadimos a CapaDatos
            List<Usuario> todosLosUsuarios = conexionDB.ObtenerTodosLosUsuarios();

            // Ordenamos para que el Admin (ID 1) salga primero
            rptUsuarios.DataSource = todosLosUsuarios.OrderBy(u => u.Id).ToList();
            rptUsuarios.DataBind();
        }

        /// <summary>
        /// Se dispara por CADA fila que se enlaza en el Repeater.
        /// Lo usamos para rellenar el DropDownList de estados.
        /// </summary>
        /// <summary>
        /// Se dispara por CADA fila que se enlaza en el Repeater.
        /// Lo usamos para rellenar el DropDownList de estados Y AHORA TAMBIÉN PARA HABILITAR/DESHABILITAR.
        /// </summary>
        protected void rptUsuarios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // 1. Encontrar todos los controles en esta fila
                var ddlNuevoEstado = (DropDownList)e.Item.FindControl("ddlNuevoEstado");
                var tbxNuevaPassword = (TextBox)e.Item.FindControl("tbxNuevaPassword");
                var btnActualizarUsuario = (Button)e.Item.FindControl("btnActualizarUsuario");

                // 2. Obtener el objeto Usuario de esta fila
                var usuario = (Usuario)e.Item.DataItem;

                if (ddlNuevoEstado != null && usuario != null && tbxNuevaPassword != null && btnActualizarUsuario != null)
                {
                    // 3. Rellenar el DropDownList
                    ddlNuevoEstado.DataSource = Enum.GetNames(typeof(EstadoUsuario));
                    ddlNuevoEstado.DataBind();

                    // 4. Establecer el valor seleccionado actual
                    ddlNuevoEstado.SelectedValue = usuario.Estado.ToString();

                    // --- 5. NUEVA LÓGICA: HABILITAR/DESHABILITAR ---
                    if (usuario.Id == 1) // Es el admin
                    {
                        ddlNuevoEstado.Enabled = false;
                        tbxNuevaPassword.Enabled = false;
                        btnActualizarUsuario.Enabled = false;
                        btnActualizarUsuario.OnClientClick = "return false;"; // Prevenir postback
                    }
                    else // Es un usuario normal
                    {
                        ddlNuevoEstado.Enabled = true;
                        tbxNuevaPassword.Enabled = true;
                        btnActualizarUsuario.Enabled = true;
                        btnActualizarUsuario.OnClientClick = "return confirm('¿Estás seguro de que quieres actualizar este usuario?');";
                    }
                }
            }
        }

        /// <summary>
        /// Se dispara cuando se hace clic en el botón "Actualizar" de CUALQUIER fila.
        /// </summary>
        protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Actualizar")
            {
                // 1. Obtener el ID del usuario (viene en el CommandArgument)
                int usuarioId = Convert.ToInt32(e.CommandArgument);

                // 2. Seguridad: No permitir la auto-actualización desde aquí
                if (usuarioId == 1) return;

                // 3. Encontrar los controles de ESA fila específica (usando e.Item)
                var ddlNuevoEstado = (DropDownList)e.Item.FindControl("ddlNuevoEstado");
                var tbxNuevaPassword = (TextBox)e.Item.FindControl("tbxNuevaPassword");
                var lblMensajeFila = (Label)e.Item.FindControl("lblMensajeFila");

                // 4. Recuperar el objeto Usuario COMPLETO de la DB
                Usuario usuarioAActualizar = conexionDB.LeeUsuarioPorId(usuarioId);

                if (usuarioAActualizar == null || ddlNuevoEstado == null || tbxNuevaPassword == null || lblMensajeFila == null)
                {
                    // Error inesperado
                    return;
                }

                // --- 5. Lógica de Actualización ---
                bool cambiosHechos = false;
                lblMensajeFila.Visible = true;
                lblMensajeFila.CssClass = "message message-error"; // Asumimos error primero

                // A. Actualizar Estado
                EstadoUsuario estadoSeleccionado = (EstadoUsuario)Enum.Parse(typeof(EstadoUsuario), ddlNuevoEstado.SelectedValue);
                if (usuarioAActualizar.Estado != estadoSeleccionado)
                {
                    usuarioAActualizar.Estado = estadoSeleccionado;
                    cambiosHechos = true;
                }

                // B. Actualizar Contraseña (si se escribió algo)
                string nuevaPassword = tbxNuevaPassword.Text;
                if (!string.IsNullOrWhiteSpace(nuevaPassword))
                {
                    // Usamos el nuevo método que añadimos a Usuario.cs
                    bool passCambiada = usuarioAActualizar.AdminEstablecerPassword(nuevaPassword);

                    if (passCambiada)
                    {
                        cambiosHechos = true;
                        // Nota: AdminEstablecerPassword ya pone al usuario como "Activo"
                        // Así que actualizamos el DDL para reflejarlo
                        ddlNuevoEstado.SelectedValue = EstadoUsuario.Activo.ToString();
                    }
                    else
                    {
                        // La contraseña no era segura
                        lblMensajeFila.Text = "¡Pass no segura!";
                        return; // No guardamos nada si la pass es inválida
                    }
                }

                // --- 6. Guardar en DB (si hubo cambios) ---
                if (cambiosHechos)
                {
                    if (conexionDB.ActualizaUsuario(usuarioAActualizar))
                    {
                        lblMensajeFila.CssClass = "message message-success";
                        lblMensajeFila.Text = "¡Actualizado!";
                        tbxNuevaPassword.Text = ""; // Limpiar el campo de contraseña
                        // Actualizar el label de "Estado Actual"
                        ((Label)e.Item.FindControl("lblEstadoActual")).Text = usuarioAActualizar.Estado.ToString();
                    }
                    else
                    {
                        lblMensajeFila.Text = "Error al guardar";
                    }
                }
                else
                {
                    lblMensajeFila.CssClass = "message";
                    lblMensajeFila.Text = "Sin cambios";
                }
            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}