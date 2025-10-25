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
                usuarioAutenticado = conexionDB.LeeUsuario(tbxUsuario.Text);
                if (usuarioAutenticado != null && usuarioAutenticado.PermitirLogin(tbxContraseña.Text))
                {
                    Session["usuarioautenticado"] = usuarioAutenticado;
                    lblIncorrecto.Text = string.Empty;
                    if (usuarioAutenticado.Id==1)
                    {
                        Server.Transfer("MenuAdmin.aspx", true);
                    } else
                    {
                        Server.Transfer("Menu.aspx", true);
                    }
                }
                else
                {
                    if(usuarioAutenticado.Estado == EstadoUsuario.Bloqueado)
                    {
                        lblIncorrecto.Text = "Usuario bloqueado. Contacte con el administrador.";
                        lblIncorrecto.Visible = true;
                        return;
                    }
                    usuarioAutenticado = null;
                    lblIncorrecto.Text = "Usuario o contraseña incorrectos";
                    lblIncorrecto.Visible = true;
                }
            }
        }

        protected void btnRegistrarse_Click(object sender, EventArgs e)
        {
            Server.Transfer("SignUp.aspx", true);

        }

        protected void btnCambiarPassword_Click(object sender, EventArgs e)
        {
            Server.Transfer("CambiarPassword.aspx", true);
        }
    }
}