<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SistemaCotizaciones.Pages.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Sistema de Cotizaciones - Login</title>
    <link href="../CSS/styles.css" rel="stylesheet" />
</head>
<body>
    <div class="login-container">
        <div class="login-header">
            <h2>Sistema Generador de Cotizaciones</h2>
            <h3>Pasty Custom Design</h3>
        </div>
        
        <form id="form1" runat="server">
            <!-- Mensaje de error/éxito -->
            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-error">
                <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            </asp:Panel>
            
            <div class="form-group">
                <label for="txtUsuario">Usuario:</label>
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" 
                           placeholder="Ingrese su usuario"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUsuario" runat="server" 
                                          ControlToValidate="txtUsuario"
                                          ErrorMessage="El usuario es obligatorio"
                                          CssClass="text-danger" Display="Dynamic">
                </asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
                <label for="txtContrasena">Contraseña:</label>
                <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" 
                           CssClass="form-control" placeholder="Ingrese su contraseña"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvContrasena" runat="server" 
                                          ControlToValidate="txtContrasena"
                                          ErrorMessage="La contraseña es obligatoria"
                                          CssClass="text-danger" Display="Dynamic">
                </asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group" style="text-align: center; margin-top: 25px;">
                <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" 
                          CssClass="btn btn-primary" style="width: 100%; padding: 12px;"
                          OnClick="btnIngresar_Click" />
            </div>
            
            <div style="text-align: center; margin-top: 20px; font-size: 12px; color: #666;">
                <p>Sistema desarrollado para Pasty Custom Design</p>
            </div>
        </form>
    </div>
</body>
</html>