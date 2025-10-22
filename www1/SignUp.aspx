<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="www1.WebForm1" %>

 <!DOCTYPE html>

 <html xmlns="http://www.w3.org/1999/xhtml">
 <head runat="server">
 <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <title>Inicio de Sesión</title>
     <link href="Estilos/LoginStyles.css" rel="stylesheet" type="text/css" />
     <%-- Eliminado el bloque <style> original --%>
 </head>
 <body>
     <form id="form1" runat="server">
         <%-- Contenedor principal para centrar y estilizar el fondo --%>
         <div class="login-container">
             <%-- Tabla original --%>
             <table style="width: 100%;">
                 <tr>
                     <%-- Eliminadas clases auto-style --%>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td style="text-align: center;">&nbsp;</td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <td>&nbsp;</td>
                     <%-- Título --%>
                     <td colspan="3" style="text-align: center;">
                         <h1><asp:Label ID="lblInicioSesion" runat="server" Text="Inicio de Sesión"></asp:Label></h1>
                     </td>
                     <%-- Etiqueta Registro --%>
                     <td style="text-align: center;">
                         <asp:Label ID="lblNoTienesCuenta" runat="server" Text="¿No tienes cuenta?"></asp:Label>
                     </td>
                     <td style="text-align: center;">&nbsp;</td>
                 </tr>
                 <tr>
                     <%-- Fila vacía debajo del título --%>
                     <td></td>
                     <td></td>
                     <td></td>
                     <td></td>
                     <%-- Botón Registrarse --%>
                     <td style="text-align: center; height: 33px;">
                         <asp:Button ID="btnRegistrarse" runat="server" OnClick="btnRegistrarse_Click" Text="Regístrate" CssClass="register-button" Width="200px" />
                     </td>
                     <td style="height: 33px;"></td>
                 </tr>
                 <tr>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <%-- Etiqueta Usuario --%>
                     <td style="width: 144px; text-align: right; padding-right: 10px;">
                         <asp:Label ID="lblUsuario" runat="server" Text="Usuario (Email)" CssClass="login-label"></asp:Label>
                     </td>
                     <%-- TextBox Usuario --%>
                     <td style="width: 278px; text-align: left;">
                         <asp:TextBox ID="tbxUsuario" runat="server" CssClass="login-textbox" placeholder="tu@email.com"></asp:TextBox>
                     </td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <%-- Etiqueta Contraseña --%>
                     <td style="width: 144px; text-align: right; padding-right: 10px;">
                         <asp:Label ID="lblContraseña" runat="server" Text="Contraseña:" CssClass="login-label"></asp:Label>
                     </td>
                     <%-- TextBox Contraseña --%>
                     <td style="width: 278px; text-align: left;">
                         <asp:TextBox ID="tbxContraseña" runat="server" TextMode="Password" CssClass="login-textbox"></asp:TextBox>
                     </td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <%-- Fila vacía --%>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <%-- Fila botón Aceptar --%>
                     <td></td>
                     <td></td>
                     <td>&nbsp;</td>
                     <td style="height: 33px; text-align: center;"> <%-- Celda botón centrado --%>
                         <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" CssClass="login-button" OnClick="btnAceptar_Click" Width="200px" />
                     </td>
                     <td style="height: 33px; text-align: center;">&nbsp;</td>
                     <td style="height: 33px;">&nbsp;</td>
                 </tr>
                 <tr>
                     <%-- Fila vacía --%>
                     <td style="height: 58px;"></td>
                     <td style="height: 58px;"></td>
                     <td colspan="3" style="height: 58px;"></td>
                     <td style="height: 58px;"></td>
                     <td style="height: 58px;"></td>
                 </tr>
                 <tr>
                     <%-- Fila Mensaje Error --%>
                     <td style="text-align: center;">&nbsp;</td>
                     <td colspan="3" style="text-align: center;">
                         <asp:Label ID="lblIncorrecto" runat="server" Text="Usuario o contraseña incorrectos" CssClass="message-label error-message" Visible="false"></asp:Label>
                     </td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                 </tr>
                 <tr>
                     <%-- Fila vacía --%>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                     <td>&nbsp;</td>
                 </tr>
             </table>
         </div>
     </form>
 </body>
 </html>