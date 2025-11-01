// Usings estándar necesarios para una página ASP.NET.
using Datos;
using MiLogica.ModeloDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    // Clase code-behind para el formulario de Login (WebForm1).
    public partial class WebForm1 : System.Web.UI.Page
    {
        // Campos privados para la conexión a la base de datos y la instancia del usuario.
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;

        /// <summary>
        /// Se ejecuta al cargar la página (PostBack o primera carga).
        /// Se encarga de la inicialización de la CapaDatos.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Lógica de inicialización ejecutada solo en la primera carga (IsPostBack = false).
            if (!IsPostBack)
            {
                // Verifica si la conexión a la DB ya existe en Application State.
                if (Application["conexionDB"] == null)
                {
                    // Si no existe, la crea una única vez y la guarda en Application State.
                    conexionDB = new CapaDatos();
                    Application["conexionDB"] = conexionDB;
                }
                else
                {
                    // Si existe, la recupera desde Application State.
                    conexionDB = (CapaDatos)Application["conexionDB"];
                }

                usuarioAutenticado = null; // Reinicia el usuario.
                lblIncorrecto.Text = string.Empty;
                lblIncorrecto.Visible = false;

                // --- NUEVO: Asegurar estado inicial de botones ---
                // Configura la interfaz al estado de login normal.
                btnAceptar.Visible = true;
                btnDesbloquear.Visible = false;
                lblIncorrecto.CssClass = "error-message"; // Por defecto el mensaje es de error.
            }
            else
            {
                // En PostBack (clics en botones), recupera la conexión de Application State.
                conexionDB = (CapaDatos)Application["conexionDB"];
            }
        }

        /// <summary>
        /// Manejador del botón Aceptar (Intento de Login).
        /// Contiene la lógica principal de autenticación, fallos y gestión de bloqueo/cooldown.
        /// </summary>
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            if (conexionDB != null)
            {
                // 1. Siempre leemos el usuario primero para obtener su estado actual.
                usuarioAutenticado = conexionDB.LeeUsuario(tbxUsuario.Text);

                // 2. Validación de existencia de usuario.
                if (usuarioAutenticado == null)
                {
                    // Error: Usuario no encontrado (o credenciales genéricas para evitar enumeración de usuarios).
                    lblIncorrecto.Text = "Usuario o contraseña incorrectos";
                    lblIncorrecto.Visible = true;
                    lblIncorrecto.CssClass = "error-message";
                    btnAceptar.Visible = true;
                    btnDesbloquear.Visible = false;
                    return; // Salir, no hay usuario.
                }

                // 3. El usuario existe, intentar login usando la lógica de negocio.
                if (usuarioAutenticado.PermitirLogin(tbxContraseña.Text))
                {
                    // --- ÉXITO DE LOGIN ---
                    // Guardar el objeto Usuario actualizado en la sesión.
                    Session["usuarioautenticado"] = usuarioAutenticado;
                    lblIncorrecto.Text = string.Empty;

                    // Redirección basada en el rol (asumido por Id == 1 para Admin).
                    if (usuarioAutenticado.Id == 1)
                    {
                        Response.Redirect("AdminMenu.aspx");
                    }
                    else
                    {
                        // Usa Server.Transfer para mantener la misma URL y el estado de la sesión si es necesario.
                        Server.Transfer("Menu.aspx", true);
                    }
                }
                else
                {
                    // --- FALLO DE LOGIN ---
                    // Comprobar por qué falló.
                    if (usuarioAutenticado.Estado == EstadoUsuario.Bloqueado)
                    {
                        // --- LÓGICA DE COOLDOWN (SEGURIDAD) ---
                        // Verifica si el tiempo de bloqueo (BloqueadoHasta) sigue vigente.
                        if (usuarioAutenticado.BloqueadoHasta.HasValue && DateTime.Now < usuarioAutenticado.BloqueadoHasta.Value)
                        {
                            // 1. AÚN EN COOLDOWN: Muestra el tiempo restante.
                            int minutosRestantes = CalcularMinutosRestantes(usuarioAutenticado.BloqueadoHasta);
                            lblIncorrecto.Text = $"Cuenta bloqueada. Inténtalo de nuevo en {minutosRestantes} min.";
                            lblIncorrecto.Visible = true;
                            lblIncorrecto.CssClass = "error-message";

                            // Mantiene el botón Aceptar visible para que el usuario pueda re-intentar tras la espera.
                            btnAceptar.Visible = true;
                            btnDesbloquear.Visible = false;
                        }
                        else
                        {
                            // 2. BLOQUEADO, PERO COOLDOWN EXPIRADO: Ofrece la opción de Desbloqueo Manual.
                            lblIncorrecto.Text = "Cuenta bloqueada. Si la contraseña es correcta, pulsa 'Desbloquear Cuenta'.";
                            lblIncorrecto.Visible = true;
                            lblIncorrecto.CssClass = "error-message";

                            // Cambia la UI para mostrar el botón de desbloqueo.
                            btnAceptar.Visible = false; // Ocultamos botón de login
                            btnDesbloquear.Visible = true; // Mostramos botón de desbloqueo
                        }
                    }
                    else
                    {
                        // 3. FALLO NORMAL (Contraseña incorrecta, pero aún no bloqueado).
                        lblIncorrecto.Text = "Usuario o contraseña incorrectos";
                        lblIncorrecto.Visible = true;
                        lblIncorrecto.CssClass = "error-message";
                        btnAceptar.Visible = true;
                        btnDesbloquear.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// Manejador del botón Desbloquear Cuenta. Se muestra cuando el cooldown ha expirado.
        /// </summary>
        protected void btnDesbloquear_Click(object sender, EventArgs e)
        {
            // Asegura que la conexión a la DB esté disponible.
            if (conexionDB == null) { conexionDB = (CapaDatos)Application["conexionDB"]; }

            string email = tbxUsuario.Text;
            string password = tbxContraseña.Text;

            // Volvemos a leer el usuario para asegurar el estado y evitar inconsistencias.
            Usuario usuarioParaDesbloquear = conexionDB.LeeUsuario(email);

            if (usuarioParaDesbloquear == null)
            {
                // Manejo de error inesperado (el usuario debería existir a estas alturas).
                lblIncorrecto.Text = "Error inesperado. Vuelve a intentarlo.";
                lblIncorrecto.CssClass = "error-message";
                lblIncorrecto.Visible = true;
                btnAceptar.Visible = true;
                btnDesbloquear.Visible = false;
                return;
            }

            // Llama al método de negocio DesbloquearUsuario.
            if (usuarioParaDesbloquear.DesbloquearUsuario(email, password))
            {
                // ¡Éxito! La cuenta se desbloqueó.
                lblIncorrecto.Text = "¡Cuenta desbloqueada! Por favor, inicia sesión.";
                lblIncorrecto.CssClass = "success-message"; // Usamos el estilo de éxito (verde).
                lblIncorrecto.Visible = true;
                btnAceptar.Visible = true; // Mostramos login de nuevo
                btnDesbloquear.Visible = false; // Ocultamos desbloqueo
            }
            else
            {
                // --- FALLO DE DESBLOQUEO (Generalmente por contraseña incorrecta) ---
                // Si falla, el método de negocio asume que ha reiniciado el cooldown.

                // Vuelve a leer el usuario para obtener el nuevo tiempo de bloqueo establecido por el modelo.
                usuarioParaDesbloquear = conexionDB.LeeUsuario(email);

                // Muestra el nuevo tiempo de espera.
                int minutosRestantes = CalcularMinutosRestantes(usuarioParaDesbloquear.BloqueadoHasta);
                lblIncorrecto.Text = $"Contraseña incorrecta. La cuenta sigue bloqueada. Inténtalo en {minutosRestantes} min.";
                lblIncorrecto.Visible = true;
                lblIncorrecto.CssClass = "error-message";

                // Forzamos al usuario a esperar el nuevo cooldown (deshabilita el desbloqueo inmediato).
                btnAceptar.Visible = true;
                btnDesbloquear.Visible = false;
            }
        }

        /// <summary>
        /// Calcula los minutos restantes (redondeado hacia arriba) para un bloqueo, basándose en el tiempo futuro.
        /// </summary>
        private int CalcularMinutosRestantes(DateTime? tiempoBloqueo)
        {
            if (!tiempoBloqueo.HasValue) return 0;

            TimeSpan restante = tiempoBloqueo.Value - DateTime.Now;

            if (restante.TotalMinutes < 0) return 0;

            // Math.Ceiling asegura que el tiempo se redondee hacia arriba (ej. 1.2 minutos se informa como 2).
            return (int)Math.Ceiling(restante.TotalMinutes);
        }


        /// <summary>
        /// Redirige a la página de registro.
        /// </summary>
        protected void btnRegistrarse_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");

        }

        /// <summary>
        /// Redirige a la página de cambio de contraseña.
        /// </summary>
        protected void btnCambiarPassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("CambiarPassword.aspx", true);
        }
    }
}
