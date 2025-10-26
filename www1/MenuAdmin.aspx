<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuAdmin.aspx.cs" Inherits="www1.MenuAdmin" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Panel de Administración</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; background-color: #f9f9f9; }
        h2 { text-align: center; color: #333; }
        .admin-header { 
        /* Estilos existentes */
            display: flex; 
            justify-content: space-between; 
            align-items: center; 
            background-color: #333; /* Fondo oscuro */
            color: white; 
            padding: 10px 20px; 
            border-radius: 5px; 
        }
        .admin-header h3 { margin: 0; }
        .admin-header a { color: #ffc107; text-decoration: none; font-weight: bold; }
        .btn-logout-admin { 
        /* Base de diseño */
            padding: 8px 10px; 
            font-size: 1.05em; 
            font-weight: bold;
            border: none;
            border-radius: 4px; 
            cursor: pointer;
            width: 200px; /* Hacemos el botón más ancho */
            transition: background-color 0.3s ease, transform 0.1s ease;
    
            /* Colores primarios (Rojo/Salida) */
            background-color: #dc3545; /* Rojo Bootstrap de peligro */
            color: white; 
    
            /* Pequeño ajuste visual */
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2); 
        }
        .btn-logout-admin:hover {
            background-color: #c82333; /* Rojo más oscuro al pasar el ratón */
            transform: translateY(-1px); /* Efecto 3D sutil */
        }
        
        /* Estilos del Repeater como Grid */
        .user-grid { margin-top: 20px; width: 100%; text-align: left; border-collapse: collapse; }
        .user-header, .user-item {
            display: grid;
            grid-template-columns: 3fr 2fr 1.5fr 1.5fr 2fr 2fr 1fr; /* 7 columnas */
            gap: 10px;
            padding: 12px;
            border-bottom: 1px solid #ddd;
            align-items: center;
        }
        .user-header { background-color: #507CD1; color: white; font-weight: bold; border-radius: 5px 5px 0 0; }
        .user-item { background-color: #fff; }
        .user-item:nth-child(even) { background-color: #f5f5f5; }

        /* Estilos para controles internos */
        .user-item input[type="text"], .user-item select {
            padding: 5px;
            width: 95%;
            border: 1px solid #ccc;
            border-radius: 4px;
        }
        .user-item input[type="submit"] {
            padding: 6px 10px;
            background-color: #28a745; /* Verde */
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }
        .user-item input[type="submit"]:hover { background-color: #218838; }
        
        /* Estilos para mensajes */
        .message { font-size: 0.9em; font-weight: bold; }
        .message-success { color: green; }
        .message-error { color: red; }

        /* Estilo para la fila del Admin (deshabilitada) */
        .admin-row { background-color: #eee; color: #777; }
        .admin-row select, .admin-row input {
            background-color: #f5f5f5;
            cursor: not-allowed;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="admin-header">
            <asp:Label ID="lblAdminNombre" runat="server" Font-Bold="True" Font-Size="Large" Text="Panel de Administración"></asp:Label>
            <asp:Button ID="btnLogOut" runat="server" Text="Cerrar Sesión (Admin)" OnClick="btnLogOut_Click" CssClass="btn-logout-admin" />
        </div>

        <h2>Gestión de Usuarios</h2>

        <div class="user-grid">
            <asp:Repeater ID="rptUsuarios" runat="server" 
                OnItemDataBound="rptUsuarios_ItemDataBound" 
                OnItemCommand="rptUsuarios_ItemCommand">
                
                <HeaderTemplate>
                    <div class="user-header">
                        <div>Email (Usuario)</div>
                        <div>Nombre</div>
                        <div>Estado Actual</div>
                        <div>Nuevo Estado</div>
                        <div>Nueva Contraseña</div>
                        <div>Acciones</div>
                        <div>Mensaje</div>
                    </div>
                </HeaderTemplate>

<ItemTemplate>
                    <%-- Usamos una clase CSS especial si es el Admin (ID=1) --%>
                    <div class="user-item <%# (int)Eval("Id") == 1 ? "admin-row" : "" %>">
                        
                        <%-- Col 1: Email --%>
                        <div><asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>' Font-Bold="true"></asp:Label></div>
                        
                        <%-- Col 2: Nombre --%>
                        <div><asp:Label ID="lblNombre" runat="server" Text='<%# Eval("Nombre") + " " + Eval("Apellidos") %>'></asp:Label></div>
                        
                        <%-- Col 3: Estado Actual --%>
                        <div><asp:Label ID="lblEstadoActual" runat="server" Text='<%# Eval("Estado") %>'></asp:Label></div>

                        
                        <%-- Col 4: Nuevo Estado (DropDownList) --%>
                        <div>
                            <asp:DropDownList ID="ddlNuevoEstado" runat="server"> <%-- ARREGLADO: Faltaba el '>' --%>
                            </asp:DropDownList>
                        </div>

                        <%-- Col 5: Nueva Contraseña (TextBox) --%>
                        <div>
                            <asp:TextBox ID="tbxNuevaPassword" runat="server" TextMode="Password" 
                                placeholder="Dejar en blanco para no cambiar">
                            </asp:TextBox>
                        </div>
                        
                        <%-- Col 6: Acciones (Button) --%>
                        <div>
                            <asp:Button ID="btnActualizarUsuario" runat="server" Text="Actualizar" 
                                CommandName="Actualizar" CommandArgument='<%# Eval("Id") %>' /> <%-- ARREGLADO: Eliminado 'Enabled' y cerrado con '/>' --%>
                        </div>

                        <%-- Col 7: Mensaje (Feedback) --%>
                        <div>
                            <asp:Label ID="lblMensajeFila" runat="server" CssClass="message" Visible="false"></asp:Label>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </form>
</body>
</html>