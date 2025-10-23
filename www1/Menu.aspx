<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="www1.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 33px;
        }
        .auto-style2 {
            height: 33px;
            width: 373px;
            text-align: left;
        }
        .auto-style3 {
            width: 373px;
        }
        .auto-style4 {
            height: 33px;
            width: 398px;
            text-align: justify;
        }
        .auto-style6 {
            height: 33px;
            width: 3179px;
        }
        .auto-style7 {
            width: 3179px;
        }
        .auto-style8 {
            width: 398px;
        }
    </style>
</head>
<body style="width: 1553px; height: 128px; margin-bottom: 10px">
    <form id="form1" runat="server">
        <table style="width:100%;">
            <tr>
                <td class="auto-style1">
                    <asp:Label ID="lblNombreApellidos" runat="server" Text="Label"></asp:Label>
                </td>
                <td class="auto-style6">&nbsp;</td>
                <td class="auto-style4">
                    <asp:Button ID="lblPerfil" runat="server" style="text-align: center" Text="Perfil" Width="122px" />
                </td>
                <td class="auto-style2">
                    <asp:Button ID="lblLogOut" runat="server" Text="Log Out" Width="95px" />
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="auto-style7">&nbsp;</td>
                <td class="auto-style8">&nbsp;</td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="auto-style7">&nbsp;</td>
                <td class="auto-style8">&nbsp;</td>
                <td class="auto-style3">&nbsp;</td>
            </tr>
        </table>
    </form>
</body>
</html>
