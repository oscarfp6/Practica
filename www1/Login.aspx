<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="www1.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style2 {
            width: 144px;
        }
        .auto-style3 {
            width: 178px;
        }
        .auto-style4 {
            width: 329px;
        }
        .auto-style5 {
            text-align: center;
        }
        .auto-style6 {
            width: 105px;
        }
        .auto-style7 {
            width: 178px;
            height: 33px;
        }
        .auto-style8 {
            width: 105px;
            height: 33px;
        }
        .auto-style9 {
            width: 144px;
            height: 33px;
        }
        .auto-style10 {
            width: 329px;
            height: 33px;
        }
        .auto-style11 {
            height: 33px;
        }
        .auto-style12 {
            width: 178px;
            height: 58px;
        }
        .auto-style13 {
            height: 58px;
        }
        .auto-style14 {
            width: 178px;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%;">
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style6">&nbsp;</td>
                    <td class="auto-style2">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style5" colspan="3">
                        <asp:Label ID="lblInicioSesion" runat="server" Text="Inicio Sesion"></asp:Label>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style6">&nbsp;</td>
                    <td class="auto-style2">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style6">&nbsp;</td>
                    <td class="auto-style2">
                        <asp:Label ID="lblUsuario" runat="server" Text="Usuario"></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:TextBox ID="tbxUsuario" runat="server"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style6">&nbsp;</td>
                    <td class="auto-style2">
                        <asp:Label ID="lblContraseña" runat="server" Text="Contraseña: "></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:TextBox ID="tbxContraseña" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style6">&nbsp;</td>
                    <td class="auto-style2">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style7"></td>
                    <td class="auto-style8"></td>
                    <td class="auto-style9">
                        <asp:Button ID="btnCambiarContraseña" runat="server" Text="Cambiar Contraseña" Width="200px" />
                    </td>
                    <td class="auto-style10">
                        <asp:Button ID="btnAceptar" runat="server" Text="Aceptar" Width="200px" OnClick="btnAceptar_Click" />

                    </td>
                    <td class="auto-style11">&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style12"></td>
                    <td class="auto-style13" colspan="3"></td>
                    <td class="auto-style13"></td>
                </tr>
                <tr>
                    <td class="auto-style14">&nbsp;</td>
                    <td class="auto-style5" colspan="3">
                        <asp:Label ID="lblIncorrecto" runat="server" Text="Usuario o contraseña incorrectos"></asp:Label>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style6">&nbsp;</td>
                    <td class="auto-style2">&nbsp;</td>
                    <td class="auto-style4">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
