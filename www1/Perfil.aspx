<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="www1.Perfil" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Perfil de Usuario</title>
    <style type="text/css">
        /* Base Styles - Consistente con Login/Menu */
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f4f9; 
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }

        .perfil-box {
            background-color: #fff; 
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
            width: 450px;
            text-align: center;
        }

        .form-title {
            font-size: 28px;
            color: #1a1a1a;
            margin-bottom: 30px;
            font-weight: 600;
            display: block;
        }

        .form-group {
            margin-bottom: 15px;
            text-align: left;
        }

        .form-group label {
            display: block;
            margin-bottom: 6px;
            font-weight: 500;
            color: #555;
            font-size: 14px;
        }

        .form-group input[type="text"] {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            box-sizing: border-box;
            font-size: 16px;
            transition: border-color 0.3s;
        }
        
        .form-group input:focus {
            border-color: #007bff;
            outline: none;
        }
        
        .btn-primary {
            width: 100%;
            padding: 12px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: bold;
            margin-top: 20px;
            transition: background-color 0.3s;
        }
        
        .btn-primary:hover {
            background-color: #0056b3;
        }
        
        .message-success {
            color: green;
            font-weight: bold;
            margin-top: 15px;
        }
        .message-error {
            color: red;
            font-weight: bold;
            margin-top: 15px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="perfil-box">
            
            <asp:Label ID="lblTitulo" runat="server" CssClass="form-title" Text="Editar Perfil"></asp:Label>

            <div class="form-group">
                <asp:Label ID="Label1" runat="server" Text="Nombre:"></asp:Label>
                <asp:TextBox ID="tbxNombre" runat="server"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="Label2" runat="server" Text="Apellidos:"></asp:Label>
                <asp:TextBox ID="tbxApellidos" runat="server"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <asp:Label ID="Label3" runat="server" Text="Email:"></asp:Label>
                <asp:TextBox ID="tbxEmail" runat="server" ReadOnly="true" BackColor="#EEEEEE"></asp:TextBox>
            </div>
            
            <div class="form-group" style="text-align: center;">
                <asp:CheckBox ID="chkSuscripcion" runat="server" Text=" Recibir notificaciones por correo electrónico" />
            </div>

            <asp:Label ID="lblMensaje" runat="server" Text="" CssClass="message-success"></asp:Label>

            <asp:Button ID="btnGuardarPerfil" runat="server" Text="Guardar Cambios" CssClass="btn-primary" OnClick="btnGuardarPerfil_Click" />

            <asp:Button ID="btnVolver" runat="server" Text="Volver al Menú" CssClass="btn-primary" OnClick="btnVolver_Click" style="background-color: #6c757d;" />

        </div>
    </form>
</body>
</html>