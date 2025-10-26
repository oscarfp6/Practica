<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="www1.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Mi Menú Principal</title>
    <style type="text/css">
        /* --- (Estilos existentes) --- */
        body { font-family: Arial, sans-serif; margin-bottom: 10px; }
        .auto-style1 { height: 33px; }
        .auto-style2 { height: 33px; width: 373px; text-align: left; }
        .auto-style4 { height: 33px; width: 398px; text-align: justify; }
        .auto-style6 { height: 33px; width: auto; text-align: right; padding-left:10px; } /* Ajustado width y padding */
        .auto-style7 { height: 33px; width: auto; text-align: right; } /* Ajustado width */
        #form1 { text-align: center; }

        /* --- Estilos para Repeater Grid --- */
        .repeater-grid-container { margin-top: 20px; width: 95%; margin-left: auto; margin-right: auto; text-align: left; }
        .repeater-header, .repeater-item, .repeater-alt-item {
            padding: 10px;
            border-bottom: 1px solid #CCC;
            display: grid;
            
            /* Columnas: Título(3), Fecha(1), Tipo(1), Distancia(1), Ritmo(1.5), Desc(4), Acciones(2) */
            grid-template-columns: 3fr 1fr 1fr 1fr 1.5fr 4fr 2fr;
            
            gap: 10px;
            align-items: center; /* Centrar verticalmente */
        }
        .repeater-header { background-color: #507CD1; color: white; font-weight: bold; }
        .repeater-item { background-color: #EFF3FB; }
        .repeater-alt-item { background-color: White; }

        /* --- Estilos para Botones de Acción --- */
        .action-buttons a, .action-buttons input[type=submit] { /* Usar LinkButton o Button */
            text-decoration: none;
            padding: 5px 10px;
            margin-right: 5px;
            border-radius: 4px;
            color: white;
            font-size: 0.9em;
            cursor: pointer;
            border: none; /* Para Button */
        }
        .btn-edit { background-color: #f0ad4e; /* Naranja */ }
        .btn-edit:hover { background-color: #ec971f; }
        .btn-delete { background-color: #d9534f; /* Rojo */ }
        .btn-delete:hover { background-color: #c9302c; }

         /* --- Estilo para Mensajes --- */
        .menu-message {
            display: block; /* O inline-block si prefieres */
            text-align: center;
            margin: 15px auto;
            padding: 10px;
            border-radius: 5px;
            font-weight: bold;
        }
        .message-success { background-color: #dff0d8; color: #3c763d; border: 1px solid #d6e9c6;}
        .message-error { background-color: #f2dede; color: #a94442; border: 1px solid #ebccd1;}
        .message-warning { background-color: #fcf8e3; color: #8a6d3b; border: 1px solid #faebcc;} /* Para mensaje suscripción */

    </style>
</head>
<body style="width: 95%; margin: auto;"> <%-- Ajustado body width --%>
    <form id="form1" runat="server">
        <%-- Encabezado con Nombre y Botones --%>
        <table style="width:100%; margin-bottom: 15px;">
            <tr>
                <td style="text-align:left;"> <%-- Alineación Nombre --%>
                    <asp:Label ID="lblNombreApellidos" runat="server" Text="Label" Font-Bold="True" Font-Size="Large"></asp:Label>
                </td>
                 <%-- Celdas flexibles para botones a la derecha --%>
                 <td style="width:100%;">&nbsp;</td> <%-- Espaciador flexible --%>
                 <td class="auto-style6">
                      <asp:Button ID="btnRegistrarActividad" runat="server" Text="Registrar Actividad" OnClick="BtnRegistrarActividad_Click" Width="180px" /> <%-- Ancho ajustado --%>
                </td>
                 <td class="auto-style6">
                    <asp:Button ID="btnPerfil" runat="server" Text="Perfil" Width="100px" OnClick="BtnPerfil_Click" /> <%-- Ancho ajustado --%>
                </td>
                <td class="auto-style6">
                     <asp:Button ID="btnLogOut" runat="server" Text="Log Out" Width="100px" OnClick="BtnLogOut_Click" /> <%-- Ancho ajustado --%>
                </td>
            </tr>
        </table>

        <hr />

          <%-- Label para Mensajes (Suscripción, Éxito/Error Borrado) --%>
        <asp:Label ID="lblMenuMessage" runat="server" CssClass="menu-message" Visible="false"></asp:Label>


        <%-- Listado central de actividades --%>
        <div class="repeater-grid-container">
            <h2 style="text-align: center;">Mis Actividades Recientes</h2>

            <asp:Repeater ID="rptActividades" runat="server" OnItemCommand="RptActividades_ItemCommand">

                <HeaderTemplate>
                     <div class="repeater-header">
                        <div>Título</div>
                        <div>Fecha</div>
                        <div>Tipo</div>
                        <div>Distancia</div>
                        <div>Ritmo</div> <%-- !! NUEVA CABECERA AÑADIDA !! --%>
                        <div>Descripción</div>
                        <div>Acciones</div>
                    </div>
                </HeaderTemplate>

                <ItemTemplate>
                    <div class="repeater-item">
                        <div><strong><%# Eval("Titulo") %></strong></div>
                        <div><%# ((DateTime)Eval("Fecha")).ToString("dd/MM/yyyy") %></div>
                        <div><%# Eval("Tipo") %></div>
                        <div><%# (double)Eval("Kms") > 0 ? Eval("Kms", "{0:N2} km") : "" %></div>
                        
                        <%-- !! NUEVA CELDA AÑADIDA (Llama a la función del C#) !! --%>
                        <div><%# FormatearRitmo(Eval("Tipo"), Eval("VelocidadMediaKmh"), Eval("RitmoMinPorKm")) %></div>

                        <div><%# Eval("Descripcion") %></div>
                        <%-- Nueva celda para botones --%>
                        <div class="action-buttons">
                            <asp:LinkButton ID="btnEditar" runat="server" Text="Editar" CssClass="btn-edit"
                                CommandName="EditActivity" CommandArgument='<%# Eval("Id") %>' />
                            <asp:LinkButton ID="btnEliminar" runat="server" Text="Eliminar" CssClass="btn-delete"
                                 CommandName="DeleteActivity" CommandArgument='<%# Eval("Id") %>'
                                OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta actividad?');" /> <%-- Confirmación JS --%>
                        </div>
                    </div>
                </ItemTemplate>

                <AlternatingItemTemplate>
                      <div class="repeater-alt-item">
                        <div><strong><%# Eval("Titulo") %></strong></div>
                        <div><%# ((DateTime)Eval("Fecha")).ToString("dd/MM/yyyy") %></div>
                        <div><%# Eval("Tipo") %></div>
                        <div><%# (double)Eval("Kms") > 0 ? Eval("Kms", "{0:N2} km") : "" %></div>

                        <%-- !! NUEVA CELDA AÑADIDA (Llama a la función del C#) !! --%>
                        <div><%# FormatearRitmo(Eval("Tipo"), Eval("VelocidadMediaKmh"), Eval("RitmoMinPorKm")) %></div>

                        <div><%# Eval("Descripcion") %></div>
                         <%-- Nueva celda para botones --%>
                        <div class="action-buttons">
                             <asp:LinkButton ID="btnEditarAlt" runat="server" Text="Editar" CssClass="btn-edit"
                                CommandName="EditActivity" CommandArgument='<%# Eval("Id") %>' />
                            <asp:LinkButton ID="btnEliminarAlt" runat="server" Text="Eliminar" CssClass="btn-delete"
                                 CommandName="DeleteActivity" CommandArgument='<%# Eval("Id") %>'
                                OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta actividad?');" /> <%-- Confirmación JS --%>
                        </div>
                    </div>
                </AlternatingItemTemplate>

            </asp:Repeater>
        </div>

        <%-- Mensaje si no hay actividades --%>
        <asp:Label ID="lblNingunaActividad" runat="server" style="text-align: center; display: block; margin-top: 20px;" Text="No tienes actividades registradas." Visible="false"></asp:Label>

    </form>
</body>
</html>
