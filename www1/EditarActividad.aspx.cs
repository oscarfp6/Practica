using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    // Asegúrate de que el nombre de la clase coincida con Inherits en el ASPX
    public partial class EditarActividad : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;
        private Actividad actividadActual; // Para guardar la actividad que estamos editando

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Recuperar Conexión y Usuario (Esencial) ---
            if (Session["usuarioautenticado"] == null) { Response.Redirect("Login.aspx"); return; }
            if (Application["conexionDB"] is CapaDatos capa) { conexionDB = capa; }
            else { MostrarMensaje("Error crítico: No se pudo acceder a la capa de datos.", false); DeshabilitarFormulario(); return; }
            usuarioAutenticado = (Usuario)Session["usuarioautenticado"];
            // --- Fin Recuperación ---

            if (!IsPostBack)
            {
                // --- Cargar Datos de la Actividad ---
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int actividadId))
                {
                    actividadActual = conexionDB.LeeActividad(actividadId);

                    // Validación: ¿Existe la actividad? ¿Pertenece al usuario? ¿Está suscrito?
                    if (actividadActual == null)
                    {
                        MostrarMensaje("Error: La actividad solicitada no existe.", false);
                        DeshabilitarFormulario();
                    }
                    else if (actividadActual.IdUsuario != usuarioAutenticado.Id)
                    {
                        MostrarMensaje("Error: No tienes permiso para editar esta actividad.", false);
                        DeshabilitarFormulario();
                    }
                    else if (!usuarioAutenticado.Suscripcion) // Doble chequeo por si accedió directamente a la URL
                    {
                        MostrarMensaje("Necesitas una suscripción activa para editar actividades.", false);
                        DeshabilitarFormulario();
                    }
                    else
                    {
                        // Guardar el ID para usarlo en el PostBack (Guardar)
                        hdnActividadId.Value = actividadId.ToString();
                        PoblarFormulario();
                    }
                }
                else
                {
                    MostrarMensaje("Error: No se especificó una actividad válida para editar.", false);
                    DeshabilitarFormulario();
                }
            }
            // En PostBack, necesitamos recuperar el ID guardado
            else if (int.TryParse(hdnActividadId.Value, out int actividadId) && actividadId > 0)
            {
                // Podríamos recargar la actividad aquí si fuera necesario,
                // pero para una simple actualización, con el ID es suficiente
                // para llamar a ActualizaActividad.
            }
            else
            {
                MostrarMensaje("Error: Se perdió la referencia a la actividad que se estaba editando.", false);
                DeshabilitarFormulario();
            }
        }

        private void PoblarFormulario()
        {
            // Poblar DropDownList de Tipos
            ddlTipoActividad.DataSource = Enum.GetNames(typeof(TipoActividad));
            ddlTipoActividad.DataBind();

            // Cargar datos en los controles
            tbxTitulo.Text = actividadActual.Titulo;
            tbxFecha.Text = actividadActual.Fecha.ToString("yyyy-MM-dd"); // Formato para input type="date"
            ddlTipoActividad.SelectedValue = actividadActual.Tipo.ToString(); // Seleccionar el tipo actual
            tbxKms.Text = actividadActual.Kms.ToString(CultureInfo.InvariantCulture); // Usar punto decimal
            tbxDescripcion.Text = actividadActual.Descripcion;

            // Asegurarse de que el mensaje esté oculto
            lblMensaje.Visible = false;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Validar Controles ASP.NET
            if (!Page.IsValid)
            {
                MostrarMensaje("Por favor, corrige los errores indicados.", false);
                return;
            }

            // 2. Recuperar ID y cargar la actividad original OTRA VEZ
            //    (Buena práctica para evitar sobreescribir cambios concurrentes si esto fuera multiusuario real)
            if (!int.TryParse(hdnActividadId.Value, out int actividadId) || actividadId <= 0)
            {
                MostrarMensaje("Error: No se pudo identificar la actividad a guardar.", false);
                return;
            }
            actividadActual = conexionDB.LeeActividad(actividadId);

            // Validar de nuevo existencia y pertenencia (por si acaso)
            if (actividadActual == null || actividadActual.IdUsuario != usuarioAutenticado.Id || !usuarioAutenticado.Suscripcion)
            {
                MostrarMensaje("Error: No se puede guardar la actividad (permiso o actividad no encontrada).", false);
                DeshabilitarFormulario();
                return;
            }


            // 3. Intentar parsear y actualizar el objeto
            try
            {
                // Parsear los valores del formulario
                string nuevoTitulo = tbxTitulo.Text.Trim();
                string nuevaDescripcion = tbxDescripcion.Text.Trim();
                TipoActividad nuevoTipo;
                if (!Enum.TryParse(ddlTipoActividad.SelectedValue, out nuevoTipo))
                    throw new FormatException("Tipo actividad inválido.");
                DateTime nuevaFecha;
                if (!DateTime.TryParse(tbxFecha.Text, out nuevaFecha))
                    throw new FormatException("Fecha inválida.");
                if (nuevaFecha > DateTime.Now) // Validación extra
                    throw new ArgumentException("La fecha no puede ser futura.");
                double nuevosKms;
                if (!double.TryParse(tbxKms.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out nuevosKms))
                    throw new FormatException("Kms inválidos.");
                if (nuevosKms < 0) // Validación extra
                    throw new ArgumentException("Los Kms no pueden ser negativos.");


                // Actualizar SOLO los campos permitidos del objeto Actividad
                actividadActual.Titulo = nuevoTitulo;
                actividadActual.Fecha = nuevaFecha;
                actividadActual.Tipo = nuevoTipo;
                actividadActual.Kms = nuevosKms;
                actividadActual.Descripcion = nuevaDescripcion;
                // NOTA: NO actualizamos Duracion, Desnivel, FCMedia aquí según tus requisitos.

                // 4. Guardar en la base de datos
                bool guardado = conexionDB.ActualizaActividad(actividadActual);

                if (guardado)
                {
                    MostrarMensaje("Actividad actualizada correctamente. Redirigiendo...", true);
                    // Redirigir al Menú después de un delay
                    string script = "setTimeout(function(){ window.location = 'Menu.aspx'; }, 2000);";
                    ScriptManager.RegisterStartupScript(this, GetType(), "RedirigirMenu", script, true);
                    DeshabilitarFormulario();
                }
                else
                {
                    MostrarMensaje("Error al guardar los cambios en la base de datos.", false);
                }
            }
            catch (FormatException ex)
            {
                MostrarMensaje($"Error en el formato de los datos: {ex.Message}", false);
            }
            catch (ArgumentException ex) // Captura validaciones de la clase Actividad (ej. Kms < 0)
            {
                MostrarMensaje($"Error de validación: {ex.Message}", false);
            }
            catch (Exception ex) // Otros errores
            {
                MostrarMensaje($"Error inesperado: {ex.Message}", false);
                // Loggear ex
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Volver al menú principal
            Response.Redirect("Menu.aspx");
        }

        // --- Funciones auxiliares ---
        private void MostrarMensaje(string mensaje, bool exito)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = exito ? "message-label success-message" : "message-label error-message";
            lblMensaje.Visible = true;
        }

        private void DeshabilitarFormulario()
        {
            tbxTitulo.Enabled = false;
            ddlTipoActividad.Enabled = false;
            tbxFecha.Enabled = false;
            tbxKms.Enabled = false;
            tbxDescripcion.Enabled = false;
            btnGuardar.Enabled = false;
            // btnCancelar se puede dejar habilitado para volver
        }
    }
}