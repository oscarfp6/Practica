<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CambiarPassword.aspx.cs" Inherits="www1.WebForm4" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Cambiar Contraseña</title>
    <style type="text/css">
        /* --- Estilos base similares a Login/SignUp --- */
        body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; display: flex; justify-content: center; align-items: center; min-height: 100vh; }
        .container-box { background-color: #fff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); width: 450px; text-align: center; }
        .container-box h2 { margin-bottom: 25px; color: #333; }
        .form-group { margin-bottom: 15px; text-align: left; position: relative; } /* Added position relative */
        .form-group label { display: block; margin-bottom: 5px; font-weight: bold; color: #555; }
        .aspTextBox { width: calc(100% - 22px); padding: 10px; border: 1px solid #ccc; border-radius: 4px; font-size: 1em; }
        .btn { width: 100%; padding: 12px; border: none; border-radius: 4px; font-size: 1.1em; cursor: pointer; margin-top: 10px; }
        .btn-primary { background-color: #007bff; color: white; }
        .btn-primary:hover { background-color: #0056b3; }
        .btn-secondary { background-color: #6c757d; color: white; margin-top: 5px; /* Less margin for cancel */}
        .btn-secondary:hover { background-color: #5a6268; }
        .message-label { /* General messages */
            display: block; /* Make it take full width below button */
            margin-top: 15px;
            font-weight: bold;
            min-height: 1.2em; /* Reserve space */
        }
        .error-message { color: #d9534f; /* Red */}
        .success-message { color: #5cb85c; /* Green */}
        .validation-error { /* Specific validator messages */
            color: red;
            font-size: 0.9em;
            display: block; /* Below the textbox */
            margin-top: 3px;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-box">

            <h2><asp:Label ID="lblCambiaTuPassword" runat="server" Text="Cambiar Contraseña"></asp:Label></h2>

            <%-- Contraseña Actual --%>
            <div class="form-group">
                <asp:Label ID="lblIntroducePasswordActual" runat="server" Text="Contraseña Actual:" AssociatedControlID="tbxPasswordActual"></asp:Label>
                <asp:TextBox ID="tbxPasswordActual" runat="server" TextMode="Password" CssClass="aspTextBox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPasswordActual" runat="server" ControlToValidate="tbxPasswordActual"
                    ErrorMessage="La contraseña actual es obligatoria." CssClass="validation-error" Display="Dynamic">* Obligatorio</asp:RequiredFieldValidator>
                <%-- Mensaje de error específico para contraseña actual incorrecta --%>
                <asp:Label ID="lblErrorPasswordActual" runat="server" CssClass="validation-error" Visible="false" Text="La contraseña actual no es correcta."></asp:Label>
            </div>

            <%-- Nueva Contraseña --%>
            <div class="form-group">
                <asp:Label ID="lblIntroduceLaNuevaContraseña" runat="server" Text="Nueva Contraseña:" AssociatedControlID="tbxNuevoPassword"></asp:Label>
                <asp:TextBox ID="tbxNuevoPassword" runat="server" TextMode="Password" CssClass="aspTextBox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNuevoPassword" runat="server" ControlToValidate="tbxNuevoPassword"
                    ErrorMessage="La nueva contraseña es obligatoria." CssClass="validation-error" Display="Dynamic">* Obligatorio</asp:RequiredFieldValidator>
                 <%-- Mensaje de error específico para contraseña no segura --%>
                <asp:Label ID="lblErrorNuevoPassword" runat="server" CssClass="validation-error" Visible="false" Text="La contraseña no cumple los requisitos de seguridad."></asp:Label>
            </div>

            <%-- Confirmar Nueva Contraseña --%>
            <div class="form-group">
                <asp:Label ID="lblConfirmaNuevoPassword" runat="server" Text="Confirmar Nueva Contraseña:" AssociatedControlID="tbxConfirmarPassword"></asp:Label>
                <asp:TextBox ID="tbxConfirmarPassword" runat="server" TextMode="Password" CssClass="aspTextBox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvConfirmarPassword" runat="server" ControlToValidate="tbxConfirmarPassword"
                    ErrorMessage="La confirmación es obligatoria." CssClass="validation-error" Display="Dynamic">* Obligatorio</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvConfirmarPassword" runat="server" ControlToValidate="tbxConfirmarPassword" ControlToCompare="tbxNuevoPassword" Operator="Equal" Type="String"
                    ErrorMessage="Las contraseñas nuevas no coinciden." CssClass="validation-error" Display="Dynamic">* No coincide</asp:CompareValidator>
            </div>

            <%-- Botón Confirmar --%>
            <div class="form-group" style="text-align: center;">
                 <asp:Button ID="btnConfirmarCambioPassword" runat="server" Text="Confirmar Cambio" CssClass="btn btn-primary" OnClick="btnConfirmarCambioPassword_Click" />
            </div>

            <%-- Mensaje General (Resultado Éxito/Error General) --%>
            <asp:Label ID="lblResultado" runat="server" CssClass="message-label" Visible="false"></asp:Label>

            <%-- Botón Cancelar/Volver --%>
             <div class="form-group" style="text-align: center;">
                 <asp:Button ID="btnCancelar" runat="server" Text="Cancelar y Volver al Perfil" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" CausesValidation="false" />
            </div>

             <%-- Resumen de Validación --%>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="error-message" HeaderText="Por favor, corrige los siguientes errores:" ShowMessageBox="false" ShowSummary="true"/>

        </div>
    </form>
</body>
</html>