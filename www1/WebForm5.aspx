<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm5.aspx.cs" Inherits="www1.WebForm5" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style2 {
            text-align: center;
        }
        .auto-style3 {
            width: 1532px;
            text-align: center;
        }
        .auto-style5 {
            width: 825px;
            text-align: center;
        }
        .auto-style6 {
            width: 778px;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="width:100%;">
                <tr>
                    <td class="auto-style6">&nbsp;</td>
                    <td class="auto-style3" colspan="4">
                        <asp:Label ID="lblRegistraTuNuevaActividad" runat="server" style="font-weight: 700; color: #000000; background-color: #FFFFFF" Text="Registra tu nueva actividad:"></asp:Label>
                    </td>
                    <td class="auto-style5">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="3">
                        <asp:Label ID="lblTitulo" runat="server" Text="Título:"></asp:Label>
                    </td>
                    <td class="auto-style2" colspan="3">
                        <asp:TextBox ID="tbxTitulo" runat="server"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="3">
                        <asp:Label ID="lblTipo" runat="server" Text="Tipo de actividad:"></asp:Label>
                    </td>
                    <td class="auto-style2" colspan="3">
                        <asp:TextBox ID="tbxTipoActividad" runat="server" TextMode="Range"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="3">
                        <asp:Label ID="lblDuracion" runat="server" Text="Duración:"></asp:Label>
                    </td>
                    <td class="auto-style2" colspan="3">
                        <asp:TextBox ID="tbxDuracion" runat="server" TextMode="Time"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="3">
                        <asp:Label ID="lblDescripcion" runat="server" Text="Descripción:"></asp:Label>
                    </td>
                    <td class="auto-style2" colspan="3">
                        <asp:TextBox ID="tbxDescripcion" runat="server"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="3">
                        <asp:Label ID="lblKms" runat="server" Text="Kms:"></asp:Label>
                    </td>
                    <td class="auto-style2" colspan="3">
                        <asp:TextBox ID="tbxKms" runat="server" TextMode="Number"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="3">
                        <asp:Label ID="lblFecha" runat="server" Text="Fecha"></asp:Label>
                    </td>
                    <td class="auto-style2" colspan="3">
                        <asp:TextBox ID="tbxFecha" runat="server" TextMode="Date"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="3">
                        <asp:Label ID="lblMetrosDesnivel" runat="server" Text="Metros de desnivel:"></asp:Label>
                    </td>
                    <td class="auto-style2" colspan="3">
                        <asp:TextBox ID="tbxMetrosDesnivel" runat="server"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="3">
                        <asp:Label ID="lblFcMedia" runat="server" Text="Frecuencia cardiaca media:"></asp:Label>
                    </td>
                    <td class="auto-style2" colspan="3">
                        <asp:TextBox ID="tbxFcMedia" runat="server"></asp:TextBox>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style2" colspan="2">&nbsp;</td>
                    <td class="auto-style2" colspan="2">
                        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" />
                    </td>
                    <td class="auto-style2" colspan="2">&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
