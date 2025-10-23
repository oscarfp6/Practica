<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistrarActividad.aspx.cs" Inherits="www1.WebForm5" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registrar Actividad</title>
    <style type="text/css">
        .campo { width: 150px; text-align: right; padding-right: 10px; }
        .valor { width: 250px; text-align: left; }
        .error { color: red; font-size: 0.9em; }
        .mensaje { font-weight: bold; margin-top: 15px; }
        .tabla-centrada { margin-left: auto; margin-right: auto; border-spacing: 5px; border-collapse: separate; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center; margin-top: 20px;">
            <h2>Registra tu nueva actividad</h2>
            
            <table class="tabla-centrada">
                <tr>
                    <td class="campo"><asp:Label ID="lblTitulo" runat="server" Text="Título:"></asp:Label></td>
                    <td class="valor">
                        <asp:TextBox ID="tbxTitulo" runat="server" Width="200px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvTitulo" runat="server" ControlToValidate="tbxTitulo"
                            ErrorMessage="El título es obligatorio." CssClass="error" Display="Dynamic">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                 <tr>
                    <td class="campo"><asp:Label ID="lblTipo" runat="server" Text="Tipo de actividad:"></asp:Label></td>
                    <td class="valor">
                        <%-- CAMBIO: Usamos DropDownList --%>
                        <asp:DropDownList ID="ddlTipoActividad" runat="server" Width="205px"></asp:DropDownList>
                    </td>
                </tr>
                 <tr>
                    <td class="campo"><asp:Label ID="lblFecha" runat="server" Text="Fecha:"></asp:Label></td>
                    <td class="valor">
                        <asp:TextBox ID="tbxFecha" runat="server" TextMode="Date" Width="200px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvFecha" runat="server" ControlToValidate="tbxFecha"
                            ErrorMessage="La fecha es obligatoria." CssClass="error" Display="Dynamic">*</asp:RequiredFieldValidator>
                        <%-- Podríamos añadir un CompareValidator para asegurar que no es futura --%>
                    </td>
                </tr>
                <tr>
                    <td class="campo"><asp:Label ID="lblDuracion" runat="server" Text="Duración (hh:mm):"></asp:Label></td>
                    <td class="valor">
                         <%-- CAMBIO: Usamos TextMode Normal y añadimos validador --%>
                        <asp:TextBox ID="tbxDuracion" runat="server" Width="200px" placeholder="Ej: 01:30"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvDuracion" runat="server" ControlToValidate="tbxDuracion"
                            ErrorMessage="La duración es obligatoria." CssClass="error" Display="Dynamic">*</asp:RequiredFieldValidator>
                         <%-- Validador para formato HH:MM o HH:MM:SS --%>
                        <asp:RegularExpressionValidator ID="revDuracion" runat="server" ControlToValidate="tbxDuracion"
                            ValidationExpression="^([0-9]{1,2}):([0-5][0-9])(:[0-5][0-9])?$"
                            ErrorMessage="Formato debe ser hh:mm o hh:mm:ss." CssClass="error" Display="Dynamic">*</asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="campo"><asp:Label ID="lblKms" runat="server" Text="Kms:"></asp:Label></td>
                    <td class="valor">
                        <%-- CAMBIO: TextMode normal con validadores --%>
                        <asp:TextBox ID="tbxKms" runat="server" Width="200px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvKms" runat="server" ControlToValidate="tbxKms"
                            ErrorMessage="Los Kms son obligatorios (pon 0 si no aplica)." CssClass="error" Display="Dynamic">*</asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvKms" runat="server" ControlToValidate="tbxKms" Operator="DataTypeCheck" Type="Double"
                            ErrorMessage="Debe ser un número válido." CssClass="error" Display="Dynamic">*</asp:CompareValidator>
                        <asp:RangeValidator ID="rvKms" runat="server" ControlToValidate="tbxKms" Type="Double" MinimumValue="0" MaximumValue="10000"
                             ErrorMessage="Debe ser un valor positivo." CssClass="error" Display="Dynamic">*</asp:RangeValidator>
                    </td>
                </tr>
                 <tr>
                    <td class="campo"><asp:Label ID="lblMetrosDesnivel" runat="server" Text="Desnivel (metros):"></asp:Label></td>
                    <td class="valor">
                        <asp:TextBox ID="tbxMetrosDesnivel" runat="server" Width="200px" Text="0"></asp:TextBox>
                         <asp:CompareValidator ID="cvDesnivel" runat="server" ControlToValidate="tbxMetrosDesnivel" Operator="DataTypeCheck" Type="Integer"
                            ErrorMessage="Debe ser un número entero válido." CssClass="error" Display="Dynamic">*</asp:CompareValidator>
                        <asp:RangeValidator ID="rvDesnivel" runat="server" ControlToValidate="tbxMetrosDesnivel" Type="Integer" MinimumValue="0" MaximumValue="50000"
                             ErrorMessage="Debe ser un valor positivo." CssClass="error" Display="Dynamic">*</asp:RangeValidator>
                    </td>
                </tr>
                <tr>
                    <td class="campo"><asp:Label ID="lblFcMedia" runat="server" Text="FC Media (opcional):"></asp:Label></td>
                    <td class="valor">
                        <asp:TextBox ID="tbxFcMedia" runat="server" Width="200px"></asp:TextBox>
                         <asp:CompareValidator ID="cvFcMedia" runat="server" ControlToValidate="tbxFcMedia" Operator="DataTypeCheck" Type="Integer"
                            ErrorMessage="Debe ser un número entero (si se introduce)." CssClass="error" Display="Dynamic">*</asp:CompareValidator>
                         <%-- Rango según tu clase Actividad --%>
                        <asp:RangeValidator ID="rvFcMedia" runat="server" ControlToValidate="tbxFcMedia" Type="Integer" MinimumValue="30" MaximumValue="220"
                             ErrorMessage="Debe estar entre 30 y 220 bpm (si se introduce)." CssClass="error" Display="Dynamic">*</asp:RangeValidator>
                    </td>
                </tr>
                 <tr>
                    <td class="campo"><asp:Label ID="lblDescripcion" runat="server" Text="Descripción (opcional):"></asp:Label></td>
                    <td class="valor">
                        <asp:TextBox ID="tbxDescripcion" runat="server" TextMode="MultiLine" Rows="3" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="valor" style="padding-top: 15px;">
                        <asp:Button ID="btnConfirmar" runat="server" Text="Guardar Actividad" OnClick="btnConfirmar_Click" />
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" CausesValidation="false" style="margin-left: 10px;" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="valor">
                         <%-- Label para mostrar mensajes de éxito o error --%>
                        <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false"></asp:Label>
                    </td>
                </tr>
            </table>

            <%-- Muestra un resumen de todos los errores de validación --%>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="error" HeaderText="Por favor, corrige los siguientes errores:" />

        </div>
    </form>
</body>
</html>