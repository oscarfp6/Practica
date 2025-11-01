// Usings necesarios para la funcionalidad de la página.
using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Globalization; // Necesario para CultureInfo.InvariantCulture
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    // Clase code-behind para el formulario de Perfil de Usuario.
    public partial class Perfil : System.Web.UI.Page
    {
        // Campos privados para la conexión a la base de datos y la instancia del usuario.
        private CapaDatos conexionDB;
        private Usuario usuarioautenticado;

        /// <summary>
        /// Se ejecuta al cargar la página. Inicializa la conexión y autentica la sesión.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // --- CÓDIGO DE INICIALIZACIÓN ---
            // Verifica la sesión y obtiene la conexión a la DB.
            if (Session["usuarioautenticado"] == null) { Response.Redirect("Login.aspx"); return; }
            if (Application["conexionDB"] is CapaDatos capa) { conexionDB = capa; }
            else { /* Manejar error crítico si es necesario */ return; }
            usuarioautenticado = (Usuario)Session["usuarioautenticado"];
            // --- Fin CÓDIGO DE INICIALIZACIÓN ---


            if (!IsPostBack)
            {
                // Primera carga:
                CargarDatosPerfil();
                ActualizarEstadoSuscripcionUI(); // <--- NUEVO: Configura el Label y el Botón de Suscripción al cargar.
                lblMensaje.Visible = false;
            }
        }

        /// <summary>
        /// Rellena los TextBoxes con los datos del usuario autenticado.
        /// </summary>
        private void CargarDatosPerfil()
        {
            if (usuarioautenticado == null) return;

            // Asigna valores de las propiedades del modelo a los controles de la UI.
            tbxNombre.Text = usuarioautenticado.Nombre;
            tbxApellidos.Text = usuarioautenticado.Apellidos;
            tbxEmail.Text = usuarioautenticado.Email; // El email no se edita en este formulario.

            // Mapeo seguro de tipos anulables (Nullable<T>) a cadena, o cadena vacía si es null.
            tbxEdad.Text = usuarioautenticado.Edad.HasValue ? usuarioautenticado.Edad.Value.ToString() : string.Empty;

            // Usar CultureInfo.InvariantCulture para asegurar que el peso se formatea con punto decimal.
            tbxPeso.Text = usuarioautenticado.Peso.HasValue ? usuarioautenticado.Peso.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }

        /// <summary>
        /// Manejador del botón Guardar. Actualiza los datos personales (Nombre, Apellidos, Edad, Peso).
        /// </summary>
        protected void btnGuardarPerfil_Click(object sender, EventArgs e)
        {
            // La validación de controles ASP.NET debe ejecutarse primero.
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
                // 1. Obtención y Parsing de los datos del formulario (la validación de límites/formato ya la hizo Page.IsValid).
                string nuevoNombre = tbxNombre.Text.Trim();
                string nuevoApellidos = tbxApellidos.Text.Trim();

                int? nuevaEdad = null;
                if (!string.IsNullOrWhiteSpace(tbxEdad.Text)) { nuevaEdad = int.Parse(tbxEdad.Text.Trim()); }

                double? nuevoPeso = null;
                // Maneja el parsing de decimales con punto (InvariantCulture).
                if (!string.IsNullOrWhiteSpace(tbxPeso.Text)) { nuevoPeso = double.Parse(tbxPeso.Text.Trim().Replace(',', '.'), CultureInfo.InvariantCulture); }

                // 2. Llama al método de la Capa de Negocio para actualizar el objeto.
                // Esta llamada puede lanzar ArgumentException si hay violaciones de reglas (ej. Nombre con dígitos).
                usuarioautenticado.ActualizarPerfil(nuevoNombre, nuevoApellidos, nuevaEdad, nuevoPeso);

                // 3. Persistencia de los cambios en la base de datos.
                if (conexionDB.ActualizaUsuario(usuarioautenticado))
                {
                    // Éxito: Actualizar la Sesión para reflejar el objeto modificado y mostrar mensaje.
                    Session["usuarioautenticado"] = usuarioautenticado;
                    lblMensaje.Text = "Perfil actualizado correctamente.";
                    lblMensaje.CssClass = "message-success";
                    lblMensaje.Visible = true;
                }
                else
                {
                    // Fallo en la Capa de Datos.
                    lblMensaje.Text = "Error al guardar los cambios en la base de datos.";
                    lblMensaje.CssClass = "message-error";
                    lblMensaje.Visible = true;
                }
            }
            catch (ArgumentException ex) // Captura errores de validación lanzados por el modelo (Capa de Negocio).
            {
                lblMensaje.Text = $"Error de validación: {ex.Message}";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
            catch (FormatException ex) // Captura errores de conversión (aunque Page.IsValid debería prevenirlos).
            {
                lblMensaje.Text = $"Error en el formato de los datos: Verifique los campos numéricos (Edad y Peso).";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
            catch (Exception ex) // Captura cualquier error inesperado.
            {
                // Loggear ex en un sistema real.
                lblMensaje.Text = "Error inesperado al intentar actualizar el perfil.";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
        }

        // --- NUEVO MÉTODO: Para actualizar la UI de Suscripción ---
        /// <summary>
        /// Configura el Label y el Botón de Suscripción basándose en el estado actual del usuario.
        /// </summary>
        private void ActualizarEstadoSuscripcionUI()
        {
            if (usuarioautenticado.Suscripcion)
            {
                lblSuscripcionActual.Text = "Activa";
                lblSuscripcionActual.CssClass = "status-active"; // Estilo verde
                btnToggleSuscripcion.Text = "Cancelar Suscripción";
                btnToggleSuscripcion.CssClass = "btn btn-unsubscribe"; // Estilo rojo
            }
            else
            {
                lblSuscripcionActual.Text = "Inactiva";
                lblSuscripcionActual.CssClass = "status-inactive"; // Estilo gris
                btnToggleSuscripcion.Text = "Activar Suscripción";
                btnToggleSuscripcion.CssClass = "btn btn-subscribe"; // Estilo verde
            }
        }

        // --- NUEVO EVENT HANDLER: Para el botón de Suscripción ---
        /// <summary>
        /// Manejador para el botón de activación/cancelación de suscripción.
        /// </summary>
        protected void btnToggleSuscripcion_Click(object sender, EventArgs e)
        {
            if (usuarioautenticado == null) return;

            lblMensaje.Text = string.Empty; // Limpiar mensaje general
            lblMensaje.Visible = false;

            try
            {
                // 1. Cambiar el estado de la suscripción invirtiendo el valor actual.
                usuarioautenticado.Suscripcion = !usuarioautenticado.Suscripcion;

                // 2. Guardar el objeto Usuario actualizado en la base de datos.
                if (conexionDB.ActualizaUsuario(usuarioautenticado))
                {
                    // 3. Éxito: Actualizar la sesión y la interfaz.
                    Session["usuarioautenticado"] = usuarioautenticado;
                    ActualizarEstadoSuscripcionUI();

                    // 4. Mostrar mensaje de éxito específico.
                    lblMensaje.Text = $"Suscripción {(usuarioautenticado.Suscripcion ? "activada" : "cancelada")} correctamente.";
                    lblMensaje.CssClass = "message-success";
                    lblMensaje.Visible = true;
                }
                else
                {
                    // Fallo en la Capa de Datos: Revertir el cambio en el objeto y mostrar error.
                    usuarioautenticado.Suscripcion = !usuarioautenticado.Suscripcion; // Revertir
                    lblMensaje.Text = "Error al actualizar el estado de la suscripción.";
                    lblMensaje.CssClass = "message-error";
                    lblMensaje.Visible = true;
                }
            }
            catch (Exception ex) // Captura cualquier otro error inesperado.
            {
                // Loggear el error 'ex' en un sistema real.
                lblMensaje.Text = "Ha ocurrido un error inesperado al cambiar la suscripción.";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
        }


        /// <summary>
        /// Redirige a la página de cambio de contraseña.
        /// </summary>
        protected void btnIrACambiarPassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("CambiarPassword.aspx");
        }

        /// <summary>
        /// Redirige al menú principal.
        /// </summary>
        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("Menu.aspx");
        }
    }
}
