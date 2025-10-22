<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CambiarPassword.aspx.cs" Inherits="www1.WebForm4" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 52px;
        }
        .auto-style2 {
            height: 52px;
            width: 130px;
        }
        .auto-style6 {
            height: 52px;
            width: 214px;
        }
        .auto-style8 {
            width: 130px;
            height: 42px;
        }
        .auto-style11 {
            height: 42px;
        }
        .auto-style12 {
            height: 52px;
            width: 305px;
        }
        .auto-style13 {
            width: 305px;
            height: 42px;
        }
        .auto-style14 {
            width: 214px;
            height: 42px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <table style="width: 100%;">
            <tr>
                <td class="auto-style2"></td>
                <td class="auto-style12">
                    <asp:Label ID="lblCambiaTuPassword" runat="server" BorderStyle="Groove" style="text-align: center" Text="Cambia tu contraseña" Width="462px"></asp:Label>
                </td>
                <td class="auto-style6">&nbsp;</td>
                <td class="auto-style1"></td>
            </tr>
            <tr>
                <td class="auto-style8"></td>
                <td class="auto-style13">
                    <asp:Label ID="lblIntroducePasswordActual" runat="server" Text="Introduce la contraseña actual:"></asp:Label>
                </td>
                <td class="auto-style14">
                    <asp:TextBox ID="tbxPasswordActual" runat="server"></asp:TextBox>
                </td>
                <td class="auto-style11">
                    <asp:Label ID="lblErrorPasswordActualCambiarPassword" runat="server" Text="Esta contraseña no coincide con la actual"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style8"></td>
                <td class="auto-style13">
                    <asp:Label ID="lblIntroduceLaNuevaContraseña" runat="server" Text="Introduce la nueva contraseña:"></asp:Label>
                </td>
                <td class="auto-style14">
                    <asp:TextBox ID="tbxNuevoPassword" runat="server"></asp:TextBox>
                </td>
                <td class="auto-style11">
                    <asp:Label ID="lblNuevoPasswordCambiarContraseña" runat="server" Text="Prueba con una contraseña más segura"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="auto-style8"></td>
                <td class="auto-style13">
                    <asp:Label ID="lblConfirmaNuevoPassword" runat="server" Text="Confirma la nueva contraseña:"></asp:Label>
                </td>
                <td class="auto-style14">
                    <asp:TextBox ID="tbxConfirmarPassword" runat="server"></asp:TextBox>
                </td>
                <td class="auto-style11">
                    <asp:Label ID="lblConfirmarPasswordNoCoincide" runat="server" Text="No coincide con la nueva contraseña"></asp:Label>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
