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
    public partial class WebForm3 : System.Web.UI.Page
    {
        private CapaDatos conexionDB;
        private Usuario usuarioARegistrar;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (conexionDB == null)
                {

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

                usuarioARegistrar = null;
                lblContraseñaNoSegura.Visible = false;
                lblEmailEnUsoRegistro.Visible = false;
                lblRegistroCorrecto.Visible = false;
            }
            else
            {
                conexionDB = (CapaDatos)Application["conexionDB"];
            }
        }

        protected void btnRegistrarse_Click(object sender, EventArgs e)
        {
            // Lógica para registrar un nuevo usuario
            if (conexionDB != null)
            {

                lblRegistroCorrecto.Visible = false;

                usuarioARegistrar = conexionDB.LeeUsuario(tbxEmailRegistro.Text);
                if (usuarioARegistrar != null)
                {
                    lblEmailEnUsoRegistro.Text = "El correo electrónico ya está en uso.";
                    lblEmailEnUsoRegistro.Visible = true;
                    return;
                }
                else if (MiLogica.Utils.Password.ValidarPassword(tbxPasswordRegistro.Text) == false)
                {
                    lblContraseñaNoSegura.Text = "La contraseña no cumple los requisitos de seguridad.";
                    lblContraseñaNoSegura.Visible = true;
                    return;

                }
                {
                    Usuario user = new Usuario();
                    user.Email = tbxEmailRegistro.Text;
                    user._passwordHash = MiLogica.Utils.Encriptar.EncriptarPasswordSHA256(tbxPasswordRegistro.Text);
                    conexionDB.GuardaUsuario(user);
                    lblRegistroCorrecto.Text = "Registro completado con éxito. Redirigiendo al inicio de sesión...";
                    lblRegistroCorrecto.Visible = true;
                }
            }
            string script = "setTimeout(function(){ window.location = 'Login.aspx'; }, 2500);";
            ScriptManager.RegisterStartupScript(this, GetType(), "RedirigirLogin", script, true);

            // Por ejemplo, validar los datos ingresados y guardarlos en la base de datos
            // Después de registrar, redirigir al usuario a la página de inicio de sesión
            /*
            System.Threading.Thread.Sleep(1500); // Simula un retardo para ver el mensaje antes de la transferencia
            Server.Transfer("Login.aspx", true);
            */
        }
    }
}