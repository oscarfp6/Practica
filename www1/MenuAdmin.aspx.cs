// Usings necesarios para el funcionamiento de la página.
using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq; // Necesario para OrderBy
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    // Clase code-behind para el menú de administración.
    public partial class MenuAdmin : System.Web.UI.Page
    {
        // Campos privados para la conexión a la base de datos y la instancia del usuario.
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;

        /// <summary>
        /// Se ejecuta al cargar la página. Realiza la autenticación y la validación de roles.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // --- 1. Recuperar Conexión y Usuario ---
            if (Application["conexionDB"] is CapaDatos capa) { conexionDB = capa; }
            else { Response.Redirect("Login.aspx"); return; } // Error crítico: Redirigir si falla la conexión

            if (Session["usuarioautenticado"] is Usuario user) { usuarioAutenticado = user; }
            else { Response.Redirect("Login.aspx"); return; } // Redirigir si no hay sesión

            // --- 2. VALIDACIÓN DE SEGURIDAD (Control de Acceso Basado en Roles) ---
            // Verifica si el usuario actual tiene el ID de Administrador (ID 1).
            if (usuarioAutenticado.Id != 1)
            {
                // Si no es el administrador, redirige inmediatamente al menú normal.
                Response.Redirect("Menu.aspx");
                return;
            }

            // Lógica de carga de datos solo en la primera carga (IsPostBack = false).
            if (!IsPostBack)
            {
                lblAdminNombre.Text = $"Bienvenido, Administrador ({usuarioAutenticado.Nombre})";
                CargarUsuarios();
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios desde la base de datos y los enlaza al Repeater.
        /// </summary>
        private void CargarUsuarios()
        {
            // Llama a la Capa de Datos para obtener la lista de todos los usuarios.
            List<Usuario> todosLosUsuarios = conexionDB.ObtenerTodosLosUsuarios();

            // Ordena la lista (ej. por ID) y la enlaza al Repeater de la interfaz.
            rptUsuarios.DataSource = todosLosUsuarios.OrderBy(u => u.Id).ToList();
            rptUsuarios.DataBind();
        }

        /// <summary>
        /// Se dispara por CADA fila que se enlaza en el Repeater (ItemDataBound).
        /// Se usa para configurar dinámicamente los controles de cada fila (estado inicial, habilitación/deshabilitación).
        /// </summary>
        protected void rptUsuarios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // 1. Encontrar los controles de la fila.
                var ddlNuevoEstado = (DropDownList)e.Item.FindControl("ddlNuevoEstado");
                var tbxNuevaPassword = (TextBox)e.Item.FindControl("tbxNuevaPassword");
                var btnActualizarUsuario = (Button)e.Item.FindControl("btnActualizarUsuario");

                // 2. Obtener el objeto Usuario de esta fila para acceder a sus datos.
                var usuario = (Usuario)e.Item.DataItem;

                if (ddlNuevoEstado != null && usuario != null && tbxNuevaPassword != null && btnActualizarUsuario != null)
                {
                    // 3. Rellenar el DropDownList con los valores del Enum EstadoUsuario.
                    ddlNuevoEstado.DataSource = Enum.GetNames(typeof(EstadoUsuario));
                    ddlNuevoEstado.DataBind();

                    // 4. Establecer el estado actual del usuario como el valor seleccionado.
                    ddlNuevoEstado.SelectedValue = usuario.Estado.ToString();

                    // --- 5. LÓGICA DE CONTROL: Prevenir que el Admin se auto-administre ---
                    if (usuario.Id == 1) // Si el usuario de la fila es el propio Administrador
                    {
                        // Deshabilita todos los controles para evitar cambios accidentales o auto-bloqueo.
                        ddlNuevoEstado.Enabled = false;
                        tbxNuevaPassword.Enabled = false;
                        btnActualizarUsuario.Enabled = false;
                        btnActualizarUsuario.OnClientClick = "return false;"; // Prevenir el envío al servidor
                    }
                    else // Es un usuario normal, permitir la administración.
                    {
                        ddlNuevoEstado.Enabled = true;
                        tbxNuevaPassword.Enabled = true;
                        btnActualizarUsuario.Enabled = true;
                        // Confirmación del lado del cliente para la acción de actualización.
                        btnActualizarUsuario.OnClientClick = "return confirm('¿Estás seguro de que quieres actualizar este usuario?');";
                    }
                }
            }
        }

        /// <summary>
        /// Se dispara cuando se hace clic en el botón "Actualizar" de una fila.
        /// Implementa la lógica de actualización del usuario.
        /// </summary>
        protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Actualizar")
            {
                // 1. Obtener el ID del usuario.
                int usuarioId = Convert.ToInt32(e.CommandArgument);

                // 2. Seguridad: Doble chequeo para no permitir la auto-actualización.
                if (usuarioId == 1) return;

                // 3. Encontrar los controles de la fila.
                var ddlNuevoEstado = (DropDownList)e.Item.FindControl("ddlNuevoEstado");
                var tbxNuevaPassword = (TextBox)e.Item.FindControl("tbxNuevaPassword");
                var lblMensajeFila = (Label)e.Item.FindControl("lblMensajeFila");

                // 4. Recuperar el objeto Usuario COMPLETO de la DB (para trabajar con el estado actual).
                Usuario usuarioAActualizar = conexionDB.LeeUsuarioPorId(usuarioId);

                if (usuarioAActualizar == null || ddlNuevoEstado == null || tbxNuevaPassword == null || lblMensajeFila == null)
                {
                    // Manejo de error si los controles o el usuario no se encuentran.
                    return;
                }

                // --- 5. Lógica de Actualización ---
                bool cambiosHechos = false;
                lblMensajeFila.Visible = true;
                lblMensajeFila.CssClass = "message message-error"; // Estilo inicial de error.

                // A. Actualizar Estado
                EstadoUsuario estadoSeleccionado = (EstadoUsuario)Enum.Parse(typeof(EstadoUsuario), ddlNuevoEstado.SelectedValue);
                if (usuarioAActualizar.Estado != estadoSeleccionado)
                {
                    usuarioAActualizar.Estado = estadoSeleccionado;
                    cambiosHechos = true;
                }

                // B. Actualizar Contraseña (si el campo no está vacío)
                string nuevaPassword = tbxNuevaPassword.Text;
                if (!string.IsNullOrWhiteSpace(nuevaPassword))
                {
                    // Usa el método de la capa de negocio, que incluye la validación de seguridad de la contraseña.
                    if (usuarioAActualizar.AdminEstablecerPassword(nuevaPassword))
                    {
                        cambiosHechos = true;
                        // Si la contraseña se cambia con éxito, el modelo lo marca como ACTIVO y elimina el bloqueo.
                        ddlNuevoEstado.SelectedValue = EstadoUsuario.Activo.ToString();
                    }
                    else
                    {
                        // Fallo: La contraseña no cumple con los requisitos de seguridad.
                        lblMensajeFila.Text = "¡Pass no segura!";
                        return; // Detiene el proceso de guardado si la contraseña es inválida.
                    }
                }

                // --- 6. Guardar en DB (Persistencia) ---
                if (cambiosHechos)
                {
                    if (conexionDB.ActualizaUsuario(usuarioAActualizar))
                    {
                        // Éxito.
                        lblMensajeFila.CssClass = "message message-success";
                        lblMensajeFila.Text = "¡Actualizado!";
                        tbxNuevaPassword.Text = ""; // Limpiar el campo de contraseña por seguridad.
                        // Actualizar el label de "Estado Actual" para reflejar el cambio (ej. de Bloqueado a Activo).
                        ((Label)e.Item.FindControl("lblEstadoActual")).Text = usuarioAActualizar.Estado.ToString();
                    }
                    else
                    {
                        // Fallo al guardar en la Capa de Datos.
                        lblMensajeFila.Text = "Error al guardar";
                    }
                }
                else
                {
                    // No hubo cambios para guardar.
                    lblMensajeFila.CssClass = "message";
                    lblMensajeFila.Text = "Sin cambios";
                }
            }
        }

        /// <summary>
        /// Cierra la sesión del administrador y lo redirige a la página de login.
        /// </summary>
        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
