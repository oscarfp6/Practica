<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="www1.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style2 {
            height: 45px;
        }
        .auto-style3 {
            height: 45px;
            width: 182px;
        }
        .auto-style4 {
            width: 182px;
            height: 53px;
        }
        .auto-style5 {
            height: 58px;
        }
        .auto-style6 {
            width: 182px;
            height: 58px;
        }
        .auto-style7 {
            height: 53px;
        }
        .auto-style8 {
            height: 58px;
            width: 418px;
        }
        .auto-style9 {
            height: 58px;
            width: 34px;
        }
        .auto-style10 {
            height: 45px;
            width: 188px;
        }
        .auto-style11 {
            height: 58px;
            width: 188px;
        }
        .auto-style12 {
            height: 53px;
            width: 188px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%; height: 137px;">
                <tr>
                    <td class="auto-style2"></td>
                    <td class="auto-style3">&nbsp;</td>
                    <td class="auto-style2" colspan="2">
                        <asp:Label ID="lblRegistrate" runat="server" CssClass="auto-style2" style="text-align: center" Text="Regístrate"></asp:Label>
                    </td>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style2"></td>
                </tr>
                <tr>
                    <td class="auto-style5"></td>
                    <td class="auto-style6"></td>
                    <td class="auto-style8">
                        <asp:Label ID="lblEmailRegistro" runat="server" Text="Email"></asp:Label>
                    </td>
                    <td class="auto-style9">
                        <asp:TextBox ID="tbxEmailRegistro" runat="server"></asp:TextBox>
                    </td>
                    <td class="auto-style11">
                        <asp:Label ID="lblEmailEnUsoRegistro" runat="server" Text="Email ya asociado a otra cuenta"></asp:Label>
                    </td>
                    <td class="auto-style5"></td>
                </tr>
                <tr>
                    <td class="auto-style5"></td>
                    <td class="auto-style6"></td>
                    <td class="auto-style8">
                        <asp:Label ID="lblPasswordRegistro" runat="server" Text="Contraseña"></asp:Label>
                    </td>
                    <td class="auto-style9">
                        <asp:TextBox ID="tbxPasswordRegistro" runat="server"></asp:TextBox>
                    </td>
                    <td class="auto-style11">
                        <asp:Label ID="lblContraseñaNoSegura" runat="server" Text="Prueba con una contraseña más segura"></asp:Label>
                    </td>
                    <td class="auto-style5"></td>
                </tr>
                <tr>
                    <td class="auto-style7"></td>
                    <td class="auto-style4"></td>
                    <td class="auto-style7" colspan="2">&nbsp;</td>
                    <td class="auto-style12">&nbsp;</td>
                    <td class="auto-style7"></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
