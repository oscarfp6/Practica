<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditarActividad.aspx.cs" Inherits="www1.EditarActividad" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Editar Actividad</title>
    <style type="text/css">
        /* --- Estilos base consistentes --- */
        body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; display: flex; justify-content: center; align-items: center; min-height: 100vh; }
        .container-box { background-color: #fff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); width: 450px; text-align: center; }
        .container-box h2 { margin-bottom: 25px; color: #333; }
        .form-group { margin-bottom: 15px; text-align: left; position: relative; }
        .form-group label { display: block; margin-bottom: 5px; font-weight: bold; color: #555; }
        .aspTextBox, .aspDropDownList { /* Estilo común para TextBox y DropDownList */
             width: calc(100% - 22px); padding: 10px; border: 1px solid #ccc; border-radius: 4px; font-size: 1em; box-sizing: border-box; /* Asegura padding correcto */
        }
        .aspTextBoxMulti { /* Para Descripcion */
             width: calc(100% - 22px); padding: 10px; border: 1px solid #ccc; border-radius: 4px; font-size: 1em; box-sizing: border-box; min-height: 80px;
        }
        .btn { width: 100%; padding: 12px; border: none; border-radius: 4px; font-size: 1.1em; cursor: pointer; margin-top: 10px; }
        .btn-primary { background-color: #007bff; color: white; }
        .btn-primary:hover { background-color: #0056b3; }
        .btn-secondary { background-color: #6c757d; color: white; margin-top: 5px; }
        .btn-secondary:hover { background-color: #5a6268; }
        .message-label { display: block; margin-top: 15px; font-weight: bold; min-height: 1.2em; }
        .error-message { color: #d9534f; /* Red */}
        .success-message { color: #5cb85c; /* Green */}
        .validation-error { color: red; font-size: 0.9em; display: block; margin-top: 3px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
         <asp:HiddenField ID="hdnActividadId" runat="server" Value="0"/> <%-- Para guardar el ID de la actividad --%>

        <div class="container-box">
            <h2>Editar Actividad</h2>

            <%-- Título --%>
            <div class="form-group">
                <asp:Label ID="lblTitulo" runat="server" Text="Título:" AssociatedControlID="tbxTitulo"></asp:Label>
                <asp:TextBox ID="tbxTitulo" runat="server" CssClass="aspTextBox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvTitulo" runat="server" ControlToValidate="tbxTitulo"
                    ErrorMessage="El título es obligatorio." CssClass="validation-error" Display="Dynamic">* Obligatorio</asp:RequiredFieldValidator>
            </div>

            <%-- Tipo Actividad --%>
            <div class="form-group">
                <asp:Label ID="lblTipo" runat="server" Text="Tipo de actividad:" AssociatedControlID="ddlTipoActividad"></asp:Label>
                <asp:DropDownList ID="ddlTipoActividad" runat="server" CssClass="aspDropDownList"></asp:DropDownList>
            </div>

            <%-- Fecha --%>
            <div class="form-group">
                <asp:Label ID="lblFecha" runat="server" Text="Fecha:" AssociatedControlID="tbxFecha"></asp:Label>
                <asp:TextBox ID="tbxFecha" runat="server" TextMode="Date" CssClass="aspTextBox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFecha" runat="server" ControlToValidate="tbxFecha"
                    ErrorMessage="La fecha es obligatoria." CssClass="validation-error" Display="Dynamic">* Obligatorio</asp:RequiredFieldValidator>
                <%-- Podrías añadir validador para fecha no futura si es necesario --%>
            </div>

            <%-- Kms (Distancia) --%>
             <div class="form-group">
                <asp:Label ID="lblKms" runat="server" Text="Kms:" AssociatedControlID="tbxKms"></asp:Label>
                <asp:TextBox ID="tbxKms" runat="server" CssClass="aspTextBox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvKms" runat="server" ControlToValidate="tbxKms"
                    ErrorMessage="Los Kms son obligatorios (pon 0 si no aplica)." CssClass="validation-error" Display="Dynamic">* Obligatorio</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvKms" runat="server" ControlToValidate="tbxKms" Operator="DataTypeCheck" Type="Double"
                    ErrorMessage="Debe ser un número válido (use '.' para decimales)." CssClass="validation-error" Display="Dynamic">* Número inválido</asp:CompareValidator>
                <asp:RangeValidator ID="rvKms" runat="server" ControlToValidate="tbxKms" Type="Double" MinimumValue="0" MaximumValue="10000"
                     ErrorMessage="Debe ser un valor positivo." CssClass="validation-error" Display="Dynamic">* Valor positivo</asp:RangeValidator>
            </div>

            <%-- Descripción --%>
            <div class="form-group">
                <asp:Label ID="lblDescripcion" runat="server" Text="Descripción (opcional):" AssociatedControlID="tbxDescripcion"></asp:Label>
                <asp:TextBox ID="tbxDescripcion" runat="server" TextMode="MultiLine" Rows="3" CssClass="aspTextBoxMulti"></asp:TextBox>
            </div>

             <%-- Botón Guardar Cambios --%>
            <div class="form-group" style="text-align: center;">
                 <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
            </div>

            <%-- Mensaje General (Resultado Éxito/Error) --%>
            <asp:Label ID="lblMensaje" runat="server" CssClass="message-label" Visible="false"></asp:Label>

             <%-- Botón Cancelar/Volver --%>
             <div class="form-group" style="text-align: center;">
                 <asp:Button ID="btnCancelar" runat="server" Text="Cancelar y Volver al Menú" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" CausesValidation="false" />
            </div>

             <%-- Resumen de Validación --%>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="error-message" HeaderText="Por favor, corrige los siguientes errores:" ShowMessageBox="false" ShowSummary="true"/>

        </div>
    </form>
</body>
</html>