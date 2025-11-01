// Usings necesarios para la funcionalidad de la página.
using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Globalization; // Necesario para ParseExact y CultureInfo.InvariantCulture
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    // Clase code-behind para el formulario de registro de nuevas actividades.
    public partial class WebForm5 : System.Web.UI.Page
    {
        // Campos privados para la conexión a la base de datos y la instancia del usuario.
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;

        /// <summary>
        /// Se ejecuta al cargar la página. Inicializa la conexión, autentica la sesión y carga datos iniciales.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Recuperar Conexión y Usuario ---
            // Verifica y recupera la conexión a la base de datos desde Application State.
            if (Application["conexionDB"] == null)
            {
                // Manejo de error si la capa de datos no está inicializada.
                Response.Redirect("Login.aspx");
                return;
            }
            conexionDB = (CapaDatos)Application["conexionDB"];

            // Verifica y recupera el usuario autenticado desde Session State.
            if (Session["usuarioautenticado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            usuarioAutenticado = (Usuario)Session["usuarioautenticado"];
            // --- Fin Recuperación ---

            // Lógica de inicialización ejecutada solo en la primera carga (IsPostBack = false).
            if (!IsPostBack)
            {
                // --- Poblar DropDownList de Tipos de Actividad ---
                // Carga los nombres del Enum TipoActividad en el control DropDownList.
                ddlTipoActividad.DataSource = Enum.GetNames(typeof(TipoActividad));
                ddlTipoActividad.DataBind();

                // --- Inicializar campos ---
                // Establece la fecha por defecto como hoy, en formato compatible con input type="date" (yyyy-MM-dd).
                tbxFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
                lblMensaje.Visible = false;
            }
        }

        /// <summary>
        /// Manejador del botón Confirmar. Procesa la entrada del usuario, valida los datos y guarda la actividad.
        /// </summary>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            // 1. Validar la página (ejecuta los validadores de ASP.NET en el lado del servidor).
            if (!Page.IsValid)
            {
                lblMensaje.Text = "Hay errores en el formulario.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Visible = true;
                return;
            }

            // 2. Intentar parsear los datos y construir el objeto Actividad.
            try
            {
                // Campos de texto (Título y Descripción) se limpian de espacios.
                string titulo = tbxTitulo.Text.Trim();
                string descripcion = tbxDescripcion.Text.Trim();

                // Parsear Tipo Actividad (De string a Enum).
                TipoActividad tipo;
                if (!Enum.TryParse(ddlTipoActividad.SelectedValue, out tipo))
                {
                    throw new FormatException("Tipo de actividad no válido.");
                }

                // Parsear Fecha.
                DateTime fecha;
                if (!DateTime.TryParse(tbxFecha.Text, out fecha))
                {
                    throw new FormatException("Formato de fecha no válido.");
                }
                // Validación manual: La fecha no puede ser futura.
                if (fecha > DateTime.Now)
                {
                    throw new ArgumentException("La fecha no puede ser en el futuro.");
                }


                // Parsear Duración (Requiere formato estricto: hh:mm o hh:mm:ss).
                TimeSpan duracion;
                // TimeSpan.TryParseExact intenta coincidir la cadena con una de las plantillas de formato.
                if (!TimeSpan.TryParseExact(tbxDuracion.Text.Trim(),
                                            new string[] { "h\\:mm", "hh\\:mm", "h\\:mm\\:ss", "hh\\:mm\\:ss" },
                                            CultureInfo.InvariantCulture, out duracion))
                {
                    throw new FormatException("Formato de duración no válido (use hh:mm o hh:mm:ss).");
                }
                // Validación manual: La duración debe ser mayor que cero.
                if (duracion <= TimeSpan.Zero)
                {
                    throw new ArgumentException("La duración debe ser mayor que cero.");
                }


                // Parsear Kms (Manejo de decimales con punto o coma para CultureInfo.InvariantCulture).
                double kms;
                if (!double.TryParse(tbxKms.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out kms))
                {
                    throw new FormatException("Valor de Kms no válido.");
                }

                // Parsear Desnivel (entero).
                int metrosDesnivel;
                if (!int.TryParse(tbxMetrosDesnivel.Text.Trim(), out metrosDesnivel))
                {
                    throw new FormatException("Valor de desnivel no válido.");
                }

                // Parsear FC Media (Frecuencia Cardíaca Media) - Campo opcional (Nullable<int>).
                int? fcMedia = null; // Por defecto es null
                if (!string.IsNullOrWhiteSpace(tbxFcMedia.Text))
                {
                    int fc;
                    if (int.TryParse(tbxFcMedia.Text.Trim(), out fc))
                    {
                        fcMedia = fc; // Se asigna solo si el parsing es exitoso.
                    }
                    else
                    {
                        throw new FormatException("Valor de FC Media no válido.");
                    }
                }

                // 3. Crear el objeto Actividad. Esto ejecuta las validaciones internas restantes de la Capa de Modelo.
                Actividad nuevaActividad = new Actividad(
                    usuarioAutenticado.Id,
                    titulo,
                    kms,
                    metrosDesnivel,
                    duracion,
                    fecha,
                    tipo,
                    descripcion,
                    fcMedia
                );

                // 4. Guardar en la "base de datos" (Capa de Datos).
                bool guardado = conexionDB.GuardaActividad(nuevaActividad);

                // 5. Mostrar resultado y redirigir.
                if (guardado)
                {
                    lblMensaje.Text = "Actividad registrada con éxito. Redirigiendo al menú...";
                    lblMensaje.ForeColor = System.Drawing.Color.Green;
                    lblMensaje.Visible = true;

                    // Script para redirigir tras un breve retraso (mejora UX).
                    string script = "setTimeout(function(){ window.location = 'Menu.aspx'; }, 2000);";
                    ScriptManager.RegisterStartupScript(this, GetType(), "RedirigirMenu", script, true);

                    // Deshabilitar botones para prevenir doble envío.
                    btnConfirmar.Enabled = false;
                    btnCancelar.Enabled = false;
                }
                else
                {
                    // Fallo de persistencia.
                    lblMensaje.Text = "Error al guardar la actividad. Inténtalo de nuevo.";
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    lblMensaje.Visible = true;
                }
            }
            catch (FormatException ex) // Captura errores de conversión de tipos (ej. "abc" en Kms).
            {
                lblMensaje.Text = $"Error en el formato de los datos: {ex.Message}";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Visible = true;
            }
            catch (ArgumentException ex) // Captura errores de validación (ej. Duración negativa o Fecha futura).
            {
                lblMensaje.Text = $"Error de validación: {ex.Message}";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Visible = true;
            }
            catch (Exception ex) // Captura cualquier otro error inesperado.
            {
                // En una aplicación real, se debería loggear el error.
                lblMensaje.Text = $"Ha ocurrido un error inesperado: {ex.Message}";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Visible = true;
            }
        }

        /// <summary>
        /// Manejador del botón Cancelar. Redirige al menú principal.
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Simplemente redirige de vuelta al menú sin guardar.
            Response.Redirect("Menu.aspx");
        }
    }
}
