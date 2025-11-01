// Usings necesarios para la funcionalidad de la página.
using Datos; // Capa de Acceso a Datos.
using MiLogica.ModeloDatos; // Clase Usuario y modelos.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www1
{
    // Clase code-behind para el formulario de Registro (SignUp).
    public partial class WebForm3 : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioARegistrar; // Variable para almacenar temporalmente el resultado de la búsqueda.

        /// <summary>
        /// Se ejecuta al cargar la página. Inicializa la conexión y el estado de la UI.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // --- Inicialización de la Conexión a la Base de Datos (singleton) ---
                if (conexionDB == null)
                {
                    if (Application["conexionDB"] == null)
                    {
                        // La conexión no existe en Application State, la creamos y la guardamos.
                        conexionDB = new CapaDatos();
                        Application["conexionDB"] = conexionDB;
                    }
                    else
                    {
                        // La conexión ya existe, la recuperamos.
                        conexionDB = (CapaDatos)Application["conexionDB"];
                    }
                }

                usuarioARegistrar = null;
                // Ocultar todos los mensajes de error/éxito en la carga inicial.
                lblContraseñaNoSegura.Visible = false;
                lblEmailEnUsoRegistro.Visible = false;
                lblRegistroCorrecto.Visible = false;
            }
            else
            {
                // En PostBack, solo recuperamos la conexión.
                conexionDB = (CapaDatos)Application["conexionDB"];
            }
        }

        /// <summary>
        /// Manejador del botón Registrarse. Procesa el intento de creación de una nueva cuenta.
        /// </summary>
        protected void btnRegistrarse_Click(object sender, EventArgs e)
        {
            // Ocultar mensajes de éxito antes de empezar las validaciones.
            lblRegistroCorrecto.Visible = false;
            lblContraseñaNoSegura.Visible = false;
            lblEmailEnUsoRegistro.Visible = false;

            if (conexionDB != null)
            {
                // 1. Validación de Unicidad (Email en uso): Intentar leer el usuario.
                usuarioARegistrar = conexionDB.LeeUsuario(tbxEmailRegistro.Text);

                if (usuarioARegistrar != null)
                {
                    // Error: El email ya está en uso.
                    lblEmailEnUsoRegistro.Text = "El correo electrónico ya está en uso.";
                    lblEmailEnUsoRegistro.Visible = true;
                }
                // 2. Validación de Seguridad (Contraseña no segura): Si el email es único, chequear la contraseña.
                else if (MiLogica.Utils.Password.ValidarPassword(tbxPasswordRegistro.Text) == false)
                {
                    // Error: La contraseña no cumple con los requisitos de seguridad.
                    lblContraseñaNoSegura.Text = "La contraseña no cumple los requisitos de seguridad.";
                    lblContraseñaNoSegura.Visible = true;
                }
                else // <-- Lógica de ÉXITO: Email único Y contraseña válida.
                {
                    // 3. Crear el objeto Usuario (Capa de Modelo).
                    Usuario user = new Usuario();
                    user.Email = tbxEmailRegistro.Text;

                    // 4. Encriptar y asignar el hash de la contraseña.
                    user._passwordHash = MiLogica.Utils.Encriptar.EncriptarPasswordSHA256(tbxPasswordRegistro.Text);

                    // 5. Guardar en la base de datos (Capa de Datos).
                    conexionDB.GuardaUsuario(user);

                    // Mensaje de éxito (queda oculto en la redirección, pero útil para depuración).
                    lblRegistroCorrecto.Text = "Registro completado con éxito. Redirigiendo...";
                    lblRegistroCorrecto.Visible = true;

                    // 6. Redirección al Login para que el usuario pueda iniciar sesión con su nueva cuenta.
                    Response.Redirect("Login.aspx");
                }
            }
        }
    }
}
