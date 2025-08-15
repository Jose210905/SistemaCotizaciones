<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Productos.aspx.cs" Inherits="SistemaCotizaciones.Pages.Productos" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Sistema de Cotizaciones - Gestión de Productos</title>
    <link href="../CSS/styles.css" rel="stylesheet" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header -->
        <div class="header">
            <h1>🌪 Sistema de Cotizaciones - Pasty Custom Design</h1>
        </div>

        <!-- Navigation Menu -->
        <div class="nav-menu">
            <a href="Default.aspx">🏠 Dashboard</a>
            <a href="Clientes.aspx">👥 Clientes</a>
            <a href="Productos.aspx" style="background-color: #2c3e50;">📦 Productos</a>
            <a href="Cotizaciones.aspx">📋 Cotizaciones</a>
            <a href="Reportes.aspx">📊 Reportes</a>
            <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" style="color: white; text-decoration: none; padding: 10px 20px; margin: 0 5px;">🚪 Cerrar Sesión</asp:LinkButton>
        </div>

        <div class="container">
            <div class="form-container">
                <h2>📦 Gestión de Productos</h2>

                <!-- Mensajes de error/éxito -->
                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
                </asp:Panel>

                <!-- Pestañas -->
                <div style="margin-bottom: 20px;">
                    <asp:Button ID="btnTabRegistrar" runat="server" Text="➕ Registrar Producto" 
                        CssClass="btn btn-primary" OnClick="btnTabRegistrar_Click" />
                    <asp:Button ID="btnTabBuscar" runat="server" Text="🔍 Buscar Producto" 
                        CssClass="btn" OnClick="btnTabBuscar_Click" />
                    <asp:Button ID="btnTabListar" runat="server" Text="📋 Lista de Productos" 
                        CssClass="btn" OnClick="btnTabListar_Click" />
                </div>

                <!-- Panel Registrar Producto -->
                <asp:Panel ID="pnlRegistrar" runat="server">
                    <h3>➕ Registrar Nuevo Producto</h3>
                    <div class="form-group">
                        <label for="txtCodigoCabys">Código CABYS *</label>
                        <asp:TextBox ID="txtCodigoCabys" runat="server" CssClass="form-control" 
                            placeholder="Código CABYS del producto"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCodigoCabys" runat="server" 
                            ControlToValidate="txtCodigoCabys" ErrorMessage="El código CABYS es obligatorio"
                            ValidationGroup="RegistrarProducto" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtNombreProducto">Nombre del Producto *</label>
                        <asp:TextBox ID="txtNombreProducto" runat="server" CssClass="form-control" 
                            placeholder="Ingrese el nombre del producto"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNombreProducto" runat="server" 
                            ControlToValidate="txtNombreProducto" ErrorMessage="El nombre del producto es obligatorio"
                            ValidationGroup="RegistrarProducto" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtDescripcion">Descripción</label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" 
                            placeholder="Descripción del producto" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtCantidadDisponible">Cantidad Disponible *</label>
                        <asp:TextBox ID="txtCantidadDisponible" runat="server" CssClass="form-control" 
                            placeholder="Cantidad en inventario" TextMode="Number"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCantidadDisponible" runat="server" 
                            ControlToValidate="txtCantidadDisponible" ErrorMessage="La cantidad disponible es obligatoria"
                            ValidationGroup="RegistrarProducto" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                        <asp:RangeValidator ID="rvCantidadDisponible" runat="server" 
                            ControlToValidate="txtCantidadDisponible" MinimumValue="0" MaximumValue="999999"
                            Type="Integer" ErrorMessage="La cantidad debe ser mayor o igual a 0"
                            ValidationGroup="RegistrarProducto" Display="Dynamic" CssClass="alert-error"></asp:RangeValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtPrecio">Precio (₡) *</label>
                        <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control" 
                            placeholder="Precio del producto" TextMode="Number" step="0.01"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPrecio" runat="server" 
                            ControlToValidate="txtPrecio" ErrorMessage="El precio es obligatorio"
                            ValidationGroup="RegistrarProducto" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                        <asp:RangeValidator ID="rvPrecio" runat="server" 
                            ControlToValidate="txtPrecio" MinimumValue="0.01" MaximumValue="999999.99"
                            Type="Double" ErrorMessage="El precio debe ser mayor a ₡0.01"
                            ValidationGroup="RegistrarProducto" Display="Dynamic" CssClass="alert-error"></asp:RangeValidator>
                    </div>
                    <asp:Button ID="btnRegistrar" runat="server" Text="💾 Registrar Producto" 
                        CssClass="btn btn-success" OnClick="btnRegistrar_Click" 
                        ValidationGroup="RegistrarProducto" />
                    <asp:Button ID="btnLimpiar" runat="server" Text="🗑️ Limpiar" 
                        CssClass="btn" OnClick="btnLimpiar_Click" />
                </asp:Panel>

                <!-- Panel Buscar Producto -->
                <asp:Panel ID="pnlBuscar" runat="server" Visible="false">
                    <h3>🔍 Buscar Producto</h3>
                    <div class="form-group">
                        <label for="ddlTipoBusqueda">Buscar por:</label>
                        <asp:DropDownList ID="ddlTipoBusqueda" runat="server" CssClass="form-control">
                            <asp:ListItem Value="ID">ID del Producto</asp:ListItem>
                            <asp:ListItem Value="CodigoCabys">Código CABYS</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label for="txtBuscarValor">Valor de búsqueda</label>
                        <asp:TextBox ID="txtBuscarValor" runat="server" CssClass="form-control" 
                            placeholder="Ingrese ID o Código CABYS"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnBuscar" runat="server" Text="🔍 Buscar" 
                        CssClass="btn btn-primary" OnClick="btnBuscar_Click" />

                    <!-- Panel con datos del producto encontrado -->
                    <asp:Panel ID="pnlProductoEncontrado" runat="server" Visible="false" style="margin-top: 20px;">
                        <h4>📋 Información del Producto</h4>
                        <div style="background-color: #f8f9fa; padding: 15px; border-radius: 5px;">
                            <div class="form-group">
                                <label for="txtEditCodigoCabys">Código CABYS *</label>
                                <asp:TextBox ID="txtEditCodigoCabys" runat="server" CssClass="form-control" ReadOnly="true" 
                                    style="background-color: #e9ecef;"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="txtEditNombreProducto">Nombre del Producto *</label>
                                <asp:TextBox ID="txtEditNombreProducto" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEditNombreProducto" runat="server" 
                                    ControlToValidate="txtEditNombreProducto" ErrorMessage="El nombre del producto es obligatorio"
                                    ValidationGroup="EditarProducto" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <label for="txtEditDescripcion">Descripción</label>
                                <asp:TextBox ID="txtEditDescripcion" runat="server" CssClass="form-control" 
                                    TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="txtEditCantidadDisponible">Cantidad Disponible *</label>
                                <asp:TextBox ID="txtEditCantidadDisponible" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEditCantidadDisponible" runat="server" 
                                    ControlToValidate="txtEditCantidadDisponible" ErrorMessage="La cantidad disponible es obligatoria"
                                    ValidationGroup="EditarProducto" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                                <asp:RangeValidator ID="rvEditCantidadDisponible" runat="server" 
                                    ControlToValidate="txtEditCantidadDisponible" MinimumValue="0" MaximumValue="999999"
                                    Type="Integer" ErrorMessage="La cantidad debe ser mayor o igual a 0"
                                    ValidationGroup="EditarProducto" Display="Dynamic" CssClass="alert-error"></asp:RangeValidator>
                            </div>
                            <div class="form-group">
                                <label for="txtEditPrecio">Precio (₡) *</label>
                                <asp:TextBox ID="txtEditPrecio" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEditPrecio" runat="server" 
                                    ControlToValidate="txtEditPrecio" ErrorMessage="El precio es obligatorio"
                                    ValidationGroup="EditarProducto" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                                <asp:RangeValidator ID="rvEditPrecio" runat="server" 
                                    ControlToValidate="txtEditPrecio" MinimumValue="0.01" MaximumValue="999999.99"
                                    Type="Double" ErrorMessage="El precio debe ser mayor a ₡0.01"
                                    ValidationGroup="EditarProducto" Display="Dynamic" CssClass="alert-error"></asp:RangeValidator>
                            </div>
                            
                            <!-- Alertas de stock -->
                            <asp:Panel ID="pnlAlertaStock" runat="server" Visible="false">
                                <div style="margin-bottom: 15px;">
                                    <asp:Label ID="lblAlertaStock" runat="server" CssClass="alert alert-error"></asp:Label>
                                </div>
                            </asp:Panel>
                            
                            <div style="margin-top: 15px;">
                                <asp:Button ID="btnEditar" runat="server" Text="💾 Guardar Cambios" 
                                    CssClass="btn btn-success" OnClick="btnEditar_Click" 
                                    ValidationGroup="EditarProducto" />
                                <asp:Button ID="btnEliminar" runat="server" Text="🗑️ Eliminar Producto" 
                                    CssClass="btn btn-danger" OnClick="btnEliminar_Click" 
                                    OnClientClick="return confirm('¿Está seguro que desea eliminar este producto? Esta acción no se puede deshacer.');" />
                            </div>
                        </div>
                    </asp:Panel>
                </asp:Panel>

                <!-- Panel Lista de Productos -->
                <asp:Panel ID="pnlListar" runat="server" Visible="false">
                    <h3>📋 Lista de Productos Activos</h3>
                    <div style="margin-bottom: 15px;">
                        <asp:Button ID="btnActualizar" runat="server" Text="🔄 Actualizar Lista" 
                            CssClass="btn btn-primary" OnClick="btnActualizar_Click" />
                        <span style="margin-left: 20px; font-weight: bold;">Total de productos: 
                            <asp:Label ID="lblTotalProductos" runat="server" Text="0" style="color: #27ae60;"></asp:Label>
                        </span>
                        <span style="margin-left: 20px; font-weight: bold; color: #e74c3c;">Productos agotados: 
                            <asp:Label ID="lblProductosAgotados" runat="server" Text="0"></asp:Label>
                        </span>
                    </div>
                    <asp:GridView ID="gvProductos" runat="server" CssClass="table" AutoGenerateColumns="false" 
                        EmptyDataText="No hay productos registrados en el sistema." 
                        HeaderStyle-BackgroundColor="#27ae60" HeaderStyle-ForeColor="White">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="CodigoCabys" HeaderText="Código CABYS" />
                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:BoundField DataField="CantidadDisponible" HeaderText="Stock" />
                            <asp:BoundField DataField="Precio" HeaderText="Precio (₡)" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Registro" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:TemplateField HeaderText="Estado Stock">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoStock" runat="server" 
                                        Text='<%# (Convert.ToInt32(Eval("CantidadDisponible")) == 0) ? "❌ AGOTADO" : 
                                               (Convert.ToInt32(Eval("CantidadDisponible")) <= 5) ? "⚠️ BAJO" : "✅ DISPONIBLE" %>'
                                        ForeColor='<%# (Convert.ToInt32(Eval("CantidadDisponible")) == 0) ? System.Drawing.Color.Red : 
                                                      (Convert.ToInt32(Eval("CantidadDisponible")) <= 5) ? System.Drawing.Color.Orange : System.Drawing.Color.Green %>'
                                        Font-Bold="true">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>