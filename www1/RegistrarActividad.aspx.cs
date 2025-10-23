using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Globalization; // Necesario para ParseExact
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Recuperar Conexión y Usuario (Similar a Menu.aspx) ---
            if (Application["conexionDB"] == null)
            {
                // Manejo de error o inicialización si es necesario
                // Por simplicidad, asumimos que existe o redirigimos
                Response.Redirect("Login.aspx"); // O manejar de otra forma
                return;
            }
            conexionDB = (CapaDatos)Application["conexionDB"];

            if (Session["usuarioautenticado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            usuarioAutenticado = (Usuario)Session["usuarioautenticado"];
            // --- Fin Recuperación ---

            if (!IsPostBack)
            {
                // --- Poblar DropDownList de Tipos de Actividad ---
                // Usamos los nombres del Enum TipoActividad
                ddlTipoActividad.DataSource = Enum.GetNames(typeof(TipoActividad));
                ddlTipoActividad.DataBind();

                // --- Inicializar campos ---
                tbxFecha.Text = DateTime.Now.ToString("yyyy-MM-dd"); // Formato que entiende TextMode="Date"
                lblMensaje.Visible = false;
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            // 1. Validar la página (ejecuta los validadores ASP.NET)
            if (!Page.IsValid)
            {
                lblMensaje.Text = "Hay errores en el formulario.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Visible = true;
                return;
            }

            // 2. Intentar parsear los datos
            try
            {
                string titulo = tbxTitulo.Text.Trim();
                string descripcion = tbxDescripcion.Text.Trim();

                // Parsear Tipo Actividad desde el DropDownList
                TipoActividad tipo;
                if (!Enum.TryParse(ddlTipoActividad.SelectedValue, out tipo))
                {
                    throw new FormatException("Tipo de actividad no válido.");
                }

                // Parsear Fecha
                DateTime fecha;
                if (!DateTime.TryParse(tbxFecha.Text, out fecha)) // TextMode="Date" suele dar yyyy-MM-dd
                {
                    throw new FormatException("Formato de fecha no válido.");
                }
                // Validar que no sea futura (aunque la clase Actividad también lo hace)
                if (fecha > DateTime.Now)
                {
                    throw new ArgumentException("La fecha no puede ser en el futuro.");
                }


                // Parsear Duración (hh:mm o hh:mm:ss)
                TimeSpan duracion;
                // TimeSpan.ParseExact requiere CultureInfo para :
                if (!TimeSpan.TryParseExact(tbxDuracion.Text.Trim(), new string[] { "h\\:mm", "hh\\:mm", "h\\:mm\\:ss", "hh\\:mm\\:ss" }, CultureInfo.InvariantCulture, out duracion))
                {
                    throw new FormatException("Formato de duración no válido (use hh:mm o hh:mm:ss).");
                }
                // Validar que sea mayor que cero (la clase Actividad también lo hace)
                if (duracion <= TimeSpan.Zero)
                {
                    throw new ArgumentException("La duración debe ser mayor que cero.");
                }


                // Parsear Kms
                double kms;
                if (!double.TryParse(tbxKms.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out kms)) // Reemplazar coma por punto y usar cultura invariante
                {
                    throw new FormatException("Valor de Kms no válido.");
                }

                // Parsear Desnivel
                int metrosDesnivel;
                if (!int.TryParse(tbxMetrosDesnivel.Text.Trim(), out metrosDesnivel))
                {
                    throw new FormatException("Valor de desnivel no válido.");
                }

                // Parsear FC Media (opcional)
                int? fcMedia = null; // Por defecto es null
                if (!string.IsNullOrWhiteSpace(tbxFcMedia.Text))
                {
                    int fc;
                    if (int.TryParse(tbxFcMedia.Text.Trim(), out fc))
                    {
                        fcMedia = fc; // Solo asignamos si se parsea correctamente
                    }
                    else
                    {
                        throw new FormatException("Valor de FC Media no válido.");
                    }
                }

                // 3. Crear el objeto Actividad (usará las validaciones internas de la clase)
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

                // 4. Guardar en la "base de datos"
                bool guardado = conexionDB.GuardaActividad(nuevaActividad);

                // 5. Mostrar resultado y redirigir
                if (guardado)
                {
                    lblMensaje.Text = "Actividad registrada con éxito. Redirigiendo al menú...";
                    lblMensaje.ForeColor = System.Drawing.Color.Green;
                    lblMensaje.Visible = true;

                    // Redirigir después de un pequeño retraso para que el usuario vea el mensaje
                    string script = "setTimeout(function(){ window.location = 'Menu.aspx'; }, 2000);"; // 2 segundos
                    ScriptManager.RegisterStartupScript(this, GetType(), "RedirigirMenu", script, true);

                    // Deshabilitar el botón para evitar doble envío
                    btnConfirmar.Enabled = false;
                    btnCancelar.Enabled = false;
                }
                else
                {
                    // Esto no debería pasar si las validaciones son correctas, pero es buena práctica manejarlo
                    lblMensaje.Text = "Error al guardar la actividad. Inténtalo de nuevo.";
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    lblMensaje.Visible = true;
                }
            }
            catch (FormatException ex) // Errores al convertir tipos
            {
                lblMensaje.Text = $"Error en el formato de los datos: {ex.Message}";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Visible = true;
            }
            catch (ArgumentException ex) // Errores de validación (de la clase Actividad o manuales)
            {
                lblMensaje.Text = $"Error de validación: {ex.Message}";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Visible = true;
            }
            catch (Exception ex) // Captura cualquier otro error inesperado
            {
                // En una aplicación real, aquí registrarías el error detallado (log)
                lblMensaje.Text = $"Ha ocurrido un error inesperado: {ex.Message}";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Visible = true;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Simplemente redirige de vuelta al menú sin guardar
            Response.Redirect("Menu.aspx");
        }
    }
}