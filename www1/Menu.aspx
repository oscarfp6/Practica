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
        .auto-style4 {
            height: 33px;
            width: 398px;
            text-align: justify;
        }
        .auto-style6 {
            height: 33px;
            width: 2720px;
            text-align: right;
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
        #form1 {
            text-align: center;
        }
        .auto-style7 {
            height: 33px;
            width: 2190px;
            text-align: right;
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
                 <td class="auto-style7">&nbsp;</td>
                 <td class="auto-style6">
                     <asp:Button ID="btnRegistrarActividad" runat="server" Text="Registrar Actividad" OnClick="btnRegistrarActividad_Click" Width="288px" />
                </td>
                 <td class="auto-style6">&nbsp;</td>
                <td class="auto-style4">
                    <%-- (Req 2) Botón Perfil con evento OnClick --%>
                    <asp:Button ID="btnPerfil" runat="server" style="text-align: center" Text="Perfil" Width="122px" OnClick="btnPerfil_Click" />
                </td>
                <td class="auto-style2">
                    <%-- (Req 2) Botón Log Out con evento OnClick --%>
                    <asp:Button ID="btnLogOut" runat="server" Text="Log Out" Width="95px"  OnClick="btnLogOut_Click" />
                </td>
            </tr>
        </table>
        
        <hr />

        <%-- (Req 3 y 4) Listado central de actividades --%>
        <div style="margin-top: 20px ; width: 100%; text-align: center;">
            
<div style="margin-top: 20px; width: 90%; margin-left: auto; margin-right: auto; text-align: left;">
    <h2 style="text-align: center;">Mis Actividades Recientes</h2>

    <asp:Repeater ID="rptActividades" runat="server" OnItemCommand="rptActividades_ItemCommand">
        
        <HeaderTemplate>
            <div style="background-color: #507CD1; color: white; font-weight: bold; padding: 10px; display: grid; grid-template-columns: 2fr 1fr 1fr 1fr 3fr; gap: 10px;">
                <div>Título</div>
                <div>Fecha</div>
                <div>Tipo</div>
                <div>Distancia</div>
                <div>Descripción</div>
            </div>
        </HeaderTemplate>

        <ItemTemplate>
            <div style="background-color: #EFF3FB; padding: 10px; border-bottom: 1px solid #CCC; display: grid; grid-template-columns: 2fr 1fr 1fr 1fr 3fr; gap: 10px;">
                
                <div>
                    <strong><%# Eval("Titulo") %></strong>
                </div>
                
                <div>
                    <%# ((DateTime)Eval("Fecha")).ToString("dd/MM/yyyy") %>
                </div>
                
                <div>
                    <%# Eval("Tipo") %>
                </div>

                <div>
                    <span Visible='<%# ((double)Eval("Kms")) > 0 %>'>
                        <%# Eval("Kms", "{0:N2} km") %>
                    </span>
                </div>
                
                <div>
                    <%# Eval("Descripcion") %>
                </div>

                </div>
        </ItemTemplate>

        <AlternatingItemTemplate>
            <div style="background-color: White; padding: 10px; border-bottom: 1px solid #CCC; display: grid; grid-template-columns: 2fr 1fr 1fr 1fr 3fr; gap: 10px;">
                
                <div>
                    <strong><%# Eval("Titulo") %></strong>
                </div>
                
                <div>
                    <%# ((DateTime)Eval("Fecha")).ToString("dd/MM/yyyy") %>
                </div>
                
                <div>
                    <%# Eval("Tipo") %>
                </div>
                
                <div>
                    <span visible='<%# ((double)Eval("Kms")) > 0 %>'>
                        <%# (double)Eval("Kms") > 0 ? Eval("Kms", "{0:N2} km") : "" %>
                    </span>
                </div>
                
                <div>
                    <%# Eval("Descripcion") %>
                </div>
                
            </div>
        </AlternatingItemTemplate>



    </asp:Repeater>
</div>
        </div>

        <asp:Label ID="lblNingunaActividad" runat="server" style="text-align: center" Text="Label"></asp:Label>

    </form>
</body>
</html>