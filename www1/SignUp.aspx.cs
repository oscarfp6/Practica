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
            if (conexionDB != null)
            {
                lblRegistroCorrecto.Visible = false;
                usuarioARegistrar = conexionDB.LeeUsuario(tbxEmailRegistro.Text);

                if (usuarioARegistrar != null)
                {
                    lblEmailEnUsoRegistro.Text = "El correo electrónico ya está en uso.";
                    lblEmailEnUsoRegistro.Visible = true;
                }
                else if (MiLogica.Utils.Password.ValidarPassword(tbxPasswordRegistro.Text) == false)
                {
                    lblContraseñaNoSegura.Text = "La contraseña no cumple los requisitos de seguridad.";
                    lblContraseñaNoSegura.Visible = true;
                }
                else // <-- AÑADIR ESTE ELSE
                {
                    // Solo si el email NO existe Y la contraseña ES válida
                    Usuario user = new Usuario();
                    user.Email = tbxEmailRegistro.Text;
                    user._passwordHash = MiLogica.Utils.Encriptar.EncriptarPasswordSHA256(tbxPasswordRegistro.Text);
                    conexionDB.GuardaUsuario(user);

                    // Esta línea ya no es visible para el usuario,
                    // pero la dejamos por si la necesitas en el futuro.
                    lblRegistroCorrecto.Text = "Registro completado con éxito. Redirigiendo...";
                    lblRegistroCorrecto.Visible = true;

                    // Redirección inmediata del lado del servidor
                    Response.Redirect("Login.aspx");
                }
            }
        }
    }
}