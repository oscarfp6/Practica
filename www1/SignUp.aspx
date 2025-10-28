<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="www1.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registro de Usuario</title>
    <style type="text/css">
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }
        .signup-container {
            background-color: #fff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            width: 450px; /* Un poco más ancho para los mensajes */
            text-align: center;
        }
        .signup-container h2 {
            margin-bottom: 25px;
            color: #333;
        }
        .form-group {
            margin-bottom: 15px;
            text-align: left;
            position: relative; /* Para posicionar mensajes de error al lado */
            padding-right: 150px; /* Espacio para mensajes */
        }
        .form-group label {
            display: inline-block; /* O block si prefieres encima */
            width: 100px; /* Ancho fijo para etiquetas */
            margin-right: 10px;
            font-weight: bold;
            color: #555;
            vertical-align: middle;
        }
         /* Estilo para los controles ASP.NET TextBox */
        .aspTextBox {
            width: calc(100% - 132px); /* Ajusta según el label */
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 1em;
            display: inline-block;
            vertical-align: middle;
        }
        .signup-button {
            width: 100%;
            padding: 12px;
            border: none;
            border-radius: 4px;
            font-size: 1.1em;
            cursor: pointer;
            margin-top: 10px;
            background-color: #5cb85c; /* Verde */
            color: white;
        }
        .signup-button:hover {
            background-color: #4cae4c;
        }
        .message-label {
            display: inline-block; /* Para ponerlo al lado del input */
            font-size: 0.9em;
            font-weight: bold;
            margin-left: 10px;
            position: absolute; /* Posicionarlo respecto al form-group */
            right: 0;
            top: 50%;
            transform: translateY(-50%);
            width: 140px; /* Ancho del mensaje */
        }
        .error-message {
            color: #d9534f; /* Rojo */
        }
        .success-message {
             color: #5cb85c; /* Verde */
        }


    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="signup-container">

             <%-- Título --%>
             <h2><asp:Label ID="lblRegistrate" runat="server" Text="Regístrate"></asp:Label></h2>

             <%-- Campo Email --%>
            <div class="form-group">
                <asp:Label ID="lblEmailRegistro" runat="server" Text="Email:" AssociatedControlID="tbxEmailRegistro"></asp:Label>
                <asp:TextBox ID="tbxEmailRegistro" runat="server" CssClass="aspTextBox" TextMode="Email"></asp:TextBox>
                <asp:Label ID="lblEmailEnUsoRegistro" runat="server" Text="Email ya en uso" CssClass="message-label error-message" AssociatedControlID="tbxEmailRegistro"></asp:Label>
                <%-- Añadir RequiredFieldValidator y RegularExpressionValidator si se desea --%>
            </div>

            <%-- Campo Contraseña --%>
            <div class="form-group">
                <asp:Label ID="lblPasswordRegistro" runat="server" Text="Contraseña:" AssociatedControlID="tbxPasswordRegistro"></asp:Label>
                <asp:TextBox ID="tbxPasswordRegistro" runat="server" TextMode="Password" CssClass="aspTextBox"></asp:TextBox>
                <asp:Label ID="lblContraseñaNoSegura" runat="server" Text="Contraseña no segura" CssClass="message-label error-message" AssociatedControlID="tbxPasswordRegistro"></asp:Label>
                 <%-- Añadir RequiredFieldValidator si se desea --%>
            </div>

            <%-- Botón Confirmar --%>
             <div class="form-group" style="text-align: center; padding-right: 0;">
                 <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar Registro" CssClass="signup-button" OnClick="btnRegistrarse_Click" />
            </div>

             <%-- Mensaje de Éxito --%>
             <div style="text-align: center; margin-top: 15px;">
                 <asp:Label ID="lblRegistroCorrecto" runat="server" EnableViewState="False" CssClass="message-label success-message" style="position: static; transform: none; width: auto;" Text="Registro completado satisfactoriamente"></asp:Label>
            </div>

             <%-- Podrías añadir un ValidationSummary aquí también --%>
             <%-- <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="error-message" HeaderText="Por favor, corrige los errores:" /> --%>

        </div>
    </form>
</body>
</html>