<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="www1.Perfil" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Perfil de Usuario</title>
    <style type="text/css">
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f9; display: flex; justify-content: center; align-items: center; min-height: 100vh; margin: 0; }
        .perfil-box { background-color: #fff; padding: 40px; border-radius: 10px; box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15); width: 450px; text-align: center; }
        .form-title { font-size: 28px; color: #1a1a1a; margin-bottom: 30px; font-weight: 600; display: block; }
        .form-group { margin-bottom: 15px; text-align: left; }
        .form-group label { display: block; margin-bottom: 6px; font-weight: 500; color: #555; font-size: 14px; }
        /* Estilo general para TextBox */
        .aspTextBox { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 5px; box-sizing: border-box; font-size: 16px; transition: border-color 0.3s; }
        .aspTextBox:focus { border-color: #007bff; outline: none; }
        .aspTextBox[readonly] { background-color: #eee; cursor: not-allowed; } /* Estilo para ReadOnly */
        .btn { width: 100%; padding: 12px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; font-weight: bold; margin-top: 10px; transition: background-color 0.3s; }
        .btn-primary { background-color: #007bff; color: white; }
        .btn-primary:hover { background-color: #0056b3; }
        .btn-secondary { background-color: #6c757d; color: white; }
        .btn-secondary:hover { background-color: #5a6268; }
        .btn-warning { background-color: #ffc107; color: #212529; margin-top:20px;} /* Botón cambiar contraseña */
        .btn-warning:hover { background-color: #e0a800; }
        .message-success { color: green; font-weight: bold; margin-top: 15px; min-height: 1.2em; /* Espacio para mensaje */}
        .message-error { color: red; font-weight: bold; margin-top: 15px; min-height: 1.2em; /* Espacio para mensaje */}
        .validation-error { color: red; font-size: 0.9em; display: block; /* Para que aparezca debajo */}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="perfil-box">

            <asp:Label ID="lblTitulo" runat="server" CssClass="form-title" Text="Editar Perfil"></asp:Label>

            <div class="form-group">
                <asp:Label ID="Label1" runat="server" Text="Nombre:" AssociatedControlID="tbxNombre"></asp:Label>
                <asp:TextBox ID="tbxNombre" runat="server" CssClass="aspTextBox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="tbxNombre"
                    ErrorMessage="El nombre es obligatorio." CssClass="validation-error" Display="Dynamic">* Obligatorio</asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <asp:Label ID="Label2" runat="server" Text="Apellidos:" AssociatedControlID="tbxApellidos"></asp:Label>
                <asp:TextBox ID="tbxApellidos" runat="server" CssClass="aspTextBox"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="rfvApellidos" runat="server" ControlToValidate="tbxApellidos"
                    ErrorMessage="Los apellidos son obligatorios." CssClass="validation-error" Display="Dynamic">* Obligatorio</asp:RequiredFieldValidator>
            </div>

            <%-- NUEVOS CAMPOS: Edad y Peso --%>
             <div class="form-group">
                <asp:Label ID="Label4" runat="server" Text="Edad (años):" AssociatedControlID="tbxEdad"></asp:Label>
                <asp:TextBox ID="tbxEdad" runat="server" CssClass="aspTextBox" TextMode="Number"></asp:TextBox> <%-- Usar TextMode Number para navegadores compatibles --%>
                <asp:CompareValidator ID="cvEdad" runat="server" ControlToValidate="tbxEdad" Operator="DataTypeCheck" Type="Integer"
                     ErrorMessage="La edad debe ser un número entero." CssClass="validation-error" Display="Dynamic">* Debe ser un número entero</asp:CompareValidator>
                 <asp:RangeValidator ID="rvEdad" runat="server" ControlToValidate="tbxEdad" Type="Integer" MinimumValue="0" MaximumValue="120"
                     ErrorMessage="La edad debe estar entre 0 y 120." CssClass="validation-error" Display="Dynamic">* Edad entre 0 y 120</asp:RangeValidator>
            </div>

             <div class="form-group">
                <asp:Label ID="Label5" runat="server" Text="Peso (kg):" AssociatedControlID="tbxPeso"></asp:Label>
                <asp:TextBox ID="tbxPeso" runat="server" CssClass="aspTextBox" placeholder="Ej: 75.5"></asp:TextBox>
                 <asp:CompareValidator ID="cvPeso" runat="server" ControlToValidate="tbxPeso" Operator="DataTypeCheck" Type="Double"
                     ErrorMessage="El peso debe ser un número (use '.' para decimales)." CssClass="validation-error" Display="Dynamic">* Debe ser un número</asp:CompareValidator>
                  <asp:RangeValidator ID="rvPeso" runat="server" ControlToValidate="tbxPeso" Type="Double" MinimumValue="0" MaximumValue="500"
                     ErrorMessage="El peso debe ser un valor positivo y razonable." CssClass="validation-error" Display="Dynamic">* Peso entre 0 y 500</asp:RangeValidator>
            </div>
            <%-- FIN NUEVOS CAMPOS --%>

            <div class="form-group">
                <asp:Label ID="Label3" runat="server" Text="Email:" AssociatedControlID="tbxEmail"></asp:Label>
                <asp:TextBox ID="tbxEmail" runat="server" ReadOnly="true" CssClass="aspTextBox"></asp:TextBox> <%-- Email no editable, aplicamos estilo readonly --%>
            </div>

            <%-- Suscripción eliminada según requisitos --%>
            <%-- <div class="form-group" style="text-align: center;">
                <asp:CheckBox ID="chkSuscripcion" runat="server" Text=" Recibir notificaciones por correo electrónico" />
            </div> --%>

            <asp:Label ID="lblMensaje" runat="server" Text="" CssClass="message-success" Visible="false"></asp:Label> <%-- Oculto inicialmente --%>

            <asp:Button ID="btnGuardarPerfil" runat="server" Text="Guardar Cambios" CssClass="btn btn-primary" OnClick="btnGuardarPerfil_Click" />

            <%-- NUEVO BOTÓN: Cambiar Contraseña --%>
            <asp:Button ID="btnIrACambiarPassword" runat="server" Text="Cambiar Contraseña" CssClass="btn btn-warning" OnClick="btnIrACambiarPassword_Click" CausesValidation="false"/>

            <asp:Button ID="btnVolver" runat="server" Text="Volver al Menú" CssClass="btn btn-secondary" OnClick="btnVolver_Click" CausesValidation="false" />

            <%-- Resumen de validación (opcional pero recomendado) --%>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="message-error" HeaderText="Por favor, corrige los siguientes errores:" ShowMessageBox="false" ShowSummary="true"/>

        </div>
    </form>
</body>
</html>