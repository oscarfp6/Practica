// Usings necesarios para la funcionalidad de la página web.
using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Globalization; // Necesario para CultureInfo.InvariantCulture
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    // Asegúrate de que el nombre de la clase coincida con Inherits en el ASPX
    public partial class EditarActividad : System.Web.UI.Page
    {
        // Campos privados para la conexión a la base de datos y el usuario autenticado.
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;
        private Actividad actividadActual; // Para guardar la actividad que estamos editando

        /// <summary>
        /// Se ejecuta cada vez que la página es cargada (inicialización y postbacks).
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Recuperar Conexión y Usuario (Esencial) ---
            // 1. Verificación de sesión: Redirige si el usuario no está autenticado.
            if (Session["usuarioautenticado"] == null) { Response.Redirect("Login.aspx"); return; }

            // 2. Recuperación de la conexión a la DB desde Application State.
            if (Application["conexionDB"] is CapaDatos capa) { conexionDB = capa; }
            else { MostrarMensaje("Error crítico: No se pudo acceder a la capa de datos.", false); DeshabilitarFormulario(); return; }

            // 3. Asignación del usuario autenticado.
            usuarioAutenticado = (Usuario)Session["usuarioautenticado"];
            // --- Fin Recuperación ---

            // Lógica de carga de datos (solo la primera vez que la página se carga).
            if (!IsPostBack)
            {
                // --- Cargar Datos de la Actividad ---
                // 1. Intentar obtener el ID de la actividad de la URL (QueryString).
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int actividadId))
                {
                    actividadActual = conexionDB.LeeActividad(actividadId);

                    // 2. Ejecutar validaciones de seguridad y negocio al cargar:

                    // Validación A: ¿Existe la actividad?
                    if (actividadActual == null)
                    {
                        MostrarMensaje("Error: La actividad solicitada no existe.", false);
                        DeshabilitarFormulario();
                    }
                    // Validación B: ¿El usuario actual es el propietario de la actividad? (Control de Acceso)
                    else if (actividadActual.IdUsuario != usuarioAutenticado.Id)
                    {
                        MostrarMensaje("Error: No tienes permiso para editar esta actividad.", false);
                        DeshabilitarFormulario();
                    }
                    // Validación C: ¿Tiene el usuario una suscripción activa? (Regla de Negocio)
                    else if (!usuarioAutenticado.Suscripcion)
                    {
                        MostrarMensaje("Necesitas una suscripción activa para editar actividades.", false);
                        DeshabilitarFormulario();
                    }
                    else
                    {
                        // Flujo de éxito: Guardar el ID en un control oculto para PostBacks y poblar la UI.
                        hdnActividadId.Value = actividadId.ToString();
                        PoblarFormulario();
                    }
                }
                else
                {
                    // Manejo de error si el ID no es válido o está ausente en la URL.
                    MostrarMensaje("Error: No se especificó una actividad válida para editar.", false);
                    DeshabilitarFormulario();
                }
            }
            // Lógica para PostBack (ej. si se pulsa el botón Guardar o Cancelar).
            else if (int.TryParse(hdnActividadId.Value, out int actividadId) && actividadId > 0)
            {
                // En PostBack, se recupera el ID de la actividad desde el campo oculto.
                // No es necesario recargar el objeto 'actividadActual' aquí, ya que se hará en btnGuardar_Click.
            }
            else
            {
                // Manejo de error si el ID se pierde en el PostBack.
                MostrarMensaje("Error: Se perdió la referencia a la actividad que se estaba editando.", false);
                DeshabilitarFormulario();
            }
        }

        /// <summary>
        /// Rellena los controles del formulario (TextBoxes, DropDownList) con los datos de actividadActual.
        /// </summary>
        private void PoblarFormulario()
        {
            // Poblar DropDownList de Tipos utilizando el Enum.
            ddlTipoActividad.DataSource = Enum.GetNames(typeof(TipoActividad));
            ddlTipoActividad.DataBind();

            // Cargar datos en los controles de entrada.
            tbxTitulo.Text = actividadActual.Titulo;
            // Formato estándar "yyyy-MM-dd" necesario para el control <input type="date"> HTML.
            tbxFecha.Text = actividadActual.Fecha.ToString("yyyy-MM-dd");
            ddlTipoActividad.SelectedValue = actividadActual.Tipo.ToString(); // Seleccionar el tipo actual
            // Usar CultureInfo.InvariantCulture para asegurar el punto decimal en números flotantes (Kms).
            tbxKms.Text = actividadActual.Kms.ToString(CultureInfo.InvariantCulture);
            tbxDescripcion.Text = actividadActual.Descripcion;

            // Asegurarse de que el mensaje de estado esté oculto al cargar los datos.
            lblMensaje.Visible = false;
        }

        /// <summary>
        /// Manejador del botón Guardar. Procesa la actualización de los datos del formulario.
        /// </summary>
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Validación de Controles ASP.NET (Validadores de campo).
            if (!Page.IsValid)
            {
                MostrarMensaje("Por favor, corrige los errores indicados.", false);
                return;
            }

            // 2. Recuperar ID de la actividad a actualizar y recargar el objeto (si es multiusuario, esto ayuda a la concurrencia).
            if (!int.TryParse(hdnActividadId.Value, out int actividadId) || actividadId <= 0)
            {
                MostrarMensaje("Error: No se pudo identificar la actividad a guardar.", false);
                return;
            }
            actividadActual = conexionDB.LeeActividad(actividadId);

            // Validar de nuevo existencia, pertenencia y suscripción (medida de seguridad crítica).
            if (actividadActual == null || actividadActual.IdUsuario != usuarioAutenticado.Id || !usuarioAutenticado.Suscripcion)
            {
                MostrarMensaje("Error: No se puede guardar la actividad (permiso o actividad no encontrada).", false);
                DeshabilitarFormulario();
                return;
            }


            // 3. Intentar parsear y actualizar el objeto Actividad, incluyendo validaciones de formato y valor.
            try
            {
                // Parsear los valores del formulario
                string nuevoTitulo = tbxTitulo.Text.Trim();
                string nuevaDescripcion = tbxDescripcion.Text.Trim();

                // Conversión de Tipo de Actividad (Enum)
                TipoActividad nuevoTipo;
                if (!Enum.TryParse(ddlTipoActividad.SelectedValue, out nuevoTipo))
                    throw new FormatException("Tipo actividad inválido.");

                // Conversión y Validación de Fecha
                DateTime nuevaFecha;
                if (!DateTime.TryParse(tbxFecha.Text, out nuevaFecha))
                    throw new FormatException("Fecha inválida.");
                if (nuevaFecha > DateTime.Now) // Validación de valor: La fecha no puede ser futura.
                    throw new ArgumentException("La fecha no puede ser futura.");

                // Conversión y Validación de Kilómetros
                double nuevosKms;
                // Maneja la cultura (reemplazando coma por punto si es necesario) para asegurar un parseo correcto.
                if (!double.TryParse(tbxKms.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out nuevosKms))
                    throw new FormatException("Kms inválidos.");
                if (nuevosKms < 0) // Validación de valor: Los Kms no pueden ser negativos.
                    throw new ArgumentException("Los Kms no pueden ser negativos.");


                // Actualizar SOLO los campos modificables del objeto Actividad.
                actividadActual.Titulo = nuevoTitulo;
                actividadActual.Fecha = nuevaFecha;
                actividadActual.Tipo = nuevoTipo;
                actividadActual.Kms = nuevosKms;
                actividadActual.Descripcion = nuevaDescripcion;
                // NOTA: Otros campos (como Duracion, Desnivel, FCMedia) no se actualizan aquí.

                // 4. Guardar en la base de datos (CapaDatos).
                bool guardado = conexionDB.ActualizaActividad(actividadActual);

                if (guardado)
                {
                    // Flujo de éxito: Mostrar mensaje y redirigir.
                    MostrarMensaje("Actividad actualizada correctamente. Redirigiendo...", true);

                    // Script para redirigir al Menú después de 2 segundos.
                    string script = "setTimeout(function(){ window.location = 'Menu.aspx'; }, 2000);";
                    ScriptManager.RegisterStartupScript(this, GetType(), "RedirigirMenu", script, true);
                    DeshabilitarFormulario();
                }
                else
                {
                    // Fallo de persistencia en la base de datos.
                    MostrarMensaje("Error al guardar los cambios en la base de datos.", false);
                }
            }
            // Manejo de errores de formato y de validación de negocio.
            catch (FormatException ex)
            {
                MostrarMensaje($"Error en el formato de los datos: {ex.Message}", false);
            }
            catch (ArgumentException ex) // Captura validaciones de la clase Actividad (ej. Kms < 0) o validaciones manuales (ej. Fecha futura).
            {
                MostrarMensaje($"Error de validación: {ex.Message}", false);
            }
            catch (Exception ex) // Captura cualquier otro error inesperado.
            {
                MostrarMensaje($"Error inesperado: {ex.Message}", false);
                // Aquí se recomienda loggear la excepción completa para análisis.
            }
        }

        /// <summary>
        /// Manejador del botón Cancelar. Redirige al menú principal.
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Volver al menú principal
            Response.Redirect("Menu.aspx");
        }

        // --- Funciones auxiliares ---

        /// <summary>
        /// Muestra el mensaje de estado en la interfaz, aplicando el estilo de éxito o error.
        /// </summary>
        private void MostrarMensaje(string mensaje, bool exito)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = exito ? "message-label success-message" : "message-label error-message";
            lblMensaje.Visible = true;
        }

        /// <summary>
        /// Deshabilita los controles de entrada principales del formulario para evitar la edición después de un error crítico o un éxito.
        /// </summary>
        private void DeshabilitarFormulario()
        {
            tbxTitulo.Enabled = false;
            ddlTipoActividad.Enabled = false;
            tbxFecha.Enabled = false;
            tbxKms.Enabled = false;
            tbxDescripcion.Enabled = false;
            btnGuardar.Enabled = false;
            // btnCancelar se puede dejar habilitado para permitir al usuario volver al menú
        }
    }
}
