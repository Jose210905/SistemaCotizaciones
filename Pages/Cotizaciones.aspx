<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cotizaciones.aspx.cs" Inherits="SistemaCotizaciones.Pages.Cotizaciones" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Sistema de Cotizaciones - Gestión de Cotizaciones</title>
    <link href="../CSS/styles.css" rel="stylesheet" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <script type="text/javascript">
        function calcularTotales() {
            var cantidad = parseFloat(document.getElementById('<%= txtCantidad.ClientID %>').value) || 0;
            var precio = parseFloat(document.getElementById('<%= txtPrecioUnitario.ClientID %>').value) || 0;
            var descuento = parseFloat(document.getElementById('<%= txtDescuento.ClientID %>').value) || 0;
            
            var bruto = cantidad * precio;
            var montoDescuento = (bruto * descuento) / 100;
            var subtotal = bruto - montoDescuento;
            var iva = subtotal * 0.13;
            var total = subtotal + iva;
            
            document.getElementById('<%= txtBruto.ClientID %>').value = bruto.toFixed(2);
            document.getElementById('<%= txtIVA.ClientID %>').value = iva.toFixed(2);
            document.getElementById('<%= txtTotal.ClientID %>').value = total.toFixed(2);
        }
    </script>
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
            <a href="Productos.aspx">📦 Productos</a>
            <a href="Cotizaciones.aspx" style="background-color: #2c3e50;">📋 Cotizaciones</a>
            <a href="Reportes.aspx">📊 Reportes</a>
            <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" style="color: white; text-decoration: none; padding: 10px 20px; margin: 0 5px;">🚪 Cerrar Sesión</asp:LinkButton>
        </div>

        <div class="container">
            <div class="form-container">
                <h2>📋 Gestión de Cotizaciones</h2>

                <!-- Mensajes de error/éxito -->
                <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>
                </asp:Panel>

                <!-- Pestañas -->
                <div style="margin-bottom: 20px;">
                    <asp:Button ID="btnTabCrear" runat="server" Text="➕ Crear Cotización" 
                        CssClass="btn btn-primary" OnClick="btnTabCrear_Click" />
                    <asp:Button ID="btnTabConsultar" runat="server" Text="🔍 Consultar Cotización" 
                        CssClass="btn" OnClick="btnTabConsultar_Click" />
                    <asp:Button ID="btnTabListar" runat="server" Text="📋 Lista de Cotizaciones" 
                        CssClass="btn" OnClick="btnTabListar_Click" />
                </div>

                <!-- Panel Crear Cotización -->
                <asp:Panel ID="pnlCrear" runat="server">
                    <h3>➕ Crear Nueva Cotización</h3>
                    
                    <div style="display: flex; gap: 30px;">
                        <!-- Columna izquierda - Datos básicos -->
                        <div style="flex: 1;">
                            <div class="form-group">
                                <label for="txtIdentificacionCliente">Identificación del Cliente *</label>
                                <div style="display: flex; gap: 10px;">
                                    <asp:TextBox ID="txtIdentificacionCliente" runat="server" CssClass="form-control" 
                                        placeholder="Ingrese identificación del cliente" style="flex: 1;"></asp:TextBox>
                                    <asp:Button ID="btnBuscarCliente" runat="server" Text="🔍 Buscar" 
                                        CssClass="btn btn-primary" OnClick="btnBuscarCliente_Click" />
                                </div>
                                <asp:RequiredFieldValidator ID="rfvIdentificacionCliente" runat="server" 
                                    ControlToValidate="txtIdentificacionCliente" ErrorMessage="La identificación del cliente es obligatoria"
                                    ValidationGroup="CrearCotizacion" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                            </div>
                            
                            <!-- Panel Cliente Encontrado -->
                            <asp:Panel ID="pnlClienteInfo" runat="server" Visible="false" 
                                style="background-color: #e8f5e8; padding: 10px; border-radius: 5px; margin-bottom: 15px;">
                                <strong>Cliente:</strong> <asp:Label ID="lblNombreCliente" runat="server"></asp:Label><br />
                                <strong>Email:</strong> <asp:Label ID="lblEmailCliente" runat="server"></asp:Label><br />
                                <strong>Teléfono:</strong> <asp:Label ID="lblTelefonoCliente" runat="server"></asp:Label>
                            </asp:Panel>

                            <div class="form-group">
                                <label for="ddlProducto">Producto *</label>
                                <asp:DropDownList ID="ddlProducto" runat="server" CssClass="form-control" 
                                    OnSelectedIndexChanged="ddlProducto_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Value="">Seleccionar producto...</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvProducto" runat="server" 
                                    ControlToValidate="ddlProducto" ErrorMessage="Debe seleccionar un producto"
                                    ValidationGroup="CrearCotizacion" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                            </div>

                            <!-- Panel Producto Info -->
                            <asp:Panel ID="pnlProductoInfo" runat="server" Visible="false" 
                                style="background-color: #e8f4f8; padding: 10px; border-radius: 5px; margin-bottom: 15px;">
                                <strong>Producto:</strong> <asp:Label ID="lblNombreProducto" runat="server"></asp:Label><br />
                                <strong>Código CABYS:</strong> <asp:Label ID="lblCodigoCabys" runat="server"></asp:Label><br />
                                <strong>Stock disponible:</strong> <asp:Label ID="lblStockDisponible" runat="server"></asp:Label><br />
                                <strong>Precio unitario:</strong> ₡<asp:Label ID="lblPrecioProducto" runat="server"></asp:Label>
                            </asp:Panel>

                            <div class="form-group">
                                <label for="txtCantidad">Cantidad *</label>
                                <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" 
                                    placeholder="Cantidad de productos" TextMode="Number" 
                                    onchange="calcularTotales();" onkeyup="calcularTotales();"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCantidad" runat="server" 
                                    ControlToValidate="txtCantidad" ErrorMessage="La cantidad es obligatoria"
                                    ValidationGroup="CrearCotizacion" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                                <asp:RangeValidator ID="rvCantidad" runat="server" 
                                    ControlToValidate="txtCantidad" MinimumValue="1" MaximumValue="9999"
                                    Type="Integer" ErrorMessage="La cantidad debe ser mayor a 0"
                                    ValidationGroup="CrearCotizacion" Display="Dynamic" CssClass="alert-error"></asp:RangeValidator>
                            </div>
                        </div>

                        <!-- Columna derecha - Cálculos -->
                        <div style="flex: 1;">
                            <div class="form-group">
                                <label for="txtPrecioUnitario">Precio Unitario (₡) *</label>
                                <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="form-control" 
                                    TextMode="Number" step="0.01" ReadOnly="true" 
                                    style="background-color: #f8f9fa;" 
                                    onchange="calcularTotales();" onkeyup="calcularTotales();"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label for="txtDescuento">Descuento (%)</label>
                                <asp:TextBox ID="txtDescuento" runat="server" CssClass="form-control" 
                                    placeholder="0" TextMode="Number" step="0.01" Text="0"
                                    onchange="calcularTotales();" onkeyup="calcularTotales();"></asp:TextBox>
                                <asp:RangeValidator ID="rvDescuento" runat="server" 
                                    ControlToValidate="txtDescuento" MinimumValue="0" MaximumValue="100"
                                    Type="Double" ErrorMessage="El descuento debe estar entre 0% y 100%"
                                    ValidationGroup="CrearCotizacion" Display="Dynamic" CssClass="alert-error"></asp:RangeValidator>
                            </div>

                            <div class="form-group">
                                <label for="txtBruto">Bruto (₡)</label>
                                <asp:TextBox ID="txtBruto" runat="server" CssClass="form-control" 
                                    ReadOnly="true" style="background-color: #f8f9fa;" Text="0.00"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label for="txtIVA">IVA 13% (₡)</label>
                                <asp:TextBox ID="txtIVA" runat="server" CssClass="form-control" 
                                    ReadOnly="true" style="background-color: #f8f9fa;" Text="0.00"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label for="txtTotal">Total (₡)</label>
                                <asp:TextBox ID="txtTotal" runat="server" CssClass="form-control" 
                                    ReadOnly="true" style="background-color: #fff3cd; font-weight: bold; font-size: 16px;" Text="0.00"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- Panel de alerta de stock -->
                    <asp:Panel ID="pnlAlertaStock" runat="server" Visible="false">
                        <div style="margin-bottom: 15px;">
                            <asp:Label ID="lblAlertaStock" runat="server" CssClass="alert alert-error"></asp:Label>
                        </div>
                    </asp:Panel>

                    <div style="margin-top: 20px;">
                        <asp:Button ID="btnCrearCotizacion" runat="server" Text="💾 Crear Cotización" 
                            CssClass="btn btn-success" OnClick="btnCrearCotizacion_Click" 
                            ValidationGroup="CrearCotizacion" />
                        <asp:Button ID="btnLimpiarCrear" runat="server" Text="🗑️ Limpiar" 
                            CssClass="btn" OnClick="btnLimpiarCrear_Click" />
                    </div>
                </asp:Panel>

                <!-- Panel Consultar Cotización -->
                <asp:Panel ID="pnlConsultar" runat="server" Visible="false">
                    <h3>🔍 Consultar Cotización</h3>
                    <div class="form-group">
                        <label for="txtConsultarIdentificacion">Identificación del Cliente</label>
                        <asp:TextBox ID="txtConsultarIdentificacion" runat="server" CssClass="form-control" 
                            placeholder="Ingrese la identificación del cliente"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnConsultar" runat="server" Text="🔍 Buscar Cotizaciones" 
                        CssClass="btn btn-primary" OnClick="btnConsultar_Click" />

                    <!-- GridView de cotizaciones del cliente -->
                    <asp:Panel ID="pnlCotizacionesCliente" runat="server" Visible="false" style="margin-top: 20px;">
                        <h4>📋 Cotizaciones del Cliente</h4>
                        <asp:GridView ID="gvCotizacionesCliente" runat="server" CssClass="table" AutoGenerateColumns="false" 
                            EmptyDataText="No hay cotizaciones para este cliente."
                            HeaderStyle-BackgroundColor="#e74c3c" HeaderStyle-ForeColor="White"
                            OnSelectedIndexChanged="gvCotizacionesCliente_SelectedIndexChanged" DataKeyNames="ID">
                            <Columns>
                                <asp:CommandField ShowSelectButton="true" SelectText="Ver" HeaderText="Acción" />
                                <asp:BoundField DataField="ID" HeaderText="ID Cotización" />
                                <asp:BoundField DataField="FechaCotizacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                <asp:BoundField DataField="Total" HeaderText="Total (₡)" DataFormatString="{0:N2}" />
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEstado" runat="server" 
                                            Text='<%# DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 30 ? "⏰ VENCIDA" : "✅ VIGENTE" %>'
                                            ForeColor='<%# DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 30 ? System.Drawing.Color.Red : System.Drawing.Color.Green %>'
                                            Font-Bold="true">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>

                    <!-- Panel Detalle de Cotización Seleccionada -->
                    <asp:Panel ID="pnlDetalleCotizacion" runat="server" Visible="false" style="margin-top: 20px;">
                        <h4>📄 Detalle de la Cotización</h4>
                        <div style="background-color: #f8f9fa; padding: 20px; border-radius: 5px;">
                            <div style="display: flex; gap: 30px;">
                                <div style="flex: 1;">
                                    <div class="form-group">
                                        <label for="txtEditCliente">Cliente *</label>
                                        <asp:TextBox ID="txtEditCliente" runat="server" CssClass="form-control" ReadOnly="true" 
                                            style="background-color: #e9ecef;"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtEditProducto">Producto *</label>
                                        <asp:TextBox ID="txtEditProducto" runat="server" CssClass="form-control" ReadOnly="true" 
                                            style="background-color: #e9ecef;"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtEditCantidad">Cantidad *</label>
                                        <asp:TextBox ID="txtEditCantidad" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvEditCantidad" runat="server" 
                                            ControlToValidate="txtEditCantidad" ErrorMessage="La cantidad es obligatoria"
                                            ValidationGroup="EditarCotizacion" Display="Dynamic" CssClass="alert-error"></asp:RequiredFieldValidator>
                                        <asp:RangeValidator ID="rvEditCantidad" runat="server" 
                                            ControlToValidate="txtEditCantidad" MinimumValue="1" MaximumValue="9999"
                                            Type="Integer" ErrorMessage="La cantidad debe ser mayor a 0"
                                            ValidationGroup="EditarCotizacion" Display="Dynamic" CssClass="alert-error"></asp:RangeValidator>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtEditDescuento">Descuento (%)</label>
                                        <asp:TextBox ID="txtEditDescuento" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                                        <asp:RangeValidator ID="rvEditDescuento" runat="server" 
                                            ControlToValidate="txtEditDescuento" MinimumValue="0" MaximumValue="100"
                                            Type="Double" ErrorMessage="El descuento debe estar entre 0% y 100%"
                                            ValidationGroup="EditarCotizacion" Display="Dynamic" CssClass="alert-error"></asp:RangeValidator>
                                    </div>
                                </div>
                                <div style="flex: 1;">
                                    <div class="form-group">
                                        <label for="txtEditPrecioUnitario">Precio Unitario (₡)</label>
                                        <asp:TextBox ID="txtEditPrecioUnitario" runat="server" CssClass="form-control" ReadOnly="true" 
                                            style="background-color: #e9ecef;"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtEditBruto">Bruto (₡)</label>
                                        <asp:TextBox ID="txtEditBruto" runat="server" CssClass="form-control" ReadOnly="true" 
                                            style="background-color: #e9ecef;"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtEditIVA">IVA 13% (₡)</label>
                                        <asp:TextBox ID="txtEditIVA" runat="server" CssClass="form-control" ReadOnly="true" 
                                            style="background-color: #e9ecef;"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtEditTotal">Total (₡)</label>
                                        <asp:TextBox ID="txtEditTotal" runat="server" CssClass="form-control" ReadOnly="true" 
                                            style="background-color: #fff3cd; font-weight: bold; font-size: 16px;"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            
                            <div style="margin-top: 15px;">
                                <asp:Button ID="btnRecalcular" runat="server" Text="🔄 Recalcular" 
                                    CssClass="btn btn-primary" OnClick="btnRecalcular_Click" />
                                <asp:Button ID="btnGuardarCambios" runat="server" Text="💾 Guardar Cambios" 
                                    CssClass="btn btn-success" OnClick="btnGuardarCambios_Click" 
                                    ValidationGroup="EditarCotizacion" />
                                <asp:Button ID="btnEliminarCotizacion" runat="server" Text="🗑️ Eliminar Cotización" 
                                    CssClass="btn btn-danger" OnClick="btnEliminarCotizacion_Click" 
                                    OnClientClick="return confirm('¿Está seguro que desea eliminar esta cotización? Esta acción no se puede deshacer.');" />
                            </div>
                        </div>
                    </asp:Panel>
                </asp:Panel>

                <!-- Panel Lista de Cotizaciones -->
                <asp:Panel ID="pnlListar" runat="server" Visible="false">
                    <h3>📋 Lista de Todas las Cotizaciones</h3>
                    <div style="margin-bottom: 15px;">
                        <asp:Button ID="btnActualizarLista" runat="server" Text="🔄 Actualizar Lista" 
                            CssClass="btn btn-primary" OnClick="btnActualizarLista_Click" />
                        <span style="margin-left: 20px; font-weight: bold;">Total de cotizaciones: 
                            <asp:Label ID="lblTotalCotizaciones" runat="server" Text="0" style="color: #e74c3c;"></asp:Label>
                        </span>
                        <span style="margin-left: 20px; font-weight: bold; color: #f39c12;">Cotizaciones vencidas: 
                            <asp:Label ID="lblCotizacionesVencidas" runat="server" Text="0"></asp:Label>
                        </span>
                    </div>
                    <asp:GridView ID="gvCotizaciones" runat="server" CssClass="table" AutoGenerateColumns="false" 
                        EmptyDataText="No hay cotizaciones registradas en el sistema." 
                        HeaderStyle-BackgroundColor="#e74c3c" HeaderStyle-ForeColor="White">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="NombreCliente" HeaderText="Cliente" />
                            <asp:BoundField DataField="IdentificacionCliente" HeaderText="Identificación" />
                            <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unit. (₡)" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="Descuento" HeaderText="Desc. %" DataFormatString="{0:N1}" />
                            <asp:BoundField DataField="Total" HeaderText="Total (₡)" DataFormatString="{0:N2}" />
                            <asp:BoundField DataField="FechaCotizacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lblEstadoLista" runat="server" 
                                        Text='<%# DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 30 ? "⏰ VENCIDA" : 
                                               DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 23 ? "⚠️ POR VENCER" : "✅ VIGENTE" %>'
                                        ForeColor='<%# DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 30 ? System.Drawing.Color.Red : 
                                                      DateTime.Now.Subtract((DateTime)Eval("FechaCotizacion")).Days > 23 ? System.Drawing.Color.Orange : System.Drawing.Color.Green %>'
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