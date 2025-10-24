using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Recuperar Conexión y Usuario ---
            if (Session["usuarioautenticado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (Application["conexionDB"] is CapaDatos capaDatos)
            {
                conexionDB = capaDatos;
            }
            else
            {
                MostrarErrorGeneral("Error crítico: No se pudo acceder a la capa de datos.");
                DeshabilitarFormulario();
                return;
            }
            usuarioAutenticado = (Usuario)Session["usuarioautenticado"];
            // --- Fin Recuperación ---

            if (!IsPostBack)
            {
                // Ocultar mensajes de error específicos al cargar
                lblErrorPasswordActual.Visible = false;
                lblErrorNuevoPassword.Visible = false;
                lblResultado.Visible = false;
            }
        }

        protected void btnConfirmarCambioPassword_Click(object sender, EventArgs e)
        {
            // Ocultar mensajes previos
            lblErrorPasswordActual.Visible = false;
            lblErrorNuevoPassword.Visible = false;
            lblResultado.Visible = false;

            // 1. Validar controles ASP.NET
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

            // 2. Obtener contraseñas
            string passActual = tbxPasswordActual.Text; // No usamos Trim() en contraseñas
            string passNueva = tbxNuevoPassword.Text;
            // No necesitamos passConfirmar aquí porque el CompareValidator ya lo hizo

            // 3. Llamar a la lógica de negocio (método CambiarPassword del Usuario)
            bool cambioExitoso = usuarioAutenticado.CambiarPassword(passActual, passNueva);

            if (cambioExitoso)
            {
                // 4. Si la lógica de negocio fue exitosa, persistir en CapaDatos
                if (conexionDB.ActualizaUsuario(usuarioAutenticado))
                {
                    // 5. Actualizar sesión y mostrar mensaje de éxito
                    Session["usuarioautenticado"] = usuarioAutenticado; // Guardar usuario actualizado
                    MostrarResultado("Contraseña cambiada con éxito.", true);
                    // Opcional: Redirigir a Perfil/Menú después de un delay
                    string script = "setTimeout(function(){ window.location = 'Perfil.aspx'; }, 2000);"; // 2 segundos
                    ScriptManager.RegisterStartupScript(this, GetType(), "RedirigirPerfil", script, true);
                    DeshabilitarFormulario(); // Evitar doble envío
                }
                else
                {
                    // Error MUY improbable si CambiarPassword tuvo éxito, pero...
                    MostrarErrorGeneral("Error al guardar la nueva contraseña en la base de datos.");
                    // Podríamos intentar revertir el cambio en el objeto usuarioAutenticado si fuera crítico
                }
            }
            else
            {
                // 6. Si CambiarPassword falló, determinar la causa y mostrar mensaje específico
                // Causa 1: Contraseña actual incorrecta
                if (!usuarioAutenticado.ComprobarPassWord(passActual)) // Re-verificar la actual
                {
                    lblErrorPasswordActual.Visible = true;
                }
                // Causa 2: Nueva contraseña no cumple requisitos (ValidarPassword falló)
                else if (!MiLogica.Utils.Password.ValidarPassword(passNueva))
                {
                    lblErrorNuevoPassword.Visible = true;
                    // Actualizar texto si el validador no es suficiente
                    lblErrorNuevoPassword.Text = "La nueva contraseña no cumple los requisitos de seguridad (longitud, mayúsculas, etc.).";
                }
                // Causa 3: Usuario bloqueado (aunque CambiarPassword ya lo chequea)
                else if (usuarioAutenticado.Estado == EstadoUsuario.Bloqueado)
                {
                    MostrarErrorGeneral("Tu cuenta está bloqueada. No puedes cambiar la contraseña.");
                    DeshabilitarFormulario();
                }
                else
                {
                    // Otro error no especificado por CambiarPassword
                    MostrarErrorGeneral("No se pudo cambiar la contraseña por un motivo desconocido.");
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Volver a la página de Perfil
            Response.Redirect("Perfil.aspx");
        }

        // --- Funciones auxiliares para mostrar mensajes ---
        private void MostrarResultado(string mensaje, bool exito)
        {
            lblResultado.Text = mensaje;
            lblResultado.CssClass = exito ? "message-label success-message" : "message-label error-message";
            lblResultado.Visible = true;
        }

        private void MostrarErrorGeneral(string mensaje)
        {
            MostrarResultado(mensaje, false); // Reutilizamos lblResultado para errores generales
        }


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