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
    public partial class WebForm1 : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioAutenticado;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (conexionDB == null)
                {
                    // Esta línea parece un error, creas dos veces el objeto.
                    // Lo corrijo para que solo cree el objeto si no existe.
                    if (Application["conexionDB"] == null)
                    {
                        conexionDB = new CapaDatos();
                        Application["conexionDB"] = conexionDB;
                    }
                    else
                    {
                        conexionDB = (CapaDatos)Application["conexionDB"];
                    }
                }

                usuarioAutenticado = null;
                lblIncorrecto.Text = string.Empty;
                lblIncorrecto.Visible = false;

                // --- NUEVO: Asegurar estado inicial de botones ---
                btnAceptar.Visible = true;
                btnDesbloquear.Visible = false;
                lblIncorrecto.CssClass = "error-message"; // Por defecto es un error
            }
            else
            {
                conexionDB = (CapaDatos)Application["conexionDB"];
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            if (conexionDB != null)
            {
                // 1. Siempre leemos el usuario primero
                usuarioAutenticado = conexionDB.LeeUsuario(tbxUsuario.Text);

                // 2. Validar si el usuario existe
                if (usuarioAutenticado == null)
                {
                    lblIncorrecto.Text = "Usuario o contraseña incorrectos";
                    lblIncorrecto.Visible = true;
                    lblIncorrecto.CssClass = "error-message";
                    btnAceptar.Visible = true;
                    btnDesbloquear.Visible = false;
                    return; // Salir, no hay usuario
                }

                // 3. El usuario existe, intentar login
                if (usuarioAutenticado.PermitirLogin(tbxContraseña.Text))
                {
                    // --- ÉXITO DE LOGIN ---
                    Session["usuarioautenticado"] = usuarioAutenticado;
                    lblIncorrecto.Text = string.Empty;
                    if (usuarioAutenticado.Id == 1)
                    {
                        Response.Redirect("AdminMenu.aspx");
                    }
                    else
                    {
                        Server.Transfer("Menu.aspx", true);
                    }
                }
                else
                {
                    // --- FALLO DE LOGIN ---
                    // Comprobar por qué falló
                    if (usuarioAutenticado.Estado == EstadoUsuario.Bloqueado)
                    {
                        // --- LÓGICA DE COOLDOWN (SEGURIDAD) ---
                        // El usuario está bloqueado. Comprobamos SI ADEMÁS está en cooldown.
                        // Asumimos que Usuario.cs tiene la propiedad 'BloqueadoHasta'
                        if (usuarioAutenticado.BloqueadoHasta.HasValue && DateTime.Now < usuarioAutenticado.BloqueadoHasta.Value)
                        {
                            // 1. AÚN EN COOLDOWN: El usuario no puede hacer NADA.
                            int minutosRestantes = CalcularMinutosRestantes(usuarioAutenticado.BloqueadoHasta);
                            lblIncorrecto.Text = $"Cuenta bloqueada. Inténtalo de nuevo en {minutosRestantes} min.";
                            lblIncorrecto.Visible = true;
                            lblIncorrecto.CssClass = "error-message";

                            // --- ¡CAMBIO CLAVE! ---
                            // Dejamos "Aceptar" visible para que el usuario pueda re-intentar tras el cooldown.
                            btnAceptar.Visible = true;
                            btnDesbloquear.Visible = false;
                        }
                        else
                        {
                            // 2. BLOQUEADO, PERO COOLDOWN EXPIRADO: Damos una oportunidad para desbloquear.
                            lblIncorrecto.Text = "Cuenta bloqueada. Si la contraseña es correcta, pulsa 'Desbloquear Cuenta'.";
                            lblIncorrecto.Visible = true;
                            lblIncorrecto.CssClass = "error-message";
                            btnAceptar.Visible = false; // Ocultamos botón de login
                            btnDesbloquear.Visible = true; // Mostramos botón de desbloqueo
                        }
                    }
                    else
                    {
                        // 3. FALLO NORMAL (Contraseña incorrecta, pero aún no bloqueado)
                        lblIncorrecto.Text = "Usuario o contraseña incorrectos";
                        lblIncorrecto.Visible = true;
                        lblIncorrecto.CssClass = "error-message";
                        btnAceptar.Visible = true;
                        btnDesbloquear.Visible = false;
                    }
                }
            }
        }

        // --- MÉTODO MODIFICADO PARA EL BOTÓN DE DESBLOQUEO ---
        protected void btnDesbloquear_Click(object sender, EventArgs e)
        {
            if (conexionDB == null) { conexionDB = (CapaDatos)Application["conexionDB"]; }

            string email = tbxUsuario.Text;
            string password = tbxContraseña.Text;

            // Volvemos a leer el usuario para estar seguros
            Usuario usuarioParaDesbloquear = conexionDB.LeeUsuario(email);

            if (usuarioParaDesbloquear == null)
            {
                // (Manejo de error por si acaso)
                lblIncorrecto.Text = "Error inesperado. Vuelve a intentarlo.";
                lblIncorrecto.CssClass = "error-message";
                lblIncorrecto.Visible = true;
                btnAceptar.Visible = true;
                btnDesbloquear.Visible = false;
                return;
            }

            // Intentamos llamar al método específico de desbloqueo
            // Asumimos que DesbloquearUsuario() ahora devuelve FALSE si está en cooldown.
            if (usuarioParaDesbloquear.DesbloquearUsuario(email, password))
            {
                // ¡Éxito!
                lblIncorrecto.Text = "¡Cuenta desbloqueada! Por favor, inicia sesión.";
                lblIncorrecto.CssClass = "success-message"; // Usamos el estilo verde
                lblIncorrecto.Visible = true;
                btnAceptar.Visible = true; // Mostramos login
                btnDesbloquear.Visible = false; // Ocultamos desbloqueo
            }
            else
            {
                // --- FALLO DE DESBLOQUEO (SEGURIDAD MEJORADA) ---
                // Fallo (la contraseña era incorrecta Y/O se ha reiniciado el cooldown).
                // Volvemos a leer el usuario para obtener el nuevo tiempo de bloqueo.
                usuarioParaDesbloquear = conexionDB.LeeUsuario(email);

                int minutosRestantes = CalcularMinutosRestantes(usuarioParaDesbloquear.BloqueadoHasta);
                lblIncorrecto.Text = $"Contraseña incorrecta. La cuenta sigue bloqueada. Inténtalo en {minutosRestantes} min.";
                lblIncorrecto.Visible = true;
                lblIncorrecto.CssClass = "error-message";

                // --- ¡CAMBIO CLAVE! ---
                // Forzamos al usuario a esperar el nuevo cooldown.
                // "Aceptar" queda visible, "Desbloquear" se oculta.
                btnAceptar.Visible = true;
                btnDesbloquear.Visible = false;
            }
        }
        // --- FIN MÉTODO MODIFICADO ---

        // --- NUEVO MÉTODO HELPER ---
        /// <summary>
        /// Calcula los minutos restantes (redondeado hacia arriba) para un bloqueo.
        /// </summary>
        private int CalcularMinutosRestantes(DateTime? tiempoBloqueo)
        {
            if (!tiempoBloqueo.HasValue) return 0;

            TimeSpan restante = tiempoBloqueo.Value - DateTime.Now;

            if (restante.TotalMinutes < 0) return 0;

            // Math.Ceiling asegura que "1.2 minutos" se muestre como "2 minutos"
            return (int)Math.Ceiling(restante.TotalMinutes);
        }
        // --- FIN NUEVO HELPER ---


        protected void btnRegistrarse_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");

        }

        protected void btnCambiarPassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("CambiarPassword.aspx", true);
        }
    }
}