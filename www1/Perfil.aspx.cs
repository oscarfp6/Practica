using Datos;
// using Datos.Database; // No es necesario si CapaDatos implementa la interfaz directamente
using MiLogica.ModeloDatos;
using System;
using System.Globalization; // Para NumberStyles y CultureInfo en Parse
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    public partial class Perfil : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioautenticado; // Cambiado nombre para consistencia

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Verificar Sesión (protección de la página) - CORREGIDO
            if (Session["usuarioautenticado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // 2. Obtener la Capa de Datos (desde Application) - CORREGIDO
            // Asumiendo que la clave en Application es "conexionDB" como en otras páginas
            if (Application["conexionDB"] is CapaDatos capaDatos)
            {
                conexionDB = capaDatos;
            }
            else
            {
                // Es mejor mostrar el error en un label y detener
                lblMensaje.Text = "Error crítico: No se pudo acceder a la capa de datos.";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
                // Deshabilitar botones si falla la carga inicial
                btnGuardarPerfil.Enabled = false;
                btnIrACambiarPassword.Enabled = false;
                btnVolver.Enabled = false;
                return; // Detener ejecución si no hay capa de datos
            }

            // 3. Cargar Usuario desde la sesión - CORREGIDO
            usuarioautenticado = (Usuario)Session["usuarioautenticado"];


            if (!IsPostBack)
            {
                CargarDatosPerfil();
                lblMensaje.Visible = false; // Asegurarse de que el mensaje esté oculto al cargar
            }
        }

        private void CargarDatosPerfil()
        {
            if (usuarioautenticado == null) return; // Seguridad adicional

            // Cargar los datos del objeto Usuario en los TextBox
            tbxNombre.Text = usuarioautenticado.Nombre;
            tbxApellidos.Text = usuarioautenticado.Apellidos;
            tbxEmail.Text = usuarioautenticado.Email; // Ya estaba bien

            // Cargar Edad y Peso (manejando posibles nulos)
            tbxEdad.Text = usuarioautenticado.Edad.HasValue ? usuarioautenticado.Edad.Value.ToString() : string.Empty;
            // Para el peso, usar CultureInfo.InvariantCulture para mostrar con '.' si hay decimales
            tbxPeso.Text = usuarioautenticado.Peso.HasValue ? usuarioautenticado.Peso.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;

            // Suscripción eliminada de la UI
            // chkSuscripcion.Checked = usuarioautenticado.Suscripcion;
        }

        protected void btnGuardarPerfil_Click(object sender, EventArgs e)
        {
            // Validar primero con los validadores ASP.NET
             if (!Page.IsValid)
             {
                 lblMensaje.Text = "Por favor, corrige los errores indicados.";
                 lblMensaje.CssClass = "message-error";
                 lblMensaje.Visible = true;
                 return;
             }


            if (usuarioautenticado == null)
            {
                 lblMensaje.Text = "Error: No se pudo cargar la información del usuario.";
                 lblMensaje.CssClass = "message-error";
                 lblMensaje.Visible = true;
                 return;
            }

            lblMensaje.Text = string.Empty; // Limpiar mensaje previo
            lblMensaje.Visible = false;

            try
            {
                // 1. Obtener nuevos valores
                string nuevoNombre = tbxNombre.Text.Trim();
                string nuevoApellidos = tbxApellidos.Text.Trim();

                // Parsear Edad (nullable int)
                int? nuevaEdad = null;
                if (!string.IsNullOrWhiteSpace(tbxEdad.Text))
                {
                    if (int.TryParse(tbxEdad.Text.Trim(), out int edadTemp))
                    {
                        nuevaEdad = edadTemp;
                    }
                    else
                    {
                        throw new FormatException("El valor introducido para la edad no es un número entero válido.");
                    }
                }

                 // Parsear Peso (nullable double)
                double? nuevoPeso = null;
                if (!string.IsNullOrWhiteSpace(tbxPeso.Text))
                {
                     // Usar NumberStyles.Any y CultureInfo.InvariantCulture para permitir '.' como decimal
                    if (double.TryParse(tbxPeso.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double pesoTemp))
                    {
                        nuevoPeso = pesoTemp;
                    }
                     else
                    {
                        throw new FormatException("El valor introducido para el peso no es un número válido.");
                    }
                }


                // Suscripción eliminada, ya no se actualiza aquí
                // bool nuevaSuscripcion = chkSuscripcion.Checked;

                // 2. Actualizar el objeto de dominio (usando el método actualizado)
                 // ¡Asegúrate de que ActualizarPerfil en Usuario.cs acepte Edad y Peso!
                usuarioautenticado.ActualizarPerfil(nuevoNombre, nuevoApellidos, nuevaEdad, nuevoPeso);

                // 3. Persistir los cambios en la Capa de Datos
                if (conexionDB.ActualizaUsuario(usuarioautenticado))
                {
                    // 4. Actualizar la Sesión y mostrar éxito
                    Session["usuarioautenticado"] = usuarioautenticado; // Actualizar el objeto en sesión
                    lblMensaje.Text = "Perfil actualizado correctamente.";
                    lblMensaje.CssClass = "message-success";
                    lblMensaje.Visible = true;
                }
                else
                {
                    // Este error podría ocurrir si el usuario se eliminó mientras editaba, etc.
                    lblMensaje.Text = "Error al guardar el perfil en la base de datos.";
                    lblMensaje.CssClass = "message-error";
                     lblMensaje.Visible = true;
                }
            }
             catch (FormatException ex) // Captura errores de TryParse si fallan (aunque los validadores deberían prevenirlo)
            {
                lblMensaje.Text = $"Error en el formato de los datos: {ex.Message}";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
             catch (ArgumentException ex) // Captura errores de validación de la clase Usuario (ej. edad/peso negativos)
            {
                lblMensaje.Text = $"Error de validación: {ex.Message}";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
            catch (Exception ex) // Captura cualquier otro error inesperado
            {
                // Loggear el error 'ex' en un sistema real
                lblMensaje.Text = "Ha ocurrido un error inesperado al guardar el perfil.";
                lblMensaje.CssClass = "message-error";
                lblMensaje.Visible = true;
            }
        }

         protected void btnIrACambiarPassword_Click(object sender, EventArgs e)
        {
             // Redirige a la página de cambio de contraseña
             // CausesValidation="false" en el botón evita que se ejecuten los validadores de esta página
             Response.Redirect("CambiarPassword.aspx");
        }


        protected void btnVolver_Click(object sender, EventArgs e)
        {
            // Redirigir al menú principal
             // CausesValidation="false" en el botón evita que se ejecuten los validadores de esta página
            Response.Redirect("Menu.aspx");
        }
    }
}