<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="www1.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Inicio de Sesión</title>
    <style type="text/css">
        body {
            font-family: Arial, sans-serif; /* Fuente más estándar */
            background-color: #f4f4f4; /* Fondo gris claro */
            margin: 0;
            padding: 0;
            display: flex; /* Para centrar el contenedor */
            justify-content: center;
            align-items: center;
            min-height: 100vh; /* Asegura altura completa */
        }
        .login-container {
            background-color: #fff; /* Fondo blanco */
            padding: 30px;
            border-radius: 8px; /* Bordes redondeados */
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); /* Sombra suave */
            width: 400px; /* Ancho fijo */
            text-align: center;
        }
        .login-container h2 {
            margin-bottom: 25px;
            color: #333;
        }
        .form-group {
            margin-bottom: 15px;
            text-align: left; /* Alinea etiquetas a la izquierda */
        }
        .form-group label {
            display: block; /* Etiqueta en línea separada */
            margin-bottom: 5px;
            font-weight: bold;
            color: #555;
        }
        .form-group input[type="text"],
        .form-group input[type="password"] {
            width: calc(100% - 22px); /* Ancho completo menos padding y borde */
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 1em;
        }
        /* Estilo para los controles ASP.NET TextBox */
        .aspNetDisabled, .aspNetDisabled::-webkit-input-placeholder { /*Placeholders*/
            cursor: text;
        }
        .aspNetDisabled, .aspNetDisabled::-moz-placeholder { /*Placeholders*/
            cursor: text;
        }
        .aspNetDisabled, .aspNetDisabled:-ms-input-placeholder { /*Placeholders*/
            cursor: text;
        }
         .aspTextBox {
            width: calc(100% - 22px); /* Ajusta como los input normales */
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 1em;
        }
        .login-button, .register-button {
            width: 100%;
            padding: 12px;
            border: none;
            border-radius: 4px;
            font-size: 1.1em;
            cursor: pointer;
            margin-top: 10px;
        }
        .login-button {
            background-color: #5cb85c; /* Verde */
            color: white;
        }
        .login-button:hover {
            background-color: #4cae4c;
        }
         .register-button {
            background-color: #f0ad4e; /* Naranja */
            color: white;
        }
        .register-button:hover {
            background-color: #ec971f;
        }
        .error-message {
            color: #d9534f; /* Rojo */
            margin-top: 15px;
            font-weight: bold;
        }
        
        /* --- NUEVO ESTILO AÑADIDO --- */
        .success-message {
            color: #5cb85c; /* Verde */
            margin-top: 15px;
            font-weight: bold;
        }

        .register-link {
            margin-top: 20px;
            color: #777;
        }
        .register-link span { /* Para el texto "¿No tienes cuenta?" */
             margin-right: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">

            <%-- Título del formulario --%>
            <h2><asp:Label ID="lblInicioSesion" runat="server" Text="Inicio de Sesión"></asp:Label></h2>

            <%-- Campo Usuario --%>
            <div class="form-group">
                <asp:Label ID="lblUsuario" runat="server" Text="Usuario (Email):" AssociatedControlID="tbxUsuario"></asp:Label>
                <asp:TextBox ID="tbxUsuario" runat="server" CssClass="aspTextBox"></asp:TextBox>
                <%-- Podrías añadir validadores aquí si quisieras --%>
            </div>

            <%-- Campo Contraseña --%>
            <div class="form-group">
                <asp:Label ID="lblContraseña" runat="server" Text="Contraseña:" AssociatedControlID="tbxContraseña"></asp:Label>
                <asp:TextBox ID="tbxContraseña" runat="server" TextMode="Password" CssClass="aspTextBox"></asp:TextBox>
                 <%-- Podrías añadir validadores aquí si quisieras --%>
            </div>

            <%-- Botón de Aceptar --%>
            <div class="form-group">
                <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="login-button" OnClick="btnAceptar_Click" />
            </div>

            <%-- --- NUEVO BOTÓN DE DESBLOQUEO AÑADIDO --- --%>
            <div class="form-group">
                 <asp:Button ID="btnDesbloquear" runat="server" Text="Desbloquear Cuenta" CssClass="register-button" OnClick="btnDesbloquear_Click" Visible="false" CausesValidation="false" />
            </div>

            <%-- Mensaje de Error (Ahora también de éxito) --%>
            <asp:Label ID="lblIncorrecto" runat="server" CssClass="error-message" Visible="false" Text="Usuario o contraseña incorrectos"></asp:Label>

             <%-- Sección de Registro --%>
             <div class="register-link">
                  <asp:Label ID="lblNoTienesCuenta" runat="server" Text="¿No tienes cuenta?"></asp:Label>
                 <asp:Button ID="btnRegistrarse" runat="server" OnClick="btnRegistrarse_Click" Text="Regístrate" CssClass="register-button" CausesValidation="false" />
             </div>

        </div>
    </form>
</body>
</html>