<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="SistemaCotizaciones.Pages.Clientes" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Sistema de Cotizaciones - Gestión de Clientes</title>
    <link href="../CSS/styles.css" rel="stylesheet" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header -->
        <div class="header">
            <h1>🏪 Sistema de Cotizaciones - Pasty Custom Design</h1>
        </div>

        <!-- Navigation Menu -->
        <div class="nav-menu">
            <a href="Default.aspx">🏠 Dashboard</a>
            <a href="Clientes.aspx" style="background-color: #2c3e50;">👥 Clientes</a>
            <a href="Productos.aspx">📦 Productos</a>
            <a href="Cotizaciones.aspx">📋 Cotizaciones</a>
            <a href="Reportes.aspx">📊 Reportes</a>
            <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" style="color: white; text-decoration: none; padding: 10px 20px; margin: 0 5px;">🚪 Cerrar Sesión</asp:LinkButton>
        </div>

        <div class="container">
            <div class="form-container">
                <h2>👥 Gestión de Clientes</h2>

                <!-- Mensajes de error/éxito -->
                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
                </asp:Panel>

                <!-- Pestañas -->
                <div style="margin-bottom: 20px;">
                    <asp:Button ID="btnTabRegistrar" runat="server" Text="➕ Registrar Cliente" 
                        CssClass="btn btn-primary" OnClick="btnTabRegistrar_Click" />
                    <asp:Button ID="btnTabBuscar" runat="server" Text="🔍 Buscar Cliente" 
                        CssClass="btn" OnClick="btnTabBuscar_Click" />
                    <asp:Button ID="btnTabListar" runat="server" Text="📋 Lista de Clientes" 
                        CssClass="btn" OnClick="btnTabListar_Click" />
                </div>

                <!-- Panel Registrar Cliente -->
                <asp:Panel ID="pnlRegistrar" runat="server">
                    <h3>➕ Registrar Nuevo Cliente</h3>
                    <div class="form-group">
                        <label for="txtNombre">Nombre Completo *</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" 
                            placeholder="Ingrese el nombre completo del cliente"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNombre" runat="server" 
                            ControlToValidate="txtNombre" ErrorMessage="El nombre es obligatorio"
                            ValidationGroup="RegistrarCliente" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtIdentificacion">Identificación *</label>
                        <asp:TextBox ID="txtIdentificacion" runat="server" CssClass="form-control" 
                            placeholder="Cédula o identificación del cliente"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvIdentificacion" runat="server" 
                            ControlToValidate="txtIdentificacion" ErrorMessage="La identificación es obligatoria"
                            ValidationGroup="RegistrarCliente" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtTelefono">Teléfono</label>
                        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" 
                            placeholder="Número de teléfono"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtEmail">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                            placeholder="Correo electrónico" TextMode="Email"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtDireccion">Dirección</label>
                        <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" 
                            placeholder="Dirección completa" TextMode="MultiLine" Rows="2"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="ddlTipoCliente">Tipo de Cliente</label>
                        <asp:DropDownList ID="ddlTipoCliente" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Seleccionar tipo</asp:ListItem>
                            <asp:ListItem Value="Individual">Individual</asp:ListItem>
                            <asp:ListItem Value="Empresa">Empresa</asp:ListItem>
                            <asp:ListItem Value="Mayorista">Mayorista</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <asp:Button ID="btnRegistrar" runat="server" Text="💾 Registrar Cliente" 
                        CssClass="btn btn-success" OnClick="btnRegistrar_Click" 
                        ValidationGroup="RegistrarCliente" />
                    <asp:Button ID="btnLimpiar" runat="server" Text="🗑️ Limpiar" 
                        CssClass="btn" OnClick="btnLimpiar_Click" />
                </asp:Panel>

                <!-- Panel Buscar Cliente -->
                <asp:Panel ID="pnlBuscar" runat="server" Visible="false">
                    <h3>🔍 Buscar Cliente</h3>
                    <div class="form-group">
                        <label for="txtBuscarIdentificacion">Identificación del Cliente</label>
                        <asp:TextBox ID="txtBuscarIdentificacion" runat="server" CssClass="form-control" 
                            placeholder="Ingrese la identificación del cliente"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnBuscar" runat="server" Text="🔍 Buscar" 
                        CssClass="btn btn-primary" OnClick="btnBuscar_Click" />

                    <!-- Panel con datos del cliente encontrado -->
                    <asp:Panel ID="pnlClienteEncontrado" runat="server" Visible="false" style="margin-top: 20px;">
                        <h4>📋 Información del Cliente</h4>
                        <div style="background-color: #f8f9fa; padding: 15px; border-radius: 5px;">
                            <div class="form-group">
                                <label for="txtEditNombre">Nombre Completo *</label>
                                <asp:TextBox ID="txtEditNombre" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEditNombre" runat="server" 
                                    ControlToValidate="txtEditNombre" ErrorMessage="El nombre es obligatorio"
                                    ValidationGroup="EditarCliente" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <label for="txtEditIdentificacion">Identificación *</label>
                                <asp:TextBox ID="txtEditIdentificacion" runat="server" CssClass="form-control" ReadOnly="true" 
                                    style="background-color: #e9ecef;"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="txtEditTelefono">Teléfono</label>
                                <asp:TextBox ID="txtEditTelefono" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="txtEditEmail">Email</label>
                                <asp:TextBox ID="txtEditEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="txtEditDireccion">Dirección</label>
                                <asp:TextBox ID="txtEditDireccion" runat="server" CssClass="form-control" 
                                    TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="ddlEditTipoCliente">Tipo de Cliente</label>
                                <asp:DropDownList ID="ddlEditTipoCliente" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Seleccionar tipo</asp:ListItem>
                                    <asp:ListItem Value="Individual">Individual</asp:ListItem>
                                    <asp:ListItem Value="Empresa">Empresa</asp:ListItem>
                                    <asp:ListItem Value="Mayorista">Mayorista</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div style="margin-top: 15px;">
                                <asp:Button ID="btnEditar" runat="server" Text="💾 Guardar Cambios" 
                                    CssClass="btn btn-success" OnClick="btnEditar_Click" 
                                    ValidationGroup="EditarCliente" />
                                <asp:Button ID="btnEliminar" runat="server" Text="🗑️ Eliminar Cliente" 
                                    CssClass="btn btn-danger" OnClick="btnEliminar_Click" 
                                    OnClientClick="return confirm('¿Está seguro que desea inhabilitar este cliente? Esta acción no se puede deshacer.');" />
                            </div>
                        </div>
                    </asp:Panel>
                </asp:Panel>

                <!-- Panel Lista de Clientes -->
                <asp:Panel ID="pnlListar" runat="server" Visible="false">
                    <h3>📋 Lista de Clientes Activos</h3>
                    <div style="margin-bottom: 15px;">
                        <asp:Button ID="btnActualizar" runat="server" Text="🔄 Actualizar Lista" 
                            CssClass="btn btn-primary" OnClick="btnActualizar_Click" />
                        <span style="margin-left: 20px; font-weight: bold;">Total de clientes: 
                            <asp:Label ID="lblTotalClientes" runat="server" Text="0" style="color: #3498db;"></asp:Label>
                        </span>
                    </div>
                    <asp:GridView ID="gvClientes" runat="server" CssClass="table" AutoGenerateColumns="false" 
                        EmptyDataText="No hay clientes registrados en el sistema." 
                        HeaderStyle-BackgroundColor="#34495e" HeaderStyle-ForeColor="White">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Identificacion" HeaderText="Identificación" />
                            <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="TipoCliente" HeaderText="Tipo" />
                            <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Registro" DataFormatString="{0:dd/MM/yyyy}" />
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>