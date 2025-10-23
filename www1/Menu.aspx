<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="www1.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Mi Menú Principal</title>
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
        /* Estilo para el GridView (opcional, pero recomendado) */
        .gridview-header {
            background-color: #507CD1;
            font-weight: bold;
            color: white;
        }
        .gridview-row {
            background-color: #EFF3FB;
        }
        .gridview-altrow {
            background-color: White;
        }
    </style>
</head>
<body style="width: 1553px; height: 128px; margin-bottom: 10px">
    <form id="form1" runat="server">
        <%-- Encabezado con Nombre y Botones --%>
        <table style="width:100%;">
            <tr>
                <td class="auto-style1">
                    <%-- (Req 1) Nombre y Apellidos --%>
                    <asp:Label ID="lblNombreApellidos" runat="server" Text="Label" Font-Bold="True" Font-Size="Large"></asp:Label>
                </td>
                 <td class="auto-style6">&nbsp;</td>
                <td class="auto-style4">
                    <%-- (Req 2) Botón Perfil con evento OnClick --%>
                    <asp:Button ID="btnPerfil" runat="server" style="text-align: center" Text="Perfil" Width="122px" OnClick="btnPerfil_Click" />
                </td>
                <td class="auto-style2">
                    <%-- (Req 2) Botón Log Out con evento OnClick --%>
                    <asp:Button ID="lblLogOut" runat="server" Text="Log Out" Width="95px" OnClick="lblLogOut_Click" />
                </td>
            </tr>
        </table>
        
        <hr />

        <%-- (Req 3 y 4) Listado central de actividades --%>
        <div style="margin-top: 20px; width: 100%; text-align: center;">
            <h2 style="text-align: center;">Mis Actividades Recientes</h2>
            
            <asp:GridView ID="gvActividades" runat="server" 
                AutoGenerateColumns="False" 
                CellPadding="4" 
                ForeColor="#333333" 
                GridLines="None" 
                Width="90%" 
                HorizontalAlign="Center" 
                EmptyDataText="Aún no has registrado ninguna actividad.">
                
                <AlternatingRowStyle CssClass="gridview-altrow" />
                <HeaderStyle CssClass="gridview-header" />
                <RowStyle CssClass="gridview-row" />
                
                <Columns>
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="Titulo" HeaderText="Título" />
                    <asp:BoundField DataField="Tipo" HeaderText="Tipo de Actividad" />
                    <asp:BoundField DataField="Kms" HeaderText="Kms" DataFormatString="{0:N2} km" />
                    <asp:BoundField DataField="Duracion" HeaderText="Duración" DataFormatString="{0:hh\\:mm\\:ss}" />
                    <asp:BoundField DataField="RitmoMinPorKm" HeaderText="Ritmo (min/km)" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                    <asp:BoundField DataField="FCMedia" HeaderText="FC Media" DataFormatString="{0} bpm" />
                </Columns>

            </asp:GridView>
        </div>

    </form>
</body>
</html>