// Usings necesarios para la funcionalidad de la página web.
using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

// Define el espacio de nombres de la aplicación web.
namespace www1
{
    // Clase code-behind para el formulario de cambio de contraseña.
    public partial class WebForm4 : System.Web.UI.Page
    {
        // Campos privados para la conexión a la base de datos y la instancia del usuario.
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;

        /// <summary>
        /// Se ejecuta cada vez que la página es cargada.
        /// Se encarga de la inicialización, autenticación y recuperación de la sesión.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Recuperar Conexión y Usuario ---
            // 1. Verificación de autenticación: Si no hay usuario en sesión, redirige al login.
            if (Session["usuarioautenticado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // 2. Recuperación de la conexión a la DB desde Application State.
            if (Application["conexionDB"] is CapaDatos capaDatos)
            {
                conexionDB = capaDatos;
            }
            else
            {
                // Manejo de error crítico si no se encuentra la capa de datos.
                MostrarErrorGeneral("Error crítico: No se pudo acceder a la capa de datos.");
                DeshabilitarFormulario();
                return;
            }

            // 3. Asignación del usuario autenticado desde la sesión.
            usuarioAutenticado = (Usuario)Session["usuarioautenticado"];
            // --- Fin Recuperación ---

            // Inicialización solo la primera vez que se carga la página (no en PostBacks).
            if (!IsPostBack)
            {
                // Ocultar mensajes de error específicos al cargar
                lblErrorPasswordActual.Visible = false;
                lblErrorNuevoPassword.Visible = false;
                lblResultado.Visible = false;
            }
        }

        /// <summary>
        /// Manejador de eventos para el botón de confirmar el cambio de contraseña.
        /// Contiene la lógica principal de validación y actualización.
        /// </summary>
        protected void btnConfirmarCambioPassword_Click(object sender, EventArgs e)
        {
            // Ocultar mensajes previos para limpiar la interfaz.
            lblErrorPasswordActual.Visible = false;
            lblErrorNuevoPassword.Visible = false;
            lblResultado.Visible = false;

            // 1. Validar controles ASP.NET (ej. RequiredFieldValidator, CompareValidator).
            if (!Page.IsValid)
            {
                return; // Los mensajes de los validadores se mostrarán automáticamente
            }

            // Seguridad adicional: verificar que el usuario todavía existe
            if (usuarioAutenticado == null || conexionDB == null)
            {
                MostrarErrorGeneral("Error de sesión o conexión. Intenta iniciar sesión de nuevo.");
                DeshabilitarFormulario();
                return;
            }

            // 2. Obtener contraseñas (Textos de los TextBox).
            string passActual = tbxPasswordActual.Text; // No usamos Trim() en contraseñas
            string passNueva = tbxNuevoPassword.Text;
            // La confirmación ya fue validada por CompareValidator.

            // 3. Llamar a la lógica de negocio (método CambiarPassword del objeto Usuario).
            // Esta llamada es de CAJA BLANCA, ya que utiliza el objeto de dominio directamente.
            bool cambioExitoso = usuarioAutenticado.CambiarPassword(passActual, passNueva);

            if (cambioExitoso)
            {
                // 4. Si la lógica de negocio fue exitosa, persistir el cambio en la Capa de Datos.
                if (conexionDB.ActualizaUsuario(usuarioAutenticado))
                {
                    // 5. Flujo de éxito: Actualizar la sesión, mostrar mensaje y redirigir.
                    Session["usuarioautenticado"] = usuarioAutenticado; // Guardar usuario actualizado
                    MostrarResultado("Contraseña cambiada con éxito.", true);

                    // Script para redirigir automáticamente después de 2 segundos.
                    string script = "setTimeout(function(){ window.location = 'Perfil.aspx'; }, 2000);"; // 2 segundos
                    ScriptManager.RegisterStartupScript(this, GetType(), "RedirigirPerfil", script, true);
                    DeshabilitarFormulario(); // Evitar doble envío mientras espera la redirección.
                }
                else
                {
                    // Manejo de error de persistencia (fallo de la BD, etc.).
                    MostrarErrorGeneral("Error al guardar la nueva contraseña en la base de datos.");
                }
            }
            else
            {
                // 6. Flujo de fallo: Determinar la causa del fallo de CambiarPassword y mostrar error específico.

                // Causa 1: Contraseña actual incorrecta (se re-verifica la condición aquí).
                if (!usuarioAutenticado.ComprobarPassWord(passActual))
                {
                    lblErrorPasswordActual.Visible = true;
                }
                // Causa 2: Nueva contraseña no cumple requisitos (ValidarPassword falló).
                else if (!MiLogica.Utils.Password.ValidarPassword(passNueva))
                {
                    lblErrorNuevoPassword.Visible = true;
                    // Se actualiza el texto del error para dar más detalle al usuario.
                    lblErrorNuevoPassword.Text = "La nueva contraseña no cumple los requisitos de seguridad (longitud, mayúsculas, etc.).";
                }
                // Causa 3: Usuario bloqueado (aunque el método de negocio lo maneja, se informa al usuario).
                else if (usuarioAutenticado.Estado == EstadoUsuario.Bloqueado)
                {
                    MostrarErrorGeneral("Tu cuenta está bloqueada. No puedes cambiar la contraseña.");
                    DeshabilitarFormulario();
                }
                else
                {
                    // Causa genérica para cualquier otro fallo.
                    MostrarErrorGeneral("No se pudo cambiar la contraseña por un motivo desconocido.");
                }
            }
        }

        /// <summary>
        /// Manejador de eventos para el botón de cancelar, redirige a la página de perfil.
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Volver a la página de Perfil
            Response.Redirect("Perfil.aspx");
        }

        // --- Funciones auxiliares para mostrar mensajes ---

        /// <summary>
        /// Muestra un mensaje de resultado (éxito o error) al usuario.
        /// </summary>
        private void MostrarResultado(string mensaje, bool exito)
        {
            lblResultado.Text = mensaje;
            // Aplica la clase CSS para el estilo visual (verde para éxito, rojo para error).
            lblResultado.CssClass = exito ? "message-label success-message" : "message-label error-message";
            lblResultado.Visible = true;
        }

        /// <summary>
        /// Muestra un error general o crítico, utilizando la misma etiqueta de resultado.
        /// </summary>
        private void MostrarErrorGeneral(string mensaje)
        {
            MostrarResultado(mensaje, false); // Reutilizamos lblResultado para errores generales (con estilo de error).
        }


        /// <summary>
        /// Deshabilita todos los controles principales del formulario para evitar interacciones no deseadas, 
        /// especialmente después de un éxito o un error crítico.
        /// </summary>
        private void DeshabilitarFormulario()
        {
            tbxPasswordActual.Enabled = false;
            tbxNuevoPassword.Enabled = false;
            tbxConfirmarPassword.Enabled = false;
            btnConfirmarCambioPassword.Enabled = false;
            btnCancelar.Enabled = false; // También deshabilitar cancelar si hay error crítico
        }
    }
}
