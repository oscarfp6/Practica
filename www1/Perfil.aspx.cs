using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Globalization;
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
            // ... (Código existente para verificar sesión y obtener conexionDB y usuarioautenticado) ...
            if (Session["usuarioautenticado"] == null) { Response.Redirect("Login.aspx"); return; }
            if (Application["conexionDB"] is CapaDatos capa) { conexionDB = capa; }
            else { /* Manejar error */ return; }
            usuarioautenticado = (Usuario)Session["usuarioautenticado"];
            // --- Fin código existente ---


            if (!IsPostBack)
            {
                CargarDatosPerfil();
                ActualizarEstadoSuscripcionUI(); // <--- NUEVO: Actualizar UI de suscripción al cargar
                lblMensaje.Visible = false;
            }
        }

        private void CargarDatosPerfil()
        {
            // ... (Código existente para cargar Nombre, Apellidos, Edad, Peso, Email) ...
            if (usuarioautenticado == null) return;
            tbxNombre.Text = usuarioautenticado.Nombre;
            tbxApellidos.Text = usuarioautenticado.Apellidos;
            tbxEmail.Text = usuarioautenticado.Email;
            tbxEdad.Text = usuarioautenticado.Edad.HasValue ? usuarioautenticado.Edad.Value.ToString() : string.Empty;
            tbxPeso.Text = usuarioautenticado.Peso.HasValue ? usuarioautenticado.Peso.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            // --- Fin código existente ---
        }

        protected void btnGuardarPerfil_Click(object sender, EventArgs e)
        {
            // ... (Código existente para guardar Nombre, Apellidos, Edad, Peso) ...
            // ¡IMPORTANTE! Asegúrate de que este método YA NO actualiza la propiedad Suscripcion.
            if (!Page.IsValid) 
            {
                lblMensaje.Text = "Por favor, corrige los errores del formulario.";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
                return;
            }

            if (usuarioautenticado == null) 
            {
                Response.Redirect("Login.aspx");
                return;
            }

            lblMensaje.Text = string.Empty; 
            lblMensaje.Visible = false;

            try
            {
                string nuevoNombre = tbxNombre.Text.Trim();
                string nuevoApellidos = tbxApellidos.Text.Trim();
                int? nuevaEdad = null;
                if (!string.IsNullOrWhiteSpace(tbxEdad.Text)) { nuevaEdad = int.Parse(tbxEdad.Text.Trim()); } // Simplificado (validadores ya chequearon)
                double? nuevoPeso = null;
                if (!string.IsNullOrWhiteSpace(tbxPeso.Text)) { nuevoPeso = double.Parse(tbxPeso.Text.Trim().Replace(',', '.'), CultureInfo.InvariantCulture); } // Simplificado

                usuarioautenticado.ActualizarPerfil(nuevoNombre, nuevoApellidos, nuevaEdad, nuevoPeso); // Llama al método SIN suscripción

                if (conexionDB.ActualizaUsuario(usuarioautenticado))
                {
                    Session["usuarioautenticado"] = usuarioautenticado;
                    lblMensaje.Text = "Perfil actualizado correctamente.";
                    lblMensaje.CssClass = "message-success";
                    lblMensaje.Visible = true;
                    // Podrías recargar los datos por si acaso: CargarDatosPerfil();
                }
                else 
                {
                    lblMensaje.Text = "Error al guardar los cambios en la base de datos.";
                    lblMensaje.CssClass = "message-error";
                    lblMensaje.Visible = true;
                }
            }
            catch (ArgumentException ex) 
            {
                lblMensaje.Text = $"Error de validación: {ex.Message}";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
            catch (FormatException ex)
            {
                lblMensaje.Text = $"Error en el formato de los datos: Verifique los campos numéricos (Edad y Peso).";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error inesperado al intentar actualizar el perfil.";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
            // --- Fin código existente ---
        }

        // --- NUEVO MÉTODO: Para actualizar la UI de Suscripción ---
        private void ActualizarEstadoSuscripcionUI()
        {
            if (usuarioautenticado.Suscripcion)
            {
                lblSuscripcionActual.Text = "Activa";
                lblSuscripcionActual.CssClass = "status-active";
                btnToggleSuscripcion.Text = "Cancelar Suscripción";
                btnToggleSuscripcion.CssClass = "btn btn-unsubscribe"; // Estilo de botón rojo
            }
            else
            {
                lblSuscripcionActual.Text = "Inactiva";
                lblSuscripcionActual.CssClass = "status-inactive";
                btnToggleSuscripcion.Text = "Activar Suscripción";
                btnToggleSuscripcion.CssClass = "btn btn-subscribe"; // Estilo de botón verde
            }
        }

        // --- NUEVO EVENT HANDLER: Para el botón de Suscripción ---
        protected void btnToggleSuscripcion_Click(object sender, EventArgs e)
        {
            if (usuarioautenticado == null) return;

            lblMensaje.Text = string.Empty; // Limpiar mensaje general
            lblMensaje.Visible = false;

            try
            {
                // 1. Cambiar el estado de la suscripción en el objeto
                usuarioautenticado.Suscripcion = !usuarioautenticado.Suscripcion;

                // 2. Guardar el objeto Usuario completo (ActualizaUsuario ya lo hace)
                if (conexionDB.ActualizaUsuario(usuarioautenticado))
                {
                    // 3. Actualizar la sesión
                    Session["usuarioautenticado"] = usuarioautenticado;

                    // 4. Actualizar la interfaz de usuario (texto del botón y label)
                    ActualizarEstadoSuscripcionUI();

                    // 5. Mostrar mensaje de éxito específico
                    lblMensaje.Text = $"Suscripción {(usuarioautenticado.Suscripcion ? "activada" : "cancelada")} correctamente.";
                    lblMensaje.CssClass = "message-success";
                    lblMensaje.Visible = true;
                }
                else
                {
                    // Si falla el guardado, revertir el cambio en el objeto (opcional pero bueno)
                    usuarioautenticado.Suscripcion = !usuarioautenticado.Suscripcion;
                    lblMensaje.Text = "Error al actualizar el estado de la suscripción.";
                    lblMensaje.CssClass = "message-error";
                    lblMensaje.Visible = true;
                }
            }
            catch (Exception ex) // Captura cualquier otro error inesperado
            {
                // Loggear el error 'ex' en un sistema real
                lblMensaje.Text = "Ha ocurrido un error inesperado al cambiar la suscripción.";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
        }


        protected void btnIrACambiarPassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("CambiarPassword.aspx");
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("Menu.aspx");
        }
    }
}